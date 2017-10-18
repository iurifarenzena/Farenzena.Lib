using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.AppToAppMessaging
{
    public class MessageData
    {
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public object[] Parameters { get; set; }
    }
}
