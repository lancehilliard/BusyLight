using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using BusyLight.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Constants = BusyLight.Core.Constants;

namespace BusyLight.Client {
    public partial class Form1 : Form {
        static readonly int LogDisplayCharacterLength = 25000;
        static readonly Color OffColor = Color.FromArgb(128, 119, 136, 153);

        readonly IConfig _config;
        readonly ILogger _mainLogger;
        readonly ILogger _sendLogger;
        readonly ILogger _receiveLogger;
        readonly Thread _startThread;
        readonly Thread _publishingThread;
        readonly Thread _subscribingThread;
        readonly Thread _deviceThread;
        readonly ILightDevice _lightDevice;
        readonly MicrophoneActivityPublisher _microphoneActivityPublisher;
        readonly DeviceChangerFactory _deviceChangerFactory;
        readonly ConnectionFactory _connectionFactory;
        readonly ManualResetEvent _resetEvent;

        DateTime _lastMessageWhenUtc;
        DateTime _lastDeviceStateLogShownUtc;
        DeviceState _lastDeviceState;
        Icon _icon;
        readonly bool _willReceive;

        public Form1() {
            InitializeComponent();
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            _config = new EnvironmentVariablesConfig();
            _connectionFactory = new();
            _resetEvent = new(false);
            _startThread = new Thread(DoStartWork) {IsBackground = true};
            _publishingThread = new Thread(DoPublishingWork) {IsBackground = true};
            _subscribingThread = new Thread(DoSubscribingWork) {IsBackground = true};
            _deviceThread = new Thread(DoDeviceWork) {IsBackground = true};
            _mainLogger = new ActionLogger(s => MainTextBoxText = $"{DateTime.Now}: {s}{Environment.NewLine}{MainTextBoxText}");
            _sendLogger = new ActionLogger(s => SendTextBoxText = $"{DateTime.Now}: {s}{Environment.NewLine}{SendTextBoxText}");
            _receiveLogger = new ActionLogger(s => ReceiveTextBoxText = $"{DateTime.Now}: {s}{Environment.NewLine}{ReceiveTextBoxText}");
            _lightDevice =  new SquareLightDevice(_mainLogger);
            _deviceChangerFactory = new DeviceChangerFactory(_lightDevice, _config);
            var activityPublisher = new ActivityPublisher(_connectionFactory, _sendLogger);
            _microphoneActivityPublisher = new MicrophoneActivityPublisher(new MicrophoneRegistryStatusChecker(), activityPublisher, _sendLogger);
            _willReceive = _lightDevice.IsReady();
            _icon = GetIcon(OffColor);
            _startThread.Start();
        }

        void DoStartWork() {
            var messagingConnectionSuccessful = false;
            if (string.IsNullOrWhiteSpace(_config.MessageQueueUrl)) {
                _mainLogger.Log($"{nameof(_config.MessageQueueUrl)} not defined. Correct and Quit/Restart.");
            }
            else {
                try {
                    _connectionFactory.Uri = new Uri(_config.MessageQueueUrl);
                    using var connection = _connectionFactory.CreateConnection();
                    messagingConnectionSuccessful = true;
                }
                catch (Exception e) {
                    _mainLogger.Log(e.StackTrace);
                    _mainLogger.Log(e.Message);
                    if (e.InnerException != null) {
                        _mainLogger.Log(e.InnerException.StackTrace);
                        _mainLogger.Log(e.InnerException.Message);
                    }
                    _mainLogger.Log($"{nameof(_config.MessageQueueUrl)}: '{_config.MessageQueueUrl}'");
                    _mainLogger.Log($"AMQP connect failed. Correct your config and Quit/Restart.");
                }
            }
            if (messagingConnectionSuccessful) {
                _publishingThread.Start();
                _subscribingThread.Start();
                _deviceThread.Start();
            }
            else {
                UpdateIcons(OffColor, "Error");
            }
        }

