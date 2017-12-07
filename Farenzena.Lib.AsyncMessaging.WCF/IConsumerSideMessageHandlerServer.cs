using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.AsyncMessaging.WCFPushingQueue
{
    [ServiceContract]
    public interface IConsumerSideMessageHandlerServer
    {
        [OperationContract]
        string SendMessage(MessageDTO messageDTO);
    }
}
