using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.AppToAppMessaging
{
    public interface IMessagingServer
    {
        void SendMessage(MessageData messageData);
    }
}
