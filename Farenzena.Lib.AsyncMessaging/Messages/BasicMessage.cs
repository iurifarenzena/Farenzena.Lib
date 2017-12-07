using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.AsyncMessaging.Messages
{
    public class BasicMessage
    {
        /// <summary>
        /// Message title.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Message content.
        /// </summary>
        public string Content { get; set; }
        public EMessageCategory Category { get; set; }
        public DateTime DateCreated { get; set; }

        public BasicMessage(string title, string content, EMessageCategory category = EMessageCategory.Informative)
        {
            Title = title;
            Content = content;
            Category = category;
            DateCreated = DateTime.Now;
        }

        private BasicMessage() { }
    }
}
