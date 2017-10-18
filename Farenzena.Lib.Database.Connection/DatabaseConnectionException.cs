using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.Database.Connection
{
    public class DatabaseConnectionException : Exception
    {
        public DatabaseConnectionException() : base()
        {
        }

        public DatabaseConnectionException(string message) : base(message)
        {
        }
    }
}
