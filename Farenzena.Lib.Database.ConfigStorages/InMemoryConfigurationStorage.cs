using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.Database.Connection.ConfigStorages
{
    public class InMemoryConfigurationStorage : IDatabaseConnectionConfigurationStorage
    {
        private IDictionary<string, DatabaseConnectionConfiguration> _configurations;

        public InMemoryConfigurationStorage()
        {
            _configurations = new Dictionary<string, DatabaseConnectionConfiguration>();
        }

        public IDictionary<string, DatabaseConnectionConfiguration> GetConfigurations()
        {
            return _configurations;
        }

        public void SaveConfigurations(IDictionary<string, DatabaseConnectionConfiguration> configurations)
        {
            _configurations = configurations;
        }
    }
}
