using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.ServiceProcess;
using System.Threading;
using BusyLight.Core;
using RabbitMQ.Client;

namespace BusyLight.ActivityPublisher {
    public partial class Service1 : ServiceBase {
        public Service1() {
            InitializeComponent();
        }

        static readonly int PublishIntervalSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["PublishIntervalSeconds"]);
        static readonly IMicrophoneStatusChecker MicrophoneStatusChecker = new MicrophoneRegistryStatusChecker();
        static readonly Core.ActivityPublisher ActivityPublisher = new Core.ActivityPublisher(new ConnectionFactory {Uri = new Uri(ConfigurationManager.AppSettings["MessageQueueUrl"])});
        Thread _thread;

        protected override void OnStart(string[] args) {
            _thread = new Thread(DoWork) {IsBackground = true};
            _thread.Start();
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Function runs in a background thread and is intended to run infinitely.")]
        void DoWork() {
            while (true) {
                var secondsToWaitBeforeNextCheck = .25;
                try {
                    var microphoneIsBeingUsed = MicrophoneStatusChecker.IsMicrophoneBeingUsed();
                    if (microphoneIsBeingUsed) {
                        ActivityPublisher.PublishMicrophoneUse();
                        secondsToWaitBeforeNextCheck = PublishIntervalSeconds;
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
                Thread.Sleep(TimeSpan.FromSeconds(secondsToWaitBeforeNextCheck));
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
