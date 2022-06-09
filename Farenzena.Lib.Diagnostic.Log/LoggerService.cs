using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.Diagnostic.Log
{
    public static class LoggerService
    {
        private static List<ILogObserver> _logObserversList;
        public static string DefaultSourceName { get; private set; }
        public static string SessionId { get; private set; }
        public static bool DebugEnabled { get; private set; }
        public static bool Initialized { get; private set; }

        public static void EnableDebug()
        {
            SetDebugEnable(true);
        }

        public static void DisableDebug()
        {
            SetDebugEnable(false);
        }

        public static void SetDebugEnable(bool enable)
        {
            DebugEnabled = enable;
        }

        public static async Task InicializeAsync(string applicationName, List<ILogObserver> logObservers, bool debugEnabled = false)
        {
            if (!Initialized)
            {
                _logObserversList = logObservers;
                DefaultSourceName = applicationName;
                SessionId = $"Log{applicationName}_{DateTime.Now:dd-MM-yyyy_HHmmss}";
                DebugEnabled = debugEnabled;

                foreach (var obs in _logObserversList)
                {
                    await obs.StartSessionAsync(SessionId);
                }

                Initialized = true;

                await LogAsync(new LogObject($"Application {applicationName} started", ELogType.Info, applicationName));
            }
        }

        private static async Task PostLogInternal(LogObject log)
        {
            if (Initialized)
            {
                log.SetSorceIfEmpty(DefaultSourceName);
                foreach (var obs in _logObserversList)
                {
                    await obs.HandleLogAsync(log, SessionId);
                }                
            }
        }

        public static async Task LogAsync(LogObject log)
        {
            if (Initialized)
                await PostLogInternal(log);
        }

        public static async Task LogAsync(Exception ex)
        {
            if (Initialized)
                await LogAsync(ex, null);
        }

        public static async Task LogAsync(Exception ex, dynamic parameters)
        {
            if (Initialized)
            {
                var log = ExceptionLogHelpper.CreateExceptionLog(ex);
                FillDynamicParameters(parameters, log);
                await PostLogInternal(log);
            }
        }

        public static async Task LogAsync(string message, ELogType logType = ELogType.Info, dynamic parameters = null)
        {
            await LogAsync(message, logType, DefaultSourceName, parameters);
        }

        public static async Task LogAsync(string message, ELogType logType = ELogType.Info, string source = null, dynamic parameters = null)
        {
            if (Initialized)
            {
                LogObject log = CreateLogObject(message, logType, source, parameters);
                await PostLogInternal(log);
            }
        }

        public static async Task LogDebugAsync(string message)
        {
            if (DebugEnabled && Initialized)
            {
                System.Diagnostics.Debug.WriteLine(message);
                await LogAsync(message, ELogType.Debug, null);
            }
        }

        public static async Task LogDebugAsync(string message, dynamic parameters)
        {
            if (DebugEnabled && Initialized)
            {
                System.Diagnostics.Debug.WriteLine(message);
                await LogAsync(message, ELogType.Debug, parameters);
            }
        }

        public static async Task LogDebugAsync(string message, Func<dynamic> getParametersFunction = null)
        {
            if (DebugEnabled && Initialized)
            {
                System.Diagnostics.Debug.WriteLine(message);
                await LogAsync(message, ELogType.Debug, getParametersFunction?.Invoke());
            }
        }

        public static async Task LogDebugAsync(Func<string> getMessageFunction, Func<dynamic> getParametersFunction = null)
        {
            if (DebugEnabled && Initialized)
            {
                var message = getMessageFunction();
                var parameters = getParametersFunction != null ? getParametersFunction() : null;
                System.Diagnostics.Debug.WriteLine(message);
                await LogAsync(message, ELogType.Debug, parameters);
            }
        }

        public static void Log(Exception ex)
        {
            if (Initialized)
                Log(ex, null);
        }

        public static void Log(Exception ex, dynamic dadosParaLogar)
        {
            if (Initialized)
                Task.Run(() => LogAsync(ex, dadosParaLogar)).Wait();
        }

        public static void Log(LogObject log)
        {
            if (Initialized)
                Task.Run(() => LogAsync(log)).Wait();
        }

        public static void LogError(string errorMessage, dynamic dadosParaLogar = null)
        {
            if (Initialized)
                Task.Run(() => LogAsync(errorMessage, ELogType.Error, dadosParaLogar)).Wait();
        }

        public static void LogError(string errorMessage, string source, dynamic dadosParaLogar = null)
        {
            if (Initialized)
                Task.Run(() => LogAsync(errorMessage, ELogType.Error, source, dadosParaLogar)).Wait();
        }

        private static void FillDynamicParameters(dynamic parameters, LogObject log)
        {
            if (parameters != null)
            {
                var properties = parameters.GetType().DeclaredProperties as System.Reflection.PropertyInfo[];

                foreach (var property in properties)
                {
                    log.AddParameter(property.Name, property.GetValue(parameters));
                }
            }
        }

        private static LogObject CreateLogObject(string message, ELogType logType, string source, dynamic parameters)
        {
            var log = new LogObject(message, logType, source);
            FillDynamicParameters(parameters, log);
            return log;
        }

        public static LogObject CreateEvidenceLogObject(string message, dynamic parameters)
        {
            return CreateLogObject(message, ELogType.Evidence, DefaultSourceName, parameters);
        }
    }
}
