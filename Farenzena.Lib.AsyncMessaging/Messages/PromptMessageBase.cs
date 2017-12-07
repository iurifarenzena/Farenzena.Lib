using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.AsyncMessaging.Messages
{
    /// <summary>
    /// Special message that expects a reply
    /// </summary>
    /// <typeparam name="TReplyType">Type of the explected reply</typeparam>
    public abstract class PromptMessageBase<TReplyType> : BasicMessage
    {
        public PromptMessageBase(string title, string content, EMessageCategory category = EMessageCategory.Interrogative) : base(title, content, category)
        {
        }

        /// <summary>
        /// Holds the reponse to the given message
        /// </summary>
        public TReplyType Reply { get; private set; }

        public void SetReplay(TReplyType replay)
        {
            this.Reply = replay;
        }
    }
}
