using Farenzena.Lib.AsyncMessaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.AsyncMessaging
{
    /// <summary>
    /// Relates a message with an action to be called after the message is handled
    /// </summary>
    internal class MessageCallback
    {
        public MessageCallback(BasicMessage message, Action<BasicMessage> callbackAction = null)
        {
            Message = message;
            CallbackAction = callbackAction;
        }

        internal BasicMessage Message { get; private set; }
        internal Action<BasicMessage> CallbackAction { get; private set; }
    }
}
