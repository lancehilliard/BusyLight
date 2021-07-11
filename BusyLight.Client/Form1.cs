using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows.Forms;
using BlinkStickDotNet;
using BusyLight.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Constants = BusyLight.Core.Constants;

namespace BusyLight.Client {
    public partial class Form1 : Form {
        static readonly int PublishIntervalSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["PublishIntervalSeconds"]);
        static readonly IMicrophoneStatusChecker MicrophoneStatusChecker = new MicrophoneRegistryStatusChecker();
        static readonly Core.ActivityPublisher ActivityPublisher = new Core.ActivityPublisher(new ConnectionFactory {Uri = new Uri(ConfigurationManager.AppSettings["MessageQueueUrl"])});
        static readonly MicrophoneActivityPublisher MicrophoneActivityPublisher = new MicrophoneActivityPublisher(MicrophoneStatusChecker, ActivityPublisher);
        readonly Thread _publishingThread;
        Thread _subscribingThread;
        Thread _deviceThread;
        static readonly int AssumeMaxSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["AssumeMaxSeconds"]);
        static readonly ILightDevice LightDevice = new SquareLightDevice(BlinkStick.FindFirst());
        static readonly DeviceChangerFactory DeviceChangerFactory = new(LightDevice, ConfigurationManager.AppSettings["ActiveColor"], AssumeMaxSeconds);
        static readonly ConnectionFactory ConnectionFactory = new() {Uri = new Uri(ConfigurationManager.AppSettings["MessageQueueUrl"])};
        static readonly ManualResetEvent ResetEvent = new(false);
        DateTime _lastMessageWhenUtc;


        public Form1() {
            InitializeComponent();
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            _publishingThread = new Thread(DoPublishingWork) {IsBackground = true};
            _publishingThread.Start();
            _subscribingThread = new Thread(DoSubscribingWork) {IsBackground = true};
            _subscribingThread.Start();
            _deviceThread = new Thread(DoDeviceWork) {IsBackground = true};
            _deviceThread.Start();
        }

        protected override void OnClosing(CancelEventArgs e) {
            WindowState = FormWindowState.Minimized;
            e.Cancel = true;
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Function runs in a background thread and is intended to run infinitely.")]
        void DoDeviceWork() {
            while (true) {
                var timeSinceLastMessage = DateTime.UtcNow - _lastMessageWhenUtc;
                var deviceChanger = DeviceChangerFactory.Create(timeSinceLastMessage);
                deviceChanger.Change();
                Thread.Sleep(250);
            }
        }

        void DoSubscribingWork() {
            using var connection = ConnectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(Constants.QueueName, durable: false, exclusive: false, autoDelete: true, null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, deliveryEventArgs) => {
                //var body = deliveryEventArgs.Body.ToArray();
                //var message = Encoding.UTF8.GetString(body);
                _lastMessageWhenUtc = DateTime.UtcNow > _lastMessageWhenUtc ? DateTime.UtcNow : _lastMessageWhenUtc;
                // ReSharper disable once AccessToDisposedClosure
                channel.BasicAck(deliveryEventArgs.DeliveryTag, false);
            };
            channel.BasicConsume(consumer, Constants.QueueName);
            ResetEvent.WaitOne();
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Function runs in a background thread and is intended to run infinitely.")]
        void DoPublishingWork() {
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

        void quitToolStripMenuItem_Click(object sender, EventArgs e) {
            _publishingThread.Abort();
            _subscribingThread?.Abort();
            _deviceThread?.Abort();
            LightDevice?.Dispose();
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
