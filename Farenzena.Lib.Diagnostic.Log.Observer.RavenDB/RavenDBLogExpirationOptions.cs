using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Farenzena.Lib.Diagnostic.Log.Observer.RavenDB
{
    public class RavenDBLogExpirationOptions
    {
        public TimeSpan DebugExpiration = TimeSpan.FromDays(60);
        public TimeSpan Error = TimeSpan.FromDays(180);
        public TimeSpan Warning = TimeSpan.FromDays(90);
        public TimeSpan Info = TimeSpan.FromDays(90);
        public TimeSpan Evidence = Timeout.InfiniteTimeSpan;
    }
}
