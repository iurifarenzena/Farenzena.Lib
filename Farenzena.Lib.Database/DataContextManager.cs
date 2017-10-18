using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Farenzena.Lib.Database
{
    public static class DataContextManager
    {
        private static IDataContextHandler _dataContextHandler;
        private static ThreadLocal<Dictionary<string, object>> _threadLocalObjects = new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>());

        public static bool Initialized { get; private set; }

        public static void Initialize(IDataContextHandler dataContextHandler)
        {
            if (!Initialized)
            {
                Initialized = true;
                _dataContextHandler = dataContextHandler;
            }
        }

        private static void CheckInitializationNeeded()
        {
            if (!Initialized)
                throw new InvalidOperationException("The DataContextManager must be initialized first");
        }

        public static object GetDataContextForTypeOfPOCO(Type pocoType, bool useSharedDataContext = true)
        {
            CheckInitializationNeeded();

            if(!useSharedDataContext)
                return _dataContextHandler.GetDataContextForTypeOfPOCO(pocoType);
            else
            {
                var dataContextId = pocoType.Name;
                if (!_threadLocalObjects.Value.ContainsKey(dataContextId))
                {
                    var contexto = _dataContextHandler.GetDataContextForTypeOfPOCO(pocoType);
                    _threadLocalObjects.Value.Add(dataContextId, contexto);
                }

                return _threadLocalObjects.Value[dataContextId];
            }
        }

        public static IRepository<TPoco> GetRepository<TPoco>(bool useSharedDataContext = true) where TPoco : class 
        {
            CheckInitializationNeeded();
            return _dataContextHandler.GetRepository<TPoco>(GetDataContextForTypeOfPOCO(typeof(TPoco), useSharedDataContext));
        }

        public static void DisposeDataContexts(bool commitChanges)
        {
            CheckInitializationNeeded();
            var contexts = new List<object>(_threadLocalObjects.Value.Values);
            _dataContextHandler.DisposeDataContexts(contexts, commitChanges);
            contexts.Clear();
            contexts = null;
            _threadLocalObjects.Value.Clear();
        }
    }
}
