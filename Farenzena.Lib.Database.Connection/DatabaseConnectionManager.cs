using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.Database.Connection
{
    public static class DatabaseConnectionManager
    {
        public static bool Initialized { get; private set; }

        private static IDictionary<string, DatabaseConnectionConfiguration> _configurations;
        private static IDatabaseConnectionConfigurationStorage _configurationsStorage;

        public static void Initialize(IDatabaseConnectionConfigurationStorage configurationsStorage)
        {
            if (!Initialized)
            {
                Initialized = true;
                _configurationsStorage = configurationsStorage;
            }
            else
                throw new InvalidOperationException("The DatabaseConnectionManager is already initialized");
        }

        public static void SaveConfiguration(DatabaseConnectionConfiguration config)
        {
            CheckInitializationNeeded();

            var configs = GetConfigurations();
            configs[config.ConnectionId] = config;
            _configurationsStorage.SaveConfigurations(configs);
        }

        private static void CheckInitializationNeeded()
        {
            if (!Initialized)
                throw new InvalidOperationException("The DatabaseConnectionManager must be initialized first");
        }

        public static DatabaseConnectionConfiguration GetConfiguration(string connectionId)
        {
            CheckInitializationNeeded();

            var configs = GetConfigurations();

            DatabaseConnectionConfiguration config;
            configs.TryGetValue(connectionId, out config);

            if (config == null)
                throw new DatabaseConnectionException($"Impossible to retrieve the connection [{connectionId}]. It might not have being configured yet.");

            return config;
        }

        private static IDictionary<string, DatabaseConnectionConfiguration> GetConfigurations()
        {
            if (_configurations == null)
            {
                _configurations = _configurationsStorage.GetConfigurations();
            }
            return _configurations;
        }
    }
}
