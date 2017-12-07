using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.AsyncMessaging.Messages
{
    public abstract class OptionsMessageBase<TOptionsType, TReplyType> : PromptMessageBase<TReplyType>
    {
        public OptionsMessageBase(string title, string content, EMessageCategory category = EMessageCategory.Interrogative) : base(title, content, category)
        {
            this.Options = new List<ReplyOption<TOptionsType>>();
        }

        public List<ReplyOption<TOptionsType>> Options { get; private set; }

        public void AddOption(TOptionsType value, string displayText)
        {
            Options.Add(new ReplyOption<TOptionsType>(value, displayText));
        }
    }
}
