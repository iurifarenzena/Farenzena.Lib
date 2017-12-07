using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Farenzena.Lib.WindowsService
{
    public abstract partial class FarenzenaServiceBase : ServiceBase
    {
        public static void Run(FarenzenaServiceBase serviceToRun, params string[] args)
        {
            try
            {
                if (args.Length == 0)
                {

                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                        serviceToRun
                    };
                    ServiceBase.Run(ServicesToRun);
                }
                else
                {
                    serviceToRun.Interactive = true;
                    switch (args[0])
                    {
                        case "config":
                            serviceToRun.ConfigureService();
                            break;
                        case "run":
                            serviceToRun.OnStart(args);
                            Console.WriteLine("The service is running as an application. Press the ENTER key to exit.");
                            Console.ReadLine();
                            serviceToRun.Stop();
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
            catch (Exception e)
            {
                CriticalErrorLogger.LogCriticalError(e, serviceToRun.ServiceName);
                throw;
            }
        }

        private Thread _thread;
        private System.Timers.Timer _timer;
        protected bool Interactive { get; private set; }

        public FarenzenaServiceBase()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                ServiceStarting();

                this._timer = new System.Timers.Timer(GetFirstExecutionInterval());
                this._timer.AutoReset = true;
                this._timer.Elapsed += _timer_Elapsed;
                this._timer.Enabled = true;
            }
            catch (Exception e)
            {
                CriticalErrorLogger.LogCriticalError(e, ServiceName);
                throw;
            }
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();
            _timer.Interval = GetJobInterval();

            _thread = new Thread(WorkerThreadFunc);
            _thread.Name = "Liberardor de Tarefas";
            _thread.IsBackground = true;
            _thread.Start();
        }

        private void WorkerThreadFunc()
        {
            try
            {
                DoServiceJob(false);
            }
            catch (Exception e)
            {
                CriticalErrorLogger.LogCriticalError(e, ServiceName);
                throw;
            }
            finally
            {
                _timer.Start();
            }
        }

        protected override void OnStop()
        {
            try
            {
                _timer?.Stop();
                _timer?.Dispose();
                _thread?.Abort();
                ServiceStopping();
            }
            catch (Exception e)
            {
                CriticalErrorLogger.LogCriticalError(e, ServiceName);
            }
        }

        public abstract void ConfigureService();
        public abstract void ServiceStarting();
        public abstract void ServiceStopping();
        public abstract void DoServiceJob(bool forcedExecution);
        public abstract double GetFirstExecutionInterval();
        public abstract double GetJobInterval();
    }
}
