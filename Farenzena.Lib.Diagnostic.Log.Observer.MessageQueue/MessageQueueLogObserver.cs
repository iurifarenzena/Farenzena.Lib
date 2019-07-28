using Farenzena.Lib.AsyncMessaging;
using System;
using System.Threading.Tasks;

namespace Farenzena.Lib.Diagnostic.Log.Observer.MessageQueue
{
    public class MessageQueueLogObserver : ILogObserver
    {
        public Task HandleLogAsync(LogObject log, string sessionId)
        {
            var category = EMessageCategory.Informative;

            if (log.LogType == ELogType.Error)
                category = EMessageCategory.Error;
            else if (log.LogType == ELogType.Warning)
                category = EMessageCategory.Warning;

            AsyncMessaging.MessageQueue.SendMessage(log.Source, log.Message, category);

            return Task.CompletedTask;
        }

        public Task StartSessionAsync(string sessionId)
        {
            return Task.CompletedTask;
        }
    }
}
