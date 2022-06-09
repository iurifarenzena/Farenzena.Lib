using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.Diagnostic.Log
{
    public class LogObject
    {
        private LogObject()
        {
            TimeStamp = DateTime.Now;
        }

        public LogObject(string message, ELogType logType, string souce) 
            : this(message, logType, null, souce)
        {
        }

        public LogObject(string message, ELogType logType, Dictionary<string, object> parameters = null, string souce = null) 
            : this()
        {
            Message = message;
            Parameters = parameters;
            LogType = logType;
            Source = souce;
        }

        /// <summary>
        /// Record creation time
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The log type. It can be marked as an error, a debug, a warning, etc ...
        /// </summary>
        public ELogType LogType { get; private set; }

        /// <summary>
        /// The application/module where logging occurred
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// The message that describes the log
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// A parameter dictionary to receive information pertinent to the log.
        /// The first object represents the name of the parameter, example "Operator", and the second the value, example "02".
        /// </summary>
        public Dictionary<string, object> Parameters { get; private set; }

        public void AddParameter(string key, object value)
        {
            if (Parameters == null)
                Parameters = new Dictionary<string, object>();
            Parameters.Add(key, value);
        }

        public void SetSorceIfEmpty(string sorce)
        {
            if (string.IsNullOrEmpty(this.Source))
                this.Source = sorce;
        }
    }
}
