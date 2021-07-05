using System;
using System.ServiceProcess;
using System.Threading;
using BlinkStickDotNet;
using BusyLight.Core;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace BusyLight.LightService {
    public partial class Service1 : ServiceBase {
        Thread _thread;

        public Service1() {
            InitializeComponent();
        }

        static readonly DeviceChangerFactory DeviceChangerFactory = new DeviceChangerFactory(new ActivityChecker(ConfigurationManager.AppSettings["RestDatabaseApiKey"], ConfigurationManager.AppSettings["RestBaseUrl"]));
        static readonly ILightDevice LightDevice = new SquareLightDevice(BlinkStick.FindFirst());

        protected override void OnStart(string[] args) {
            _thread = new Thread(DoWork) {IsBackground = true};
            _thread.Start();
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Function runs in a background thread and is intended to run infinitely.")]
        void DoWork() {
            while (true) {
                try {
                    var deviceChanger = DeviceChangerFactory.Create(LightDevice);
                    deviceChanger.Change();
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