using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
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
        readonly ILogger _mainLogger;
        readonly ILogger _sendLogger;
        readonly ILogger _receiveLogger;
        DateTime _lastMessageWhenUtc;
        DateTime _lastDeviceStateLogShownUtc;
        DeviceState _lastDeviceState;
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
            _mainLogger = new ActionLogger(s => MainTextBoxText = $"{DateTime.Now} {s}{Environment.NewLine}{MainTextBoxText}");
            _sendLogger = new ActionLogger(s => SendTextBoxText = $"{DateTime.Now} {s}{Environment.NewLine}{SendTextBoxText}");
            _receiveLogger = new ActionLogger(s => ReceiveTextBoxText = $"{DateTime.Now} {s}{Environment.NewLine}{ReceiveTextBoxText}");
            var activityPublisher = new ActivityPublisher(new ConnectionFactory {Uri = new Uri(ConfigurationManager.AppSettings["MessageQueueUrl"])}, _sendLogger);
            _microphoneActivityPublisher = new MicrophoneActivityPublisher(MicrophoneStatusChecker, activityPublisher, _sendLogger);
            _publishingThread.Start();
            _subscribingThread.Start();
            _deviceThread.Start();
            if (!LightDevice.IsReady()) {
                ledPanel.Hide();
            }
        }

        protected override void OnClosing(CancelEventArgs e) {
            //WindowState = FormWindowState.Minimized;
            Hide();
            e.Cancel = true;
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Function runs in a background thread and is intended to run infinitely.")]
        void DoDeviceWork() {
            if (LightDevice.IsReady()) {
                while (true) {
                    var timeSinceLastMessage = DateTime.UtcNow - _lastMessageWhenUtc;
                    var deviceChanger = _deviceChangerFactory.Create(timeSinceLastMessage);
                    var deviceState = deviceChanger.Change();
                    var deviceIsOn = deviceState == DeviceState.On;
                    notifyIcon1.Icon = deviceIsOn ? Properties.Resources.BusyLight_On : Properties.Resources.BusyLight_Off;
                    notifyIcon1.Text = $@"BusyLight - {(deviceIsOn ? "On" : "Off")}";
                    var timeSinceLastDeviceStateLogShown = DateTime.UtcNow - _lastDeviceStateLogShownUtc;
                    if (TimeSpan.FromSeconds(1) < timeSinceLastDeviceStateLogShown || deviceState != _lastDeviceState) {
                        _mainLogger.Log(notifyIcon1.Text);
                        _lastDeviceStateLogShownUtc = DateTime.UtcNow;
                    }
                    colorPanel.BackColor = deviceState == DeviceState.On ? _activeColorGetter.Get() : Color.Transparent;
                    _lastDeviceState = deviceState;
                    Thread.Sleep(250);
                }
            }
        }

        void DoSubscribingWork() {
            try {
                if (LightDevice.IsReady()) {
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
                        _receiveLogger.Log(Constants.ActivityReceiveMessage);
                    };
                    channel.BasicConsume(consumer, Constants.QueueName);
                    _receiveLogger.Log("This log shows when activity is received.");
                    ResetEvent.WaitOne();
                }
                else {
                    _receiveLogger.Log("No local BlinkStick detected.");
                }
            }
            catch (Exception e) {
                _receiveLogger.Log($"Unable to subscribe ({e.Message}). Check your AMQP URL configuration, and then close and restart.");
            }
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Function runs in a background thread and is intended to run infinitely.")]
        void DoPublishingWork() {
            _sendLogger.Log("This log shows when activity is sent.");
            while (true) {
                var secondsToWaitBeforeNextCheck = 1;
                try {
                    var activityPublished = _microphoneActivityPublisher.PublishMicrophoneActivity();
                    if (activityPublished) {
                        secondsToWaitBeforeNextCheck = PublishIntervalSeconds;
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
                Thread.Sleep(TimeSpan.FromSeconds(secondsToWaitBeforeNextCheck));
            }
        }

        string MainTextBoxText {
            get {
                string result = null;
                mainTextBox.Invoke(new MethodInvoker(GetValue));
                return result;
                void GetValue() => result = mainTextBox.Text.Substring(0, mainTextBox.Text.Length > LogDisplayLength ? LogDisplayLength : mainTextBox.Text.Length);
            }
            set {
                mainTextBox.Invoke(new MethodInvoker(SetValue));
                void SetValue() => mainTextBox.Text = value;
            }
        }

        string SendTextBoxText {
            get {
                string result = null;
                sendTextBox.Invoke(new MethodInvoker(GetValue));
                return result;
                void GetValue() => result = sendTextBox.Text.Substring(0, sendTextBox.Text.Length > LogDisplayLength ? LogDisplayLength : sendTextBox.Text.Length);
            }
            set {
                sendTextBox.Invoke(new MethodInvoker(SetValue));
                void SetValue() => sendTextBox.Text = value;
            }
        }

        string ReceiveTextBoxText {
            get {
                string result = null;
                receiveTextBox.Invoke(new MethodInvoker(GetValue));
                return result;
                void GetValue() => result = receiveTextBox.Text.Substring(0, receiveTextBox.Text.Length > LogDisplayLength ? LogDisplayLength : receiveTextBox.Text.Length);
            }
            set {
                receiveTextBox.Invoke(new MethodInvoker(SetValue));
                void SetValue() => receiveTextBox.Text = value;
            }
        }

        void quitToolStripMenuItem_Click(object sender, EventArgs e) {
            _publishingThread?.Abort();
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
