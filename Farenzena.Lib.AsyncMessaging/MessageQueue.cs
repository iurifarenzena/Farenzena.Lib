using Farenzena.Lib.AsyncMessaging.Messages;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.AsyncMessaging
{
    public static class MessageQueue
    {
        private const int NUM_MESSAGES_KEEP = 1000;
        /// <summary>
        /// Indicates whether or not the class was initialized
        /// </summary>
        private static bool _initialized;
        /// <summary>
        /// Indicates whether or not the message queue is being handled already
        /// </summary>
        private static bool _busyHandlingMessages;
        /// <summary>
        /// Instance of the message manager that implements IMessageHandler
        /// </summary>
        private static IMessageHandler _messageHandlerInstance;
        /// <summary>
        /// Message queue
        /// </summary>
        private static LinkedList<MessageCallback> _messageQueue;

        ///// <summary>
        ///// Initialize a passive message queue, which will just hold the messages until they are consumed
        ///// </summary>
        //public static void InicializePassive()
        //{
        //    if (!_initialized)
        //    {
        //        _initialized = true;
        //        _messageQueue = new Queue<MessageCallback>();
        //    }
        //}

        /// <summary>
        /// Initialize a message queue that will push each message to consumers
        /// </summary>
        /// <param name="messageHandler">Connects the queue to consumers</param>
        public static void InicializePushing(IMessageHandler messageHandler)
        {
            if (!_initialized)
            {
                _initialized = true;
                _messageQueue = new LinkedList<MessageCallback>();
                _messageHandlerInstance = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
            }
        }

        /// <summary>
        /// Sends a basic message to the queue
        /// </summary>
        /// <param name="content">Message content</param>
        /// <param name="category">Message category</param>
        public static void SendMessage(string content, EMessageCategory category = EMessageCategory.Informative)
        {
            SendMessage(new BasicMessage(string.Empty, content, category), null);
        }

        /// <summary>
        /// Sends a basic message to the queue
        /// </summary>
        /// <param name="title">Message title</param>
        /// <param name="content">Message content</param>
        /// <param name="category">Message category</param>
        public static void SendMessage(string title, string content, EMessageCategory category = EMessageCategory.Informative)
        {
            SendMessage(new BasicMessage(title, content, category), null);
        }

        /// <summary>
        /// Sends a prompt message and awaits for a response
        /// </summary>
        /// <typeparam name="TAnswer">Type of the expected answer</typeparam>
        /// <param name="message">Message to send</param>
        /// <returns></returns>
        public static async Task<TAnswer> SendMessageAndWait<TAnswer>(PromptMessageBase<TAnswer> message)
        {
            TaskCompletionSource<TAnswer> taskCompletionSource = new TaskCompletionSource<TAnswer>();
            SendMessage(message, m => taskCompletionSource.SetResult(message.Reply));
            return await taskCompletionSource.Task;
        }

        /// <summary>
        /// Sends a message to the queue
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="callback">Callback action to fire after the message gets handled</param>
        public static void SendMessage(BasicMessage message, Action<BasicMessage> callback = null)
        {
            if (!_initialized)
                throw new InvalidOperationException("The Message Queue Handler was not initialized");

            while (_messageQueue.Count >= NUM_MESSAGES_KEEP)
                _messageQueue.RemoveFirst();

            // Enqueeu a callback for this messagem
            MessageCallback messageCallback = new MessageCallback(message, callback);
            _messageQueue.AddLast(messageCallback);
            //Stars the queue processing
            HandleMessageQueueAsync();
        }

        public static async void HandleMessageQueueAsync()
        {
            // If there is any message, and the queue is not being handled yet
            if (!_busyHandlingMessages && _messageHandlerInstance != null && _messageQueue.Any())
            {
                _busyHandlingMessages = true;

                await Task.Run(async () =>
                {
                    try
                    {
                        var firstNode = _messageQueue.First;
                        //Verifica se há alguma mensagem na fila de mensagens.
                        while (firstNode != null)
                        {
                            MessageCallback m = firstNode.Value;
                            var messageType = m.Message.GetType();

                            //Verifica qual tipo a mensagem em questão é para a chamada de método apropriada.
                            if (m.Message is BooleanPromptMessage)
                                await _messageHandlerInstance.HandleBooleanPromptMessageAsync(m.Message as BooleanPromptMessage);
                            else if (messageType.IsConstructedGenericType)
                            {
                                if (messageType.GetGenericTypeDefinition() == (typeof(SingleAnswerPromptMessage<>)))
                                {
                                    var method = typeof(IMessageHandler).GetMethod(nameof(_messageHandlerInstance.HandleSingleAnswerPromptMessageAsync));
                                    await (Task)method.MakeGenericMethod(messageType.GetGenericArguments()).Invoke(_messageHandlerInstance, new object[1] { m.Message });
                                }
                                else if (messageType.GetGenericTypeDefinition() == (typeof(MultipleAnswerPromptMessage<>)))
                                {
                                    var method = typeof(IMessageHandler).GetMethod(nameof(_messageHandlerInstance.HandleMultipleAnswerPromptMessageAsync));
                                    await (Task)method.MakeGenericMethod(messageType.GetGenericArguments()).Invoke(_messageHandlerInstance, new object[1] { m.Message });
                                }
                                else
                                    throw new NotSupportedException($"There is no handler for the message type: {messageType}");
                            }
                            else
                                await _messageHandlerInstance.HandleMessageAsync(m.Message);

                            //Caso a mensagem tenha algum callback ele é executado
                            m.CallbackAction?.Invoke(m.Message);

                            firstNode = firstNode.Next;
                            _messageQueue.RemoveFirst();
                        }
                    }
                    catch (MessageHandlerUnavailableException)
                    {

                    }
                });

                _busyHandlingMessages = false;
            }
        }
    }
}
