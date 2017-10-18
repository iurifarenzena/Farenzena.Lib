using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using Farenzena.Lib.Database.Connection;

namespace Farenzena.Lib.Database
{
    public abstract class DataContextHandlerBase : IDataContextHandler
    {
        public Dictionary<Type, List<Type>> ContextToPocoTypesRelation { get; private set; } = new Dictionary<Type, List<Type>>();

        public abstract bool CheckConnectionForDataContext(Type dbContextType, DatabaseConnectionConfiguration connectionConfiguration);
        public abstract void DisposeDataContexts(List<object> dataContexts, bool commitChanges);
        public abstract List<Type> GetAcceptedPocoTypes(Type dataContextType);
        public abstract object GetDataContextOfType(Type dataContextType);
        public abstract IRepository<TPoco> GetRepository<TPoco>(object dataContext) where TPoco : class;
        
        public object GetDataContextForTypeOfPOCO(Type pocoType)
        {
            // If none context type was registered, there is no point in looking up
            if (!ContextToPocoTypesRelation.Any())
                return null;
            // If there is only one context type registered, just assume this is the right one
            else if (ContextToPocoTypesRelation.Count == 1)
                return GetDataContextOfType(ContextToPocoTypesRelation.Keys.First());
            else
            {
                // Check if one of the registered contexts can handle the POCO type
                foreach (var contextType in ContextToPocoTypesRelation.Keys)
                {
                    if(ContextToPocoTypesRelation[contextType].Contains(pocoType))
                        return GetDataContextOfType(contextType);
                }
            }

            return null;
        }
     
        public void RegisterDataContextType(Type dataContextType)
        {
            RegisterDataContextType(dataContextType, GetAcceptedPocoTypes(dataContextType));
        }

        public void RegisterDataContextType(Type dataContextType, List<Type> pocoTypes)
        {
            if (!ContextToPocoTypesRelation.ContainsKey(dataContextType))
            {
                ContextToPocoTypesRelation.Add(dataContextType, pocoTypes);
            }
        }
    }
}
