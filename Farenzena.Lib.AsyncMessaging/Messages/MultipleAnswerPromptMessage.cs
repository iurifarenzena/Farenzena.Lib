using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.AsyncMessaging.Messages
{
    public class MultipleAnswerPromptMessage<TAnsewrType> : OptionsMessageBase<TAnsewrType, List<TAnsewrType>>
    {
        public MultipleAnswerPromptMessage(string title, string content, EMessageCategory category = EMessageCategory.Interrogative) : base(title, content, category)
        {
        }
    }
}
