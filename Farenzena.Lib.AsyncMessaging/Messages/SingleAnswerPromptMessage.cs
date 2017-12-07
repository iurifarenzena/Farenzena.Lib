using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.AsyncMessaging.Messages
{
    public class SingleAnswerPromptMessage<TAnsewrType> : OptionsMessageBase<TAnsewrType, TAnsewrType>
    {
        public SingleAnswerPromptMessage(string title, string content, EMessageCategory category = EMessageCategory.Interrogative) : base(title, content, category)
        {
        }
    }
}
