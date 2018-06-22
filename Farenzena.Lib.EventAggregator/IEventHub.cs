using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.EventAggregator
{
    public interface IEventHub
    {
        void Subscribe(object subscriber);
        void Unsubscribe(object subscriber);
        void Publish<TEvent>(TEvent eventToPublish);
    }
}
