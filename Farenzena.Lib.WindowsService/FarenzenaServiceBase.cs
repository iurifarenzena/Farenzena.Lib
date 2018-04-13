using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Farenzena.Lib.WindowsService
{
    public abstract partial class FarenzenaServiceBase : ServiceBase
    {
        public const int CMD_DO_SERVICE_JOB = 255;

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
                    AllocConsole();
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


        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int AllocConsole();

        private const int STD_OUTPUT_HANDLE = -11;
        private const int MY_CODE_PAGE = 437;
        private void EnableConsole()
        {
            if (Console.Title == string.Empty)
            {
                AllocConsole();
                IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
                Microsoft.Win32.SafeHandles.SafeFileHandle safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdHandle, true);
                FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
                StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
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
            StartWorkerThreadFunc(false);
        }

        private void StartWorkerThreadFunc(bool forced)
        {
            void Reset()
            {
                _thread = null;
                _timer.Start();
            }
         
            // If there is a thread, the job is already running
            if (_thread != null)
                return;

            try
            {
                _timer.Stop();
                _timer.Interval = GetJobInterval();

                _thread = new Thread(() =>
                {
                    DoServiceJob(forced);
                    Reset();
                });
                _thread.Name = "WorkerThreadFunc";
                _thread.IsBackground = true;
                _thread.Start();
            }
            catch (Exception e)
            {
                CriticalErrorLogger.LogCriticalError(e, ServiceName);
                //throw; REMOVED this throw here because it would cause the service to stop in the event of a failling execution
                Reset();
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

        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);

            if (command == CMD_DO_SERVICE_JOB)
                StartWorkerThreadFunc(true);
            else
                HandleNonDefaultCommand(command);
        }

        public abstract void ConfigureService();
        public abstract void ServiceStarting();
        public abstract void ServiceStopping();
        public abstract void DoServiceJob(bool forcedExecution);
        public abstract double GetFirstExecutionInterval();
        public abstract double GetJobInterval();
        public abstract void HandleNonDefaultCommand(int command);
    }
}
