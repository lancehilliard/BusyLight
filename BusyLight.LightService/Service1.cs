using System;
using System.ServiceProcess;
using System.Threading;
using BlinkStickDotNet;
using BusyLight.Core;
using System.Configuration;

namespace BusyLight.LightService {
    public partial class Service1 : ServiceBase {
        Thread _thread;

        public Service1() {
            InitializeComponent();
        }

        static readonly DeviceChangerFactory DeviceChangerFactory = new DeviceChangerFactory(new ActivityChecker(ConfigurationManager.AppSettings["DatabaseApiKey"]));

        protected override void OnStart(string[] args) {
            _thread = new Thread(DoWork) {IsBackground = true};
            _thread.Start();
        }

        void DoWork() {
            while (true) {
                try {
                    var device = BlinkStick.FindFirst();
                    var deviceChanger = DeviceChangerFactory.Create(device);
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