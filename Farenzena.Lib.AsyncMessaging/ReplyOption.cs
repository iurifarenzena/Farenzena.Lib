using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.AsyncMessaging
{
    public class ReplyOption<TReplyType> : Tuple<TReplyType, string>
    {
        public ReplyOption(TReplyType value, string displayText) : base(value, displayText)
        {
        }

        public TReplyType Value { get { return this.Item1; } }
        public string DisplayText { get { return this.Item2; } }
    }
}
