using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.Database.Connection
{
    public class DatabaseConnectionConfiguration
    {
        public string ConnectionId { get; set; }
        public string ConnectionString { get; set; }
        public EDatabaseType DatabaseType { get; set; }
    }
}
