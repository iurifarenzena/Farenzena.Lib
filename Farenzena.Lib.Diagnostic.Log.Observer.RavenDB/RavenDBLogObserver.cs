using Raven.Client;
using Raven.Client.Documents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Farenzena.Lib.Diagnostic.Log.Observer.RavenDB
{
    public class RavenDBLogObserver : ILogObserver
    {
        private readonly IDocumentStore _documentStore;
        private readonly RavenDBLogExpirationOptions _expirations;

        public RavenDBLogObserver(IDocumentStore documentStore, RavenDBLogExpirationOptions expirations = null)
        {
            _documentStore = documentStore;
            _expirations = expirations ?? new RavenDBLogExpirationOptions();
        }

        public async Task HandleLogAsync(LogObject log, string sessionId)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                await session.StoreAsync(log);
                var expiration = GetExpirationForLogEntry(log);

                if (expiration != Timeout.InfiniteTimeSpan)
                {
                    var metaData = session.Advanced.GetMetadataFor(log);
                    metaData[Constants.Documents.Metadata.Expires] = DateTime.UtcNow.Add(expiration);
                }
                await session.SaveChangesAsync();
            }
        }

        private TimeSpan GetExpirationForLogEntry(LogObject log)
        {
            switch (log.LogType)
            {
                case ELogType.Debug: return _expirations.DebugExpiration;
                case ELogType.Info: return _expirations.Info;
                case ELogType.Warning: return _expirations.Warning;
                case ELogType.Error: return _expirations.Error;
                case ELogType.Evidence: return _expirations.Evidence;
                default: return Timeout.InfiniteTimeSpan;
            }
        }

        public Task StartSessionAsync(string sessionId)
        {
            return Task.CompletedTask;
        }
    }
}
