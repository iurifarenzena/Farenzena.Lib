using Farenzena.Lib.Database.Connection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Farenzena.Lib.Database.Connection.ConfigStorages
{
    public class JsonConfigurationStorage : IDatabaseConnectionConfigurationStorage
    {
        private JsonConfigurationStorage() { }

        public JsonConfigurationStorage(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        private string _configFilePath;

        public IDictionary<string, DatabaseConnectionConfiguration> GetConfigurations()
        {
            Dictionary<string, DatabaseConnectionConfiguration> _configuracoes;

            {
                var conteudoArquivo = File.ReadAllText(GetFileInfo().FullName);
                if (conteudoArquivo.Length > 0)
                    _configuracoes = JsonConvert.DeserializeObject<Dictionary<string, DatabaseConnectionConfiguration>>(conteudoArquivo);
                else
                    _configuracoes = new Dictionary<string, DatabaseConnectionConfiguration>();

                return _configuracoes;
            }
        }

        public void SaveConfigurations(IDictionary<string, DatabaseConnectionConfiguration> configurations)
        {
            var conteudo = JsonConvert.SerializeObject(configurations);

            File.WriteAllText(GetFileInfo().FullName, conteudo);
        }

        private FileInfo GetFileInfo()
        {
            DirectoryInfo dInfo = new DirectoryInfo(_configFilePath);

            if (!dInfo.Exists)
                dInfo.Create();

            var fileName = "dbconfig.json";

            var filePath = Path.Combine(dInfo.FullName, fileName);

            if (!File.Exists(filePath))
            {
                using (var stream = File.Create(filePath))
                {
                    stream.Flush();
                }
            }

            return new FileInfo(filePath);
        }
    }
}
