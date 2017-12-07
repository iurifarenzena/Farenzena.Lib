using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.AsyncMessaging.WCFPushingQueue
{
    internal class ConsumerSideMessageHandlerServerClient : ClientBase<IConsumerSideMessageHandlerServer>, IConsumerSideMessageHandlerServer
    {
        public ConsumerSideMessageHandlerServerClient(Binding binding, EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        public string SendMessage(MessageDTO messageDTO)
        {
            return base.Channel.SendMessage(messageDTO);
        }
    }
}
