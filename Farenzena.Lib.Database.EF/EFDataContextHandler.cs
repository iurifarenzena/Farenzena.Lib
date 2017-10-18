
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using Farenzena.Lib.Database.Connection;
using System.Data.Entity;
using System.Data.Common;
using Farenzena.Lib.Database.EF.Configurations;
using Oracle.ManagedDataAccess.Client;

namespace Farenzena.Lib.Database.EF
{
    public class EFDataContextHandler : DataContextHandlerBase
    {
        private Dictionary<Type, Func<DbContext>> _dataContextConstructors = new Dictionary<Type, Func<DbContext>>();

        public EFDataContextHandler() : base()
        {
            DbConfiguration.SetConfiguration(new EF6CodeConfig());
        }

        public override bool CheckConnectionForDataContext(Type dbContextType, DatabaseConnectionConfiguration connectionConfiguration)
        {
            try
            {
                var context = GetDataContextOfType(dbContextType) as DbContext;
                context.Database.Connection.Open();
                context.Database.Connection.Close();
                return true;
            }
            catch (DatabaseConnectionException)
            {
                return false;
            }
        }

        public override void DisposeDataContexts(List<object> dataContexts, bool commitChanges)
        {
            if (dataContexts.Count > 1)
            {
                // If there are more than one dbContext to dispose, applies transactions to ensure integrity
                LimparComTrancations(dataContexts, commitChanges);
            }
            else
            {
                foreach (DbContext dbcontext in dataContexts.ConvertAll(o => o as DbContext))
                {
                    if (commitChanges)
                        dbcontext?.SaveChanges();
                    dbcontext?.Dispose();
                }
            }
        }

        private void LimparComTrancations(List<object> dataContexts, bool commitChanges)
        {
            var transactions = new List<Tuple<DbContext, DbContextTransaction>>();

            foreach (DbContext dbcontext in dataContexts.OfType<DbContext>())
            {
                if (commitChanges)
                {
                    System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.Snapshot;

                    if (dbcontext.Database.Connection is OracleConnection)
                        isolationLevel = System.Data.IsolationLevel.ReadCommitted;

                    transactions.Add(new Tuple<DbContext, DbContextTransaction>(dbcontext, dbcontext.Database.BeginTransaction(isolationLevel)));
                    dbcontext?.SaveChanges();
                }
                else
                {
                    transactions.Add(new Tuple<DbContext, DbContextTransaction>(dbcontext, null));
                }
            }

            foreach (var transaction in transactions)
            {
                transaction.Item2?.Commit();
                transaction.Item2?.Dispose();
                transaction.Item1.Dispose();
            }

            transactions.Clear();
        }

        public override IRepository<TPoco> GetRepository<TPoco>(object dataContext)
        {
            return new EFBaseRepository<TPoco>(dataContext as DbContext);
        }

        public override object GetDataContextOfType(Type dataContextType)
        {
            // Get the generic method `Foo`
            var fooMethod = typeof(EFDataContextHandler).GetMethod(nameof(GetDataContextConstructor), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            // Make the non-generic method via the `MakeGenericMethod` reflection call.
            // Yes - this is confusing Microsoft!!
            var fooOfBarMethod = fooMethod.MakeGenericMethod(dataContextType);

            // Invoke the method just like a normal method.
            var contructorFunc = fooOfBarMethod.Invoke(this, null) as Func<DbContext>;

            //return GetDataContextConstructor(dataContextType)();
            return contructorFunc();
        }

        public override List<Type> GetAcceptedPocoTypes(Type dataContextType)
        {
            var types = new List<Type>();
            foreach (var field in (dataContextType as TypeInfo).DeclaredFields)
            {
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition().Name.StartsWith("DbSet"))
                    types.Add(field.FieldType.GenericTypeArguments[0]);
            }
            return types;
        }

        private Func<TDataContext> GetDataContextConstructor<TDataContext>() where TDataContext : DbContext
        {
            if (_dataContextConstructors.ContainsKey(typeof(TDataContext)))
                return (Func<TDataContext>)_dataContextConstructors[typeof(TDataContext)];
            else
            {
                return () =>
                {
                    var config = DatabaseConnectionManager.GetConfiguration(typeof(TDataContext).Name);
                    var conn = GetConnection(config);
                    return Activator.CreateInstance(typeof(TDataContext), conn) as TDataContext;
                };
            }
        }

        private DbConnection GetConnection(DatabaseConnectionConfiguration config)
        {
            if (config.DatabaseType == EDatabaseType.MSSQLServer)
                return GetSqlServerConnection(config);
            else if (config.DatabaseType == EDatabaseType.Oracle)
                return GetOracleConnection(config);
            else
                throw new DatabaseConnectionException($"Connections with database type {config.DatabaseType} are not supported yet.");
        }

        private DbConnection GetOracleConnection(DatabaseConnectionConfiguration config)
        {
            return new Oracle.ManagedDataAccess.EntityFramework.OracleConnectionFactory().CreateConnection(config.ConnectionString);
        }

        private DbConnection GetSqlServerConnection(DatabaseConnectionConfiguration config)
        {
            return new System.Data.Entity.Infrastructure.SqlConnectionFactory().CreateConnection(config.ConnectionString);
        }
    }
}
