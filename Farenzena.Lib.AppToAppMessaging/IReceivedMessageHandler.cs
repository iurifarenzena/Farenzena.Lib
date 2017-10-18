using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.AppToAppMessaging
{
    public interface IReceivedMessageHandler
    {
        Task HandleReceivedMessageAsync(MessageData messageData);
    }
}
