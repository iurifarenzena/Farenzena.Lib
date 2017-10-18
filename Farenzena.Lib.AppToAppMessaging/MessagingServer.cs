using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.AppToAppMessaging
{
    public class MessagingServer : IMessagingServer
    {
        public MessagingServer(IReceivedMessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
        }

        public IReceivedMessageHandler messageHandler { get; private set; }

        public void SendMessage(MessageData messageData)
        {
            if (messageHandler == null)
                throw new InvalidOperationException("There is no message handler defined in this server");

        }
    }
}
