using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.WindowsService
{
    public static class CriticalErrorLogger
    {
        /// <summary>
        /// Creates a log file in the %temp% folder
        /// </summary>
        /// <param name="error"></param>
        /// <param name="serviceName"></param>
        public static void LogCriticalError(Exception error, string serviceName)
        {
            var arquivo = $"error_{serviceName}_log_{DateTime.Now:dd-MM-yyyy_HHmmss}.txt";
            arquivo = Path.Combine(Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine), arquivo);
            File.WriteAllText(arquivo, $"{DateTime.Now} -> {error.Message}");
            var errors = error.InnerException;

            var tabs = "\t";
            while (errors != null)
            {
                File.AppendAllText(arquivo, $"{Environment.NewLine}{tabs}{DateTime.Now} -> {errors.Message}");
                errors = errors.InnerException;
                tabs += "\t";
            }
        }
    }
}
