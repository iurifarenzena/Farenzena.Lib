using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.AsyncMessaging.Messages
{
    /// <summary>
    /// Prompt message for yes/true no/false questions
    /// </summary>
    public class BooleanPromptMessage : PromptMessageBase<bool>
    {
        public BooleanPromptMessage(string title, string content, EMessageCategory category = EMessageCategory.Interrogative) : base(title, content, category)
        {
        }
    }
}
