using Microsoft.Data.ConnectionUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Farenzena.Lib.Database.Connection.FormsUI
{
    public class WinFormsDialogConnectionConfigurator : IDatabaseConnectionConfigurator
    {
        public bool Configure(ref DatabaseConnectionConfiguration configuration)
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
