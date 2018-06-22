using Farenzena.Lib.Database.Connection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.Database
{
    public interface IDataContextHandler
    {
        bool CheckConnectionForDataContext(Type dbContextType, DatabaseConnectionConfiguration connectionConfiguration);

        Dictionary<Type,List<Type>> ContextToPocoTypesRelation { get; }

        void DisposeDataContexts(List<object> dataContexts, bool commitChanges);

        List<Type> GetAcceptedPocoTypes(Type dataContextType);

        [Obsolete("This method must not be used. Use DataContextManager.GetDataContext<TDataContext> instead.", true)]
        object GetDataContextForTypeOfPOCO(Type pocoType);

        Type GetDataContextTypeForPOCOType(Type pocoType);

        object GetDataContextOfType(Type dataContextType);
        
        IRepository<TPoco> GetRepository<TPoco>(object dataContext) where TPoco : class;
        
        void RegisterDataContextType(Type dataContextType);

        void RegisterDataContextType(Type dataContextType, List<Type> pocoTypes);
    }
}
