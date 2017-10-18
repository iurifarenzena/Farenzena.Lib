using Farenzena.Lib.Database.Connection;
using Farenzena.Lib.Database.Connection.ConfigStorages;
using Farenzena.Lib.Database.Connection.FormsUI;
using Farenzena.Lib.Database.EFCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Farenzena.Lib.Database.Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello World!");
                var contextProvider = new EFCoreDataContextHandler();
                contextProvider.RegisterDataContextType(typeof(BloggingContext));
                
                var appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var confStorage = new JsonConfigurationStorage(appPath);
                if (DatabaseConnectionConfigurator.CheckDatabaseConnection(typeof(BloggingContext).Name, confStorage, (conf) => contextProvider.CheckConnectionForDataContext(typeof(BloggingContext),conf), true))
                {
                    DataContextManager.Initialize(contextProvider);
                    var context = contextProvider.GetDataContextOfType(typeof(BloggingContext)) as BloggingContext;
                    context.Database.EnsureCreated();
                    System.Windows.Forms.MessageBox.Show("Sucesso!!!");
                }
                else
                    System.Windows.Forms.MessageBox.Show("Não Deu!!!");
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);
            }
        }


       
    }
}
