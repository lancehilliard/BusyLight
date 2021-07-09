using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows.Forms;
using BusyLight.Core;
using RabbitMQ.Client;

namespace BusyLight.Client {
    public partial class Form1 : Form {
        static readonly int PublishIntervalSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["PublishIntervalSeconds"]);
        static readonly IMicrophoneStatusChecker MicrophoneStatusChecker = new MicrophoneRegistryStatusChecker();
        static readonly Core.ActivityPublisher ActivityPublisher = new Core.ActivityPublisher(new ConnectionFactory {Uri = new Uri(ConfigurationManager.AppSettings["MessageQueueUrl"])});
        static readonly MicrophoneActivityPublisher MicrophoneActivityPublisher = new MicrophoneActivityPublisher(MicrophoneStatusChecker, ActivityPublisher);
        readonly Thread _thread;

        public Form1() {
            InitializeComponent();
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            _thread = new Thread(DoWork) {IsBackground = true};
            _thread.Start();
        }

        protected override void OnClosing(CancelEventArgs e) {
            WindowState = FormWindowState.Minimized;
            e.Cancel = true;
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Function runs in a background thread and is intended to run infinitely.")]
        void DoWork() {
            while (true) {
                var secondsToWaitBeforeNextCheck = .25;
                try {
                    var activityPublished = MicrophoneActivityPublisher.PublishMicrophoneActivity();
                    secondsToWaitBeforeNextCheck = activityPublished ? PublishIntervalSeconds : secondsToWaitBeforeNextCheck;
                    notifyIcon1.Icon = activityPublished ? Properties.Resources.BusyLight_On : Properties.Resources.BusyLight_Off;
                    notifyIcon1.Text = $@"BusyLight - {(activityPublished ? "On" : "Off")}";
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
                Thread.Sleep(TimeSpan.FromSeconds(secondsToWaitBeforeNextCheck));
            }
        }


        private void quitToolStripMenuItem_Click(object sender, EventArgs e) {
            _thread.Abort();
            Application.Exit();
        }

        private void openStripMenuItem_Click(object sender, EventArgs e) {
            WindowState = FormWindowState.Normal;
        }

        void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                WindowState = FormWindowState.Normal;
            }
        }
    }
}
