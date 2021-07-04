using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using BusyLight.Core;

namespace BusyLight.ActivityLoggingService {
    public partial class Service1 : ServiceBase {
        public Service1() {
            InitializeComponent();
        }

        static readonly IMicrophoneStatusChecker MicrophoneStatusChecker = new MicrophoneRegistryStatusChecker();
        static readonly ActivityLogger ActivityLogger = new ActivityLogger(ConfigurationManager.AppSettings["DatabaseApiKey"]);
        Thread _thread;

        protected override void OnStart(string[] args) {
            _thread = new Thread(DoWork) {IsBackground = true};
            _thread.Start();
        }

        void DoWork() {
            while (true) {
                try {
                    var microphoneIsBeingUsed = MicrophoneStatusChecker.IsMicrophoneBeingUsed();
                    if (microphoneIsBeingUsed) {
                        ActivityLogger.LogMicrophoneUse();
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        protected override void OnStop() {
            _thread.Abort();
        }

        internal void TestStartupAndStop()
        {
            OnStart(null);
            Console.ReadKey();
            OnStop();
        }
    }
}