        protected override void OnClosing(CancelEventArgs e) {
            Hide();
            e.Cancel = true;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool DestroyIcon(IntPtr handle);

        [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Function runs in a background thread and is intended to run infinitely.")]
        void DoDeviceWork() {
            PerformLightTest();
            if (_willReceive) {
                _mainLogger.Log("BlinkStick detected. This machine will send and receive activity.");
                string lastNotifyIconText = null;
                while (true) {
                    var timeSinceLastMessage = DateTime.UtcNow - _lastMessageWhenUtc;
                    var deviceChanger = _deviceChangerFactory.Create(timeSinceLastMessage);
                    var deviceState = deviceChanger.Change();
                    var deviceIsOn = deviceState is DeviceState.On or DeviceState.SimulatedOn;
                    var timeSinceLastDeviceStateLogShown = DateTime.UtcNow - _lastDeviceStateLogShownUtc;
                    var color = deviceIsOn ? _config.ActiveColor : OffColor;
                    var onOffText = Enum.GetName(typeof(DeviceState), deviceState);
                    UpdateIcons(color, onOffText);
                    if (TimeSpan.FromSeconds(1) < timeSinceLastDeviceStateLogShown || deviceState != _lastDeviceState) {
                        if (notifyIcon1.Text != lastNotifyIconText) {
                            _mainLogger.Log(notifyIcon1.Text);
                        }
                        _lastDeviceStateLogShownUtc = DateTime.UtcNow;
                        lastNotifyIconText = notifyIcon1.Text;
                    }
                    _lastDeviceState = deviceState;
                    Thread.Sleep(250);
                }
            }
            // ReSharper disable once RedundantIfElseBlock
            else {
                notifyIcon1.Icon = _icon;
                AppIcon = _icon;
                _mainLogger.Log("BlinkStick absent. This machine will only send (not receive) activity.");
            }
        }

        void PerformLightTest() {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            var onDeviceChanger = new OnDeviceChanger(_lightDevice, _config);
            var offDeviceChanger = new OffDeviceChanger(_lightDevice);
            onDeviceChanger.Change();
            UpdateIcons(_config.ActiveColor, "Testing...");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            var offState = offDeviceChanger.Change();
            var offText = Enum.GetName(typeof(DeviceState), offState);
            UpdateIcons(OffColor, offText);
        }

        void UpdateIcons(Color color, string statusText) {
            colorPanel.BackColor = color;
            DestroyIcon(_icon.Handle); // https://stackoverflow.com/questions/12026664/a-generic-error-occurred-in-gdi-when-calling-bitmap-gethicon#comment23138558_12026812
            _icon = GetIcon(colorPanel.BackColor);
            notifyIcon1.Icon = _icon;
            AppIcon = _icon;
            notifyIcon1.Text = $@"BusyLight - {statusText}";
            LedLabelText = statusText;
        }

        Icon GetIcon(Color color) {
            var size = 16;
            var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(bitmap);
            var rect = new Rectangle(0, 0, size, size);
            var pen = new Pen(Color.Black, 2) {Alignment = PenAlignment.Inset};
            graphics.FillRectangle(new SolidBrush(color), rect);
            graphics.DrawRectangle(pen, rect);
            var result = Icon.FromHandle(bitmap.GetHicon());
            return result;
        }

        void DoSubscribingWork() {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            if (_willReceive) {
                try {
                    using var connection = _connectionFactory.CreateConnection();
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
                    _receiveLogger.Log("This log shows when activity is received at your AMQP instance.");
                    _resetEvent.WaitOne();
                }
                catch (Exception e) {
                    var messages = string.Join("; ", new List<string>{e.Message, e.InnerException?.Message}.Where(x=>x!=null));
                    _receiveLogger.Log($"Unable to subscribe ({messages}). Check your AMQP URL configuration, and then close and restart.");
                }
            }
            else {
                _receiveLogger.Log("BlinkStick absent this machine, but activity will be sent.");
            }
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Function runs in a background thread and is intended to run infinitely.")]
        void DoPublishingWork() {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            _sendLogger.Log("This log shows when activity is sent to your AMQP instance.");
            while (true) {
                var secondsToWaitBeforeNextCheck = 1;
                try {
                    var activityPublished = _microphoneActivityPublisher.PublishMicrophoneActivity();
                    if (activityPublished) {
                        secondsToWaitBeforeNextCheck = _config.PublishIntervalSeconds;
                    }
                    if (!_willReceive) {
                        _lastMessageWhenUtc = DateTime.UtcNow > _lastMessageWhenUtc ? DateTime.UtcNow : _lastMessageWhenUtc;
                        var color = activityPublished ? _config.ActiveColor : OffColor; 
                        var statusText = activityPublished ? "Sending..." : "Idle";
                        UpdateIcons(color, statusText);
                    }
                }
                catch (Exception e) {
                    var messages = string.Join("; ", new List<string>{e.Message, e.InnerException?.Message}.Where(x=>x!=null));
                    _mainLogger.Log($"Unable to send activity. Error: {messages}");
                }
                Thread.Sleep(TimeSpan.FromSeconds(secondsToWaitBeforeNextCheck));
            }
        }

        string MainTextBoxText {
            get {
                string result = null;
                mainTextBox.Invoke(new MethodInvoker(GetValue));
                return result;
                void GetValue() => result = mainTextBox.Text.Substring(0, mainTextBox.Text.Length > LogDisplayCharacterLength ? LogDisplayCharacterLength : mainTextBox.Text.Length);
            }
            set {
                mainTextBox.Invoke(new MethodInvoker(SetValue));
                void SetValue() => mainTextBox.Text = value;
            }
        }

        Icon AppIcon {
            set {
                Invoke(new MethodInvoker(SetValue));
                void SetValue() => Icon = value;
            }
        }

        string LedLabelText {
            set {
                ledLabel.Invoke(new MethodInvoker(SetValue));
                void SetValue() => ledLabel.Text = value;
            }
        }

        string SendTextBoxText {
            get {
                string result = null;
                sendTextBox.Invoke(new MethodInvoker(GetValue));
                return result;
                void GetValue() => result = sendTextBox.Text.Substring(0, sendTextBox.Text.Length > LogDisplayCharacterLength ? LogDisplayCharacterLength : sendTextBox.Text.Length);
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
                void GetValue() => result = receiveTextBox.Text.Substring(0, receiveTextBox.Text.Length > LogDisplayCharacterLength ? LogDisplayCharacterLength : receiveTextBox.Text.Length);
            }
            set {
                receiveTextBox.Invoke(new MethodInvoker(SetValue));
                void SetValue() => receiveTextBox.Text = value;
            }
        }

        void quitToolStripMenuItem_Click(object sender, EventArgs e) {
            new OffDeviceChanger(_lightDevice).Change();
            _deviceThread?.Abort();
            _subscribingThread?.Abort();
            _publishingThread?.Abort();
            _startThread?.Abort();
            _lightDevice?.Dispose();
            Application.Exit();
        }

        void openStripMenuItem_Click(object sender, EventArgs e) {
            ShowWindow();
        }

        void ShowWindow() {
            WindowState = FormWindowState.Normal;
            Show();
            Activate();
        }

        void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ShowWindow();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e) {
            Process.Start("https://github.com/lancehilliard/BusyLight");
        }
    }
}
