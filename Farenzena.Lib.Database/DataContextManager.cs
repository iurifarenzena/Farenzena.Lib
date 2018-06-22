using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Farenzena.Lib.Database
{
    public static class DataContextManager
    {
        internal static IDataContextHandler DataContextHandler { get; private set; }
        private static ThreadLocal<Dictionary<string, object>> _threadLocalObjects = new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>());

        public static bool Initialized { get; private set; }

        public static void Initialize(IDataContextHandler dataContextHandler)
        {
            if (!Initialized)
            {
                Initialized = true;
                DataContextHandler = dataContextHandler;
            }
        }

        private static void CheckInitializationNeeded()
        {
            if (!Initialized)
                throw new InvalidOperationException("The DataContextManager must be initialized first");
        }

        [Obsolete("This method must not be used. Use DataContextManager.GetDataContext<TDataContext> instead.", true)]
        public static object GetDataContextForTypeOfPOCO(Type pocoType, bool useSharedDataContext = true)
        {
            return null;

            //CheckInitializationNeeded();

            //if(!useSharedDataContext)
            //    return _dataContextHandler.GetDataContextForTypeOfPOCO(pocoType);
            //else
            //{
            //    var dataContextId = pocoType.Name;
            //    if (!_threadLocalObjects.Value.ContainsKey(dataContextId))
            //    {
            //        var contexto = _dataContextHandler.GetDataContextForTypeOfPOCO(pocoType);
            //        _threadLocalObjects.Value.Add(dataContextId, contexto);
            //    }

            //    return _threadLocalObjects.Value[dataContextId];
            //}
        }

        public static TDataContext GetDataContext<TDataContext>(bool useSharedDataContext = true) where TDataContext : class
        {
            return GetDataContext(typeof(TDataContext),useSharedDataContext) as TDataContext;
        }

        public static object GetDataContext(Type dataContextType, bool useSharedDataContext = true)
        {
            CheckInitializationNeeded();

            if (!useSharedDataContext)
                return DataContextHandler.GetDataContextOfType(dataContextType);
            else
            {
                var dataContextId = dataContextType.Name;
                if (!_threadLocalObjects.Value.ContainsKey(dataContextId))
                {
                    var contexto = DataContextHandler.GetDataContextOfType(dataContextType);
                    _threadLocalObjects.Value.Add(dataContextId, contexto);
                }

                return _threadLocalObjects.Value[dataContextId];
            }
        }

        public static IRepository<TPoco> GetRepository<TPoco>(bool useSharedDataContext = true) where TPoco : class 
        {
            CheckInitializationNeeded();

            var ctxType = DataContextHandler.GetDataContextTypeForPOCOType(typeof(TPoco));
            return DataContextHandler.GetRepository<TPoco>(GetDataContext(ctxType, useSharedDataContext));
        }

        public static void DisposeDataContexts(bool commitChanges)
        {
            CheckInitializationNeeded();
            var contexts = new List<object>(_threadLocalObjects.Value.Values);
            DataContextHandler.DisposeDataContexts(contexts, commitChanges);
            contexts.Clear();
            contexts = null;
            _threadLocalObjects.Value.Clear();
        }
    }
}
