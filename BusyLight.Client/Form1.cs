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
        readonly MicrophoneActivityPublisher _microphoneActivityPublisher;
        readonly IActiveColorGetter _activeColorGetter;
        //readonly IActiveColorSetter _activeColorSetter;
        readonly Thread _publishingThread;
        readonly Thread _subscribingThread;
        readonly Thread _deviceThread;
        static readonly int AssumeMaxSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["AssumeMaxSeconds"]);
        static readonly ILightDevice LightDevice = new SquareLightDevice(BlinkStick.FindFirst());
        readonly DeviceChangerFactory _deviceChangerFactory;
        static readonly ConnectionFactory ConnectionFactory = new() {Uri = new Uri(ConfigurationManager.AppSettings["MessageQueueUrl"])};
        static readonly ManualResetEvent ResetEvent = new(false);
        readonly ILogger _logger;
        DateTime _lastMessageWhenUtc;
        static readonly int LogDisplayLength = 25000;

        public Form1() {
            InitializeComponent();
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            _publishingThread = new Thread(DoPublishingWork) {IsBackground = true};
            _subscribingThread = new Thread(DoSubscribingWork) {IsBackground = true};
            _deviceThread = new Thread(DoDeviceWork) {IsBackground = true};
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _activeColorGetter = new ActiveColorGetter(config);
            //var appSettingUpdater = new AppSettingUpdater(config);
            //_activeColorSetter = new ActiveColorSetter(appSettingUpdater);
            _deviceChangerFactory = new DeviceChangerFactory(LightDevice, _activeColorGetter, AssumeMaxSeconds);
            _logger = new ActionLogger(s => TextBoxText = $"{DateTime.Now} {s}{Environment.NewLine}{TextBoxText}");
            var activityPublisher = new ActivityPublisher(new ConnectionFactory {Uri = new Uri(ConfigurationManager.AppSettings["MessageQueueUrl"])}, _logger);
            _microphoneActivityPublisher = new MicrophoneActivityPublisher(MicrophoneStatusChecker, activityPublisher);
            _publishingThread.Start();
            _subscribingThread.Start();
            _deviceThread.Start();
        }

        protected override void OnClosing(CancelEventArgs e) {
            //WindowState = FormWindowState.Minimized;
            Hide();
            e.Cancel = true;
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Function runs in a background thread and is intended to run infinitely.")]
        void DoDeviceWork() {
            while (true) {
                var timeSinceLastMessage = DateTime.UtcNow - _lastMessageWhenUtc;
                var deviceChanger = _deviceChangerFactory.Create(timeSinceLastMessage);
                deviceChanger.Change();
                Thread.Sleep(250);
            }
        }

        void DoSubscribingWork() {
            try {
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
            catch (Exception e) {
                _logger.Log($"Unable to subscribe ({e.Message}). Check your AMQP URL configuration, and then close and restart.");
            }
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Function runs in a background thread and is intended to run infinitely.")]
        void DoPublishingWork() {
            while (true) {
                var secondsToWaitBeforeNextCheck = 1;
                try {
                    var activityPublished = _microphoneActivityPublisher.PublishMicrophoneActivity();
                    secondsToWaitBeforeNextCheck = activityPublished ? PublishIntervalSeconds : secondsToWaitBeforeNextCheck;
                    notifyIcon1.Icon = activityPublished ? Properties.Resources.BusyLight_On : Properties.Resources.BusyLight_Off;
                    notifyIcon1.Text = $@"BusyLight - {(activityPublished ? "On" : "Off")}";
                    _logger.Log(notifyIcon1.Text);
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
                Thread.Sleep(TimeSpan.FromSeconds(secondsToWaitBeforeNextCheck));
            }
        }

        string TextBoxText {
            get {
                string result = null;
                textBox1.Invoke(new MethodInvoker(GetValue));
                return result;
                void GetValue() => result = textBox1.Text.Substring(0, textBox1.Text.Length > LogDisplayLength ? LogDisplayLength : textBox1.Text.Length);
            }
            set {
                textBox1.Invoke(new MethodInvoker(SetValue));
                void SetValue() => textBox1.Text = value;
            }
        }

        void quitToolStripMenuItem_Click(object sender, EventArgs e) {
            _publishingThread.Abort();
            _subscribingThread?.Abort();
            _deviceThread?.Abort();
            LightDevice?.Dispose();
            Application.Exit();
        }

        void openStripMenuItem_Click(object sender, EventArgs e) {
            ShowWindow();
        }

        void ShowWindow() {
            var activeColor = _activeColorGetter.Get();
            colorDialog1.Color = activeColor;
            colorPanel.BackColor = activeColor;
            WindowState = FormWindowState.Normal;
            Show();
            Activate();
        }

        void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ShowWindow();
            }
        }

        //void colorPanel_Click(object sender, EventArgs e) {
        //    if (colorDialog1.ShowDialog() == DialogResult.OK) {
        //        _activeColorSetter.Set(colorDialog1.Color);
        //        colorPanel.BackColor = colorDialog1.Color;
        //    }
        //}
    }
}
