using System;
using System.Collections;

namespace Farenzena.Lib.Diagnostic.Log
{
    internal class ExceptionLogHelpper
    {
        internal static LogObject CreateExceptionLog(Exception ex)
        {
            var log = new LogObject(CreateExceptionMessage(ex), ELogType.Error, ex.Source);

            AddExceptionDataAsLogParameter(ex, log);
            LogInnerException(ex, log);

            return log;
        }

        private static void LogInnerException(Exception ex, LogObject log, int level = 1)
        {
            if (ex.InnerException != null)
            {
                var innerEx = ex.InnerException;
                var excepLevel = $"ExLevel{level}";

                log.AddParameter(excepLevel, CreateExceptionMessage(innerEx));
                AddExceptionDataAsLogParameter(innerEx, log, excepLevel);

                LogInnerException(innerEx, log, ++level);
            }
        }

        private static void AddExceptionDataAsLogParameter(Exception ex, LogObject log, string prefixo = "")
        {
            foreach (DictionaryEntry item in ex.Data)
            {
                log.AddParameter($"{prefixo}{item.Key}", item.Value);
            }
        }

        private static string CreateExceptionMessage(Exception ex)
        {
            return $"{ex.Message} =>> {ex.StackTrace}";
        }
    }
}
