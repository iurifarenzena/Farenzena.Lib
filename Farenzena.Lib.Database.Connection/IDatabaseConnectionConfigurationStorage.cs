using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.Database.Connection
{
    public interface IDatabaseConnectionConfigurationStorage
    {
        void SaveConfigurations(IDictionary<string, DatabaseConnectionConfiguration> configurations);
        IDictionary<string, DatabaseConnectionConfiguration> GetConfigurations();
    }
}
