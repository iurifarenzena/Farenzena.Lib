using System;
using System.Threading.Tasks;

namespace Farenzena.Lib.Diagnostic.Log.Observer.Console
{
    public class ConsoleLogObserver : ILogObserver
    {
        public static string DefaultLogReceivedMessage = "## - Log message reveived - ##";
        public Task HandleLogAsync(LogObject log, string sessionId)
        {
            if (log.LogType == ELogType.Error)
                System.Console.ForegroundColor = ConsoleColor.Red;
            else if (log.LogType == ELogType.Warning)
                System.Console.ForegroundColor = ConsoleColor.Yellow;

            System.Console.WriteLine();
            System.Console.WriteLine(DefaultLogReceivedMessage);
            System.Console.WriteLine($"\t{log.LogType} - {log.Source} - {log.TimeStamp}");
            System.Console.WriteLine($"\t{log.Message}");
            if (log.Parameters != null)
            {
                foreach (var para in log.Parameters)
                {
                    System.Console.WriteLine($"\t\t[{para.Key}] => {para.Value}");
                }
            }
            System.Console.WriteLine();

            System.Console.ResetColor();

            return Task.CompletedTask;
        }

        public Task StartSessionAsync(string sessionId)
        {
            return Task.CompletedTask;
        }
    }
}
