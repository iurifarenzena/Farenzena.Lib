using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.EventAggregator
{
    public class EventHub : IEventHub
    {
        private readonly Dictionary<Type, List<WeakReference>> eventSubsribers = new Dictionary<Type, List<WeakReference>>();
        private readonly Dictionary<Type, List<WeakReference>> asyncEventSubsribers = new Dictionary<Type, List<WeakReference>>();

        private readonly object lockSubscriberDictionary = new object();

        public void Publish<TEvent>(TEvent eventToPublish)
        {
            InvokeAsyncSubscribers(eventToPublish);

            var subscriberType = typeof(IEventSubscriber<>).MakeGenericType(typeof(TEvent));

            var subscribers = GetEventSubscribersList(eventSubsribers, subscriberType);

            // Remove inactive subscribers from the list
            subscribers.RemoveAll(r => !r.IsAlive);

            foreach (var weakSubsriber in subscribers)
            {
                if (weakSubsriber.IsAlive)
                {
                    (weakSubsriber.Target as IEventSubscriber<TEvent>).HandleEvent(eventToPublish);

                    if ((eventToPublish as ICancelableEventArgs)?.Cancel ?? false)
                        break;
                }
            }
        }

        private void InvokeAsyncSubscribers<TEvent>(TEvent eventToPublish)
        {
            var subscriberType = typeof(IAsyncEventSubscriber<>).MakeGenericType(typeof(TEvent));

            var subscribers = GetEventSubscribersList(asyncEventSubsribers, subscriberType);

            // Remove inactive subscribers from the list
            subscribers.RemoveAll(r => !r.IsAlive);

            if (subscribers.Any())
            {
                var tasks = new List<Task>(subscribers.Count);

                foreach (var weakSubsriber in subscribers)
                {
                    if (weakSubsriber.IsAlive) // Gets a task for the async function being executed on the subscriber
                        tasks.Add((weakSubsriber.Target as IAsyncEventSubscriber<TEvent>).HandleEventAsync(eventToPublish));
                }
                Task.WaitAll(tasks.ToArray());
            }
        }

        /// <summary>
        /// Subscribe an objetc to receive notifcations for 
        /// </summary>
        /// <param name="subscriber"></param>
        public void Subscribe(object subscriber)
        {
            lock (lockSubscriberDictionary)
            {
                // Creates the weak reference which will allow to call the subscriber later
                WeakReference weakReference = new WeakReference(subscriber);
                // Subscribe sync event handlers
                foreach (var subscriptionType in GetSubscriptionsFromSubscriber(subscriber, typeof(IEventSubscriber<>)))
                {
                    List<WeakReference> subscribers = GetEventSubscribersList(eventSubsribers, subscriptionType);
                    subscribers.Add(weakReference);
                }
                // Subscribe async event handlers
                foreach (var subscriptionType in GetSubscriptionsFromSubscriber(subscriber, typeof(IAsyncEventSubscriber<>)))
                {
                    List<WeakReference> asyncSubscribers = GetEventSubscribersList(asyncEventSubsribers, subscriptionType);
                    asyncSubscribers.Add(weakReference);
                }
            }
        }

        public void Unsubscribe(object subscriber)
        {
            lock (lockSubscriberDictionary)
            {
                // Creates the weak reference which will allow to call the subscriber later
                WeakReference weakReference = new WeakReference(subscriber);
                // Subscribe sync event handlers
                foreach (var subscriptionType in GetSubscriptionsFromSubscriber(subscriber, typeof(IEventSubscriber<>)))
                {
                    List<WeakReference> subscribers = GetEventSubscribersList(eventSubsribers, subscriptionType);
                    subscribers.Remove(weakReference);
                }
                // Subscribe async event handlers
                foreach (var subscriptionType in GetSubscriptionsFromSubscriber(subscriber, typeof(IAsyncEventSubscriber<>)))
                {
                    List<WeakReference> asyncSubscribers = GetEventSubscribersList(asyncEventSubsribers, subscriptionType);
                    asyncSubscribers.Remove(weakReference);
                }
            }
        }

        /// <summary>
        /// Gets all subscriptions of a given interface implemented by the subscriber 
        /// </summary>
        /// <param name="subscriptionType">Specific interface to check subscriptions on (sync or async)</param>
        private IEnumerable<Type> GetSubscriptionsFromSubscriber(object subscriber, Type subscriptionType)
        {
            return subscriber.GetType()
                             .GetTypeInfo()
                             .ImplementedInterfaces
                             .Where(i =>
                                        i.GetTypeInfo().IsGenericType
                                        &&
                                        i.GetGenericTypeDefinition() == subscriptionType);
        }

        private List<WeakReference> GetEventSubscribersList(Dictionary<Type, List<WeakReference>> source, Type subscriberType)
        {
            lock (lockSubscriberDictionary)
            {
                if (!source.TryGetValue(subscriberType, out var subsribersList))
                    source.Add(subscriberType, subsribersList = new List<WeakReference>());
                return subsribersList;
            }
        }
    }
}
