using Farenzena.Lib.Database.Connection;
using Farenzena.Lib.Diagnostic.Log;
using System;

namespace Farenzena.Lib.Database
{
    public static class DatabaseConfigurator
    {
        public static IDatabaseConnectionConfigurator DatabaseConnectionConfigurator { get; private set; }

        public static IDatabaseConnectionConfigurationStorage DatabaseConnectionConfigurationStorage { get; private set; }

        public static void InitializeDatabaseAccessClasses(IDataContextHandler dataContextHandler, IDatabaseConnectionConfigurationStorage configurationStorage, IDatabaseConnectionConfigurator connectionConfigurator, params Type[] dbContextTypes)
        {
            DatabaseConnectionConfigurationStorage = configurationStorage;

            DatabaseConnectionConfigurator = connectionConfigurator;

            DataContextManager.Initialize(dataContextHandler);

            DatabaseConnectionManager.Initialize(configurationStorage);

            foreach (var dbType in dbContextTypes)
            {
                dataContextHandler.RegisterDataContextType(dbType);
            }
        }

        public static bool CheckRegisteredDatabasesConnections(bool allowConfiguration, bool forceConfiguration = false)
        {
            LoggerService.LogDebugAsync(() => $"").Wait();
            System.IO.File.AppendAllLines("log.txt", new string[] { "\n", "CheckRegisteredDatabasesConnections" });
            foreach (var dbcType in DataContextManager.DataContextHandler.ContextToPocoTypesRelation.Keys)
            {
                System.IO.File.AppendAllLines("log.txt", new string[] { "\n", dbcType.FullName });
                if (!CheckDatabaseConnection(dbcType, allowConfiguration))
                    return false;
            }

            return true;
        }

        public static bool CheckDatabaseConnection(Type dataContextType, bool allowConfiguration, bool forceConfiguration = false)
        {
            var connectionId = dataContextType.Name;

            return CheckDatabaseConnection(connectionId, confi => DataContextManager.DataContextHandler.CheckConnectionForDataContext(dataContextType, confi), allowConfiguration, forceConfiguration);
        }

        public static bool CheckDatabaseConnection(string connectionId, Func<DatabaseConnectionConfiguration, bool> validateConfigFunc, bool allowConfiguration, bool forceConfiguration = false)
        {
            System.IO.File.AppendAllLines("log.txt", new string[] { "\n", $"CheckDatabaseConnection: {connectionId}" });

            if ((allowConfiguration || forceConfiguration) && DatabaseConnectionConfigurator == null)
                throw new InvalidOperationException("The DatabaseConnectionConfigurator is null");

            var connectionSucceeded = false;
            DatabaseConnectionConfiguration config = null;

            try
            {
                config = DatabaseConnectionManager.GetConfiguration(connectionId);
            }
            catch (DatabaseConnectionException)
            {
                config = new DatabaseConnectionConfiguration() { ConnectionId = connectionId };
            }

            //do
            {
                try
                {
                    // Try the first time without configuring
                    if (forceConfiguration || !validateConfigFunc(config))
                    {
                        // If it fails, try to configure, and then check again
                        if ((allowConfiguration || forceConfiguration) && DatabaseConnectionConfigurator.Configure(ref config))
                        {
                            DatabaseConnectionManager.SaveConfiguration(config);
                            connectionSucceeded = validateConfigFunc(config);
                        }
                    }
                    else
                        connectionSucceeded = true;
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllLines("log.txt", new string[] { "\n", $"CheckDatabaseConnection ERROR: {ex}" });
                }

            } //while (!connectionSucceeded && allowConfiguration);

            return connectionSucceeded;
        }
    }
}
