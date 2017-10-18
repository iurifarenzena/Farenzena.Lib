using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.Database.EF.Configurations
{
    public class EF6CodeConfig : DbConfiguration
    {
        public EF6CodeConfig()
        {
            SetProviderServices("Oracle.ManagedDataAccess.Client", EFOracleProviderServices.Instance);
            SetProviderServices("System.Data.SqlClient", System.Data.Entity.SqlServer.SqlProviderServices.Instance);

            SetProviderFactory("Oracle.ManagedDataAccess.Client", new OracleClientFactory());
            //SetProviderFactory("System.Data.SqlClient", new SqlConnectionFactory());
        }
    }
}
