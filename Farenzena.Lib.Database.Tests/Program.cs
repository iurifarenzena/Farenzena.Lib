using Farenzena.Lib.Database.Connection;
using Farenzena.Lib.Database.Connection.ConfigStorages;
using Farenzena.Lib.Database.Connection.FormsUI;
using Farenzena.Lib.Database.EFCore;
using Microsoft.Data.ConnectionUI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Farenzena.Lib.Database.Tests
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var contextProvider = new EFCoreDataContextHandler();
            var appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var confStorage = new JsonConfigurationStorage(appPath);

            InitializeDatabaseAccessClasses(contextProvider, confStorage, typeof(BloggingContext));
        }

        public static bool InitializeDatabaseAccessClasses(IDataContextHandler dataContextHandler, IDatabaseConnectionConfigurationStorage configurationStorage, params Type[] dbContextTypes)
        {
            DataContextManager.Initialize(dataContextHandler);

            foreach (var dbType in dbContextTypes)
            {
                dataContextHandler.RegisterDataContextType(dbType);
                if (!DatabaseConnectionConfigurator.CheckDatabaseConnection(dbType.Name, configurationStorage, (conf) => dataContextHandler.CheckConnectionForDataContext(dbType, conf), true))
                    return false;
            }

            return true;
        }

        public class BloggingContext : DbContext
        {
            public DbSet<Blog> Blogs { get; set; }
            public DbSet<Post> Posts { get; set; }

            //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            //{
            //    //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
            //}
        }

        public class Blog
        {
            public int BlogId { get; set; }
            public string Url { get; set; }
            public int Rating { get; set; }
            public List<Post> Posts { get; set; }
        }

        public class Post
        {
            public int PostId { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }

            public int BlogId { get; set; }
            public Blog Blog { get; set; }
        }

    }
}
