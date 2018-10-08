using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using Farenzena.Lib.Database.Connection;

namespace Farenzena.Lib.Database.EFCore
{
    public class EFCoreDataContextHandler : DataContextHandlerBase
    {
        private Dictionary<Type, Func<DbContext>> _dataContextConstructors = new Dictionary<Type, Func<DbContext>>();
        
        public override bool CheckConnectionForDataContext(Type dbContextType, DatabaseConnectionConfiguration connectionConfiguration)
        {
            try
            {
                var context = GetDataContextOfType(dbContextType) as DbContext;
                context.Database.OpenConnection();

                return true;
            }
            catch (DatabaseConnectionException bde)
            {
                System.IO.File.WriteAllText("dberror.txt", bde.ToString());
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
            var transactions = new List<Tuple<DbContext, IDbContextTransaction>>();

            foreach (DbContext dbcontext in dataContexts.OfType<DbContext>())
            {
                if (commitChanges)
                {
                    transactions.Add(new Tuple<DbContext, IDbContextTransaction>(dbcontext, dbcontext.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot)));
                    dbcontext?.SaveChanges();
                }
                else
                {
                    transactions.Add(new Tuple<DbContext, IDbContextTransaction>(dbcontext, null));
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
            return new EFCoreBaseRepository<TPoco>(dataContext as DbContext);
        }

        public override object GetDataContextOfType(Type dataContextType)
        {
            // Get the generic method `Foo`
            var fooMethod = typeof(EFCoreDataContextHandler).GetMethod(nameof(GetDataContextConstructor), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

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
                return CreateDataContextConstructor<TDataContext>();
            }
        }

        private Func<TDataContext> CreateDataContextConstructor<TDataContext>()where TDataContext : DbContext
        {
            return () =>
            {
                var optionsBuilder = GetDbContextOptionsBuilder<TDataContext>();
                return Activator.CreateInstance(typeof(TDataContext), optionsBuilder.Options) as TDataContext;
            };
        }

        private DbContextOptionsBuilder<TDataContext> GetDbContextOptionsBuilder<TDataContext>() where TDataContext : DbContext
        {
            var config = DatabaseConnectionManager.GetConfiguration(typeof(TDataContext).Name);
            
            var dbOptions = new DbContextOptionsBuilder<TDataContext>();
            if (config.DatabaseType == EDatabaseType.MSSQLServer)
                ConfigureForMSSQLServer(config, dbOptions);
            else if (config.DatabaseType == EDatabaseType.SQLite)
                ConfigureForSQLite(config, dbOptions);
            else if (config.DatabaseType == EDatabaseType.InMemoryTempDB)
                ConfigureForInMemoryTempDB(config, dbOptions);
            else
                throw new DatabaseConnectionException($"Connections with database type {config.DatabaseType} are not supported yet.");

            return dbOptions;
        }

        private void ConfigureForInMemoryTempDB<TDataContext>(DatabaseConnectionConfiguration config, DbContextOptionsBuilder<TDataContext> dbContextOptionsBuilder) where TDataContext : DbContext
        {
            dbContextOptionsBuilder.UseInMemoryDatabase(config.ConnectionString);
        }

        private void ConfigureForSQLite<TDataContext>(DatabaseConnectionConfiguration config, DbContextOptionsBuilder<TDataContext> dbContextOptionsBuilder) where TDataContext : DbContext
        {
            dbContextOptionsBuilder.UseSqlite(config.ConnectionString);
        }

        private void ConfigureForMSSQLServer<TDataContext>(DatabaseConnectionConfiguration config, DbContextOptionsBuilder<TDataContext> dbContextOptionsBuilder) where TDataContext : DbContext
        {
            dbContextOptionsBuilder.UseSqlServer(config.ConnectionString);
        }
    }
}
