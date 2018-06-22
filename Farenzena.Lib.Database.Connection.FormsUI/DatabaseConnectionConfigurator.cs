using Microsoft.Data.ConnectionUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Farenzena.Lib.Database.Connection.FormsUI
{
    [Obsolete("This class is obsolete and must not be used anymore. For initialization use ")]
    public static class DatabaseConnectionConfigurator
    {
        public static bool InitializeDatabaseAccessClasses(IDataContextHandler dataContextHandler, IDatabaseConnectionConfigurationStorage configurationStorage, bool showConfigurationInterface, params Type[] dbContextTypes)
        {
            DataContextManager.Initialize(dataContextHandler);

            foreach (var dbType in dbContextTypes)
            {
                dataContextHandler.RegisterDataContextType(dbType);
                if (!DatabaseConnectionConfigurator.CheckDatabaseConnection(dbType.Name, configurationStorage, (conf) => dataContextHandler.CheckConnectionForDataContext(dbType, conf), showConfigurationInterface))
                    return false;
            }

            return true;
        }

        public static bool CheckDatabaseConnection(string connectionId, IDatabaseConnectionConfigurationStorage configurationStorage, Func<DatabaseConnectionConfiguration, bool> validateConfigFunc, bool showConfigurationInterface)
        {
            try
            {
                if (!DatabaseConnectionManager.Initialized)
                    DatabaseConnectionManager.Initialize(configurationStorage);

                var config = DatabaseConnectionManager.GetConfiguration(connectionId);

                if (!validateConfigFunc(config))
                    return showConfigurationInterface && Configure(connectionId, validateConfigFunc);
            }
            catch (Exception e)
            {
                return showConfigurationInterface && Configure(connectionId, validateConfigFunc);
            }

            return true;
        }

        private static bool Configure(string connectionId, Func<DatabaseConnectionConfiguration, bool> validateConfigFunc)
        {
            var configurationIsOk = false;

            try
            {
                using (var dialog = new DataConnectionDialog())
                {
                    dialog.ChooseDataSourceTitle = $"Database connection: {connectionId}";

                    dialog.Title = $"Database connection: {connectionId}";

                    // If you want the user to select from any of the available data sources, do this:
                    DataSource.AddStandardDataSources(dialog);

                    // OR, if you want only certain data sources to be available
                    // (e.g. only SQL Server), do something like this instead: 
                    //dialog.DataSources.Add(DataSource.SqlDataSource);
                    //dialog.DataSources.Add(DataSource.SqlFileDataSource);

                    while (!configurationIsOk)
                    {
                        // The way how you show the dialog is somewhat unorthodox; `dialog.ShowDialog()`
                        // would throw a `NotSupportedException`. Do it this way instead:
                        DialogResult userChoice = DataConnectionDialog.Show(dialog);

                        // Return the resulting connection string if a connection was selected:
                        if (userChoice == DialogResult.OK)
                        {
                            var config = new DatabaseConnectionConfiguration();
                            config.ConnectionString = dialog.ConnectionString;
                            config.ConnectionId = connectionId;
                            if (dialog.SelectedDataSource.Name.Contains("Oracle"))
                                config.DatabaseType = EDatabaseType.Oracle;
                            else
                                config.DatabaseType = EDatabaseType.MSSQLServer;

                            DatabaseConnectionManager.SaveConfiguration(config);

                            configurationIsOk = validateConfigFunc(config);
                        }
                        else
                            configurationIsOk = false;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return configurationIsOk;
        }


        public static bool Configure(DatabaseConnectionConfiguration configuration)
        {
            try
            {
                using (var dialog = new DataConnectionDialog())
                {
                    dialog.ChooseDataSourceTitle = $"Database connection: {configuration.ConnectionId}";

                    dialog.Title = $"Database connection: {configuration.ConnectionId}";

                    DataSource.AddStandardDataSources(dialog);
                    
                    if (!string.IsNullOrEmpty(configuration.ConnectionString))
                    {
                        if (configuration.DatabaseType == EDatabaseType.MSSQLServer)
                            dialog.SelectedDataSource = DataSource.SqlDataSource;
                        else
                            dialog.SelectedDataSource = DataSource.OracleDataSource;

                        dialog.ConnectionString = configuration.ConnectionString;
                    }

                    DialogResult userChoice = DataConnectionDialog.Show(dialog);

                    // Return the resulting connection string if a connection was selected:
                    if (userChoice == DialogResult.OK)
                    {
                        configuration.ConnectionString = dialog.ConnectionString;
                        
                        if (dialog.SelectedDataSource.Name.Contains("Oracle"))
                            configuration.DatabaseType = EDatabaseType.Oracle;
                        else
                            configuration.DatabaseType = EDatabaseType.MSSQLServer;

                        DatabaseConnectionManager.SaveConfiguration(configuration);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return false;
        }
    }
}
