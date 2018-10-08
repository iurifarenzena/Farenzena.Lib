using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Farenzena.Lib.Diagnostic.Log.Observer.Json
{
    public class JSonLogObserver : ILogObserver
    {
        /// <summary>
        /// Tamanho limite do arquivo, defino em 5 MB
        /// </summary>
        private const int MAX_FILE_SIZE = (5 * 1024 * 1024);

        private readonly AsyncLock m_lock = new AsyncLock();

        private string _logStorageDirectory;

        private JSonLogObserver() { }

        public JSonLogObserver(string logStorageDirectory)
        {
            this._logStorageDirectory = logStorageDirectory;
        }

        private readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = ShouldSerializeContractResolver.Instance,
            DateFormatString = "dd-MM-yyyy HH:mm:ss"
        };

        public async Task StartSessionAsync(string sessionId)
        {
            using (await m_lock.LockAsync())
            {
                await GetFilePath(sessionId);
            }
        }

        public async Task HandleLogAsync(LogObject log, string sessionId)
        {
            using (await m_lock.LockAsync())
            {
                var filePath = await GetFilePath(sessionId);

                string conteudo = JsonConvert.SerializeObject(log, settings) + Environment.NewLine;

                File.AppendAllText(filePath, conteudo);

                //using (StreamWriter sw = new StreamWriter(File.OpenWrite(filePath, true))
                //{
                //    //write to the file
                //}
            }
        }

        private async Task<string> GetFilePath(string sessionId)
        {
            FileInfo fInfo = GetFileInfoForCurrentSession(sessionId);

            if (fInfo.Exists && fInfo.Length > MAX_FILE_SIZE)// Se o arquivo for maior que o tamanho limite
            {
                File.Copy(fInfo.FullName, $"{fInfo.FullName}.ext{DateTime.Now:ddMMyy_HHmm}"); // cria um backup do arquivo "lotado"

                File.Delete(fInfo.FullName); // remove o arquivo "lotado"
                await Task.Delay(500); // Aguarda para garantir que o sistema alterou o nome do arquivo
                fInfo = GetFileInfoForCurrentSession(sessionId);// Recria um arquivo como nome original da sessão

            }

            return fInfo.FullName;
        }

        private FileInfo GetFileInfoForCurrentSession(string sessionId)
        {
            DirectoryInfo dInfo = new DirectoryInfo(_logStorageDirectory);

            if (!dInfo.Exists)
                dInfo.Create();

            var fileName = $"{sessionId}.json";

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

        public class ShouldSerializeContractResolver : DefaultContractResolver
        {
            public new static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();


            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);

                switch (property.PropertyName)
                {
                    case "Message":
                        property.PropertyName = "msg";
                        break;
                    case "Parameters":
                        property.PropertyName = "params";
                        break;
                    case "LogType":
                        property.PropertyName = "type";
                        break;
                    case "Source":
                        property.PropertyName = "src";
                        break;
                    case "TimeStamp":
                        property.PropertyName = "tstmp";
                        break;
                    default:
                        break;
                }
                return property;
            }
        }
    }
}
