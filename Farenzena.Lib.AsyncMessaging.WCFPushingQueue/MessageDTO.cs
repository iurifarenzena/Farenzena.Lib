using Farenzena.Lib.AsyncMessaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.AsyncMessaging.WCFPushingQueue
{
    [DataContract]
    public class MessageDTO
    {
        [DataMember]
        public string Title { get; private set; }
        [DataMember]
        public string Content { get; private set; }
        [DataMember]
        public EMessageCategory Category { get; private set; }
        [DataMember]
        public DateTime DateCreated { get; private set; }

        public MessageDTO(string title, string content, EMessageCategory category = EMessageCategory.Informative)
        {
            Title = title;
            Content = content;
            Category = category;
            DateCreated = DateTime.Now;
        }

        public MessageDTO(BasicMessage basicMessage)
        {
            Title = basicMessage.Title;
            Content = basicMessage.Content;
            Category = basicMessage.Category;
            DateCreated = basicMessage.DateCreated;
        }
    }
}
