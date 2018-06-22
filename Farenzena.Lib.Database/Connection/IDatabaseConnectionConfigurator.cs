using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.Database.Connection
{
    public interface IDatabaseConnectionConfigurator
    {
        bool Configure(ref DatabaseConnectionConfiguration configuration);
    }
}
