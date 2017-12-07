using Farenzena.Lib.AsyncMessaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.AsyncMessaging.WCFPushingQueue
{
    public class ConsumerSideMessageHandlerServer : IConsumerSideMessageHandlerServer
    {
        public ConsumerSideMessageHandlerServer(IMessageHandler consumerMessageHandler)
        {
            ConsumerMessageHandler = consumerMessageHandler ?? throw new ArgumentNullException(nameof(consumerMessageHandler));
        }

        public IMessageHandler ConsumerMessageHandler { get; private set; }

        public string SendMessage(MessageDTO messageDTO)
        {
            var message = new BasicMessage(messageDTO.Title, messageDTO.Content, messageDTO.Category);
            // For now, it is not necessary to wait, because no response is given
            ConsumerMessageHandler.HandleMessageAsync(message);
            return "ok";
        }
    }
}
