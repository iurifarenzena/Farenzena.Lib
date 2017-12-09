using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.AsyncMessaging.WCFPushingQueue
{
    public static class ConsumerConfigurationHelper
    {
        private static ServiceHost _messageServerHost;

        public static void StartMessageServerLocalhost(IMessageHandler messageHandler, string messageServiceID)
        {
            var messageServer = new ConsumerSideMessageHandlerServer(messageHandler);

            _messageServerHost = new ServiceHost(messageServer, new Uri($"net.pipe://localhost/{messageServiceID}/"));

            ((ServiceBehaviorAttribute)_messageServerHost.Description.Behaviors[typeof(ServiceBehaviorAttribute)]).InstanceContextMode = InstanceContextMode.Single;

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            _messageServerHost.AddServiceEndpoint(typeof(IConsumerSideMessageHandlerServer), binding, "netpipeendpoint");

            _messageServerHost.Open(TimeSpan.FromSeconds(10));
        }

        public static void StopMessageServer()
        {
            _messageServerHost.Close(TimeSpan.FromSeconds(10));
        }
    }
}
