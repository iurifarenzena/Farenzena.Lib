using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.Diagnostic.Log
{
    public interface ILogObserver
    {
        Task StartSessionAsync(string sessionId);
        Task HandleLogAsync(LogObject log, string sessionId);
    }
}
