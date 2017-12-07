using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.AsyncMessaging.WCFPushingQueue
{
    public static class SourceConfigurationHelper
    {
        private static Binding _binding;
        private static EndpointAddress _remoteAddress;

        public static void ConfigureLocalhostServerClient(string messageServiceID)
        {
            _binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            _remoteAddress = new EndpointAddress($"net.pipe://localhost/{messageServiceID}/netpipeendpoint");

            SourceSideMessageHandler sourceSideMessageHandler = new SourceSideMessageHandler();
            MessageQueue.InicializePushing(sourceSideMessageHandler);
        }

        internal static ConsumerSideMessageHandlerServerClient GetMessageServerClient()
        {
            if (_binding == null || _remoteAddress == null)
                throw new InvalidOperationException("A conection to a message handler server was not initilized");
            return new ConsumerSideMessageHandlerServerClient(_binding, _remoteAddress);
        }
    }
}
