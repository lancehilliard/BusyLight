using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using BusyLight.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Constants = BusyLight.Core.Constants;

namespace BusyLight.Client {
    public partial class Form1 : Form {
        static readonly IMicrophoneStatusChecker MicrophoneStatusChecker = new MicrophoneRegistryStatusChecker();
        readonly MicrophoneActivityPublisher _microphoneActivityPublisher;
        readonly Thread _publishingThread;
        readonly Thread _subscribingThread;
        readonly Thread _deviceThread;
        readonly ILightDevice _lightDevice;
        readonly DeviceChangerFactory _deviceChangerFactory;
        static readonly IConfig Config = new EnvironmentVariablesConfig();
        static readonly ConnectionFactory ConnectionFactory = new() {Uri = new Uri(Config.MessageQueueUrl)};
        static readonly ManualResetEvent ResetEvent = new(false);
        readonly ILogger _mainLogger;
        readonly ILogger _sendLogger;
        readonly ILogger _receiveLogger;
        DateTime _lastMessageWhenUtc;
        DateTime _lastDeviceStateLogShownUtc;
        DeviceState _lastDeviceState;
        static readonly int LogDisplayLength = 25000;
        Icon icon;

        public Form1() {
            InitializeComponent();
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            _publishingThread = new Thread(DoPublishingWork) {IsBackground = true};
            _subscribingThread = new Thread(DoSubscribingWork) {IsBackground = true};
            _deviceThread = new Thread(DoDeviceWork) {IsBackground = true};
            _mainLogger = new ActionLogger(s => MainTextBoxText = $"{DateTime.Now}: {s}{Environment.NewLine}{MainTextBoxText}");
            _sendLogger = new ActionLogger(s => SendTextBoxText = $"{DateTime.Now}: {s}{Environment.NewLine}{SendTextBoxText}");
            _receiveLogger = new ActionLogger(s => ReceiveTextBoxText = $"{DateTime.Now}: {s}{Environment.NewLine}{ReceiveTextBoxText}");
            _lightDevice =  new SquareLightDevice(_mainLogger);
            _deviceChangerFactory = new DeviceChangerFactory(_lightDevice, Config);
            var activityPublisher = new ActivityPublisher(ConnectionFactory, _sendLogger);
            _microphoneActivityPublisher = new MicrophoneActivityPublisher(MicrophoneStatusChecker, activityPublisher, _sendLogger);
            _publishingThread.Start();
            _subscribingThread.Start();
            _deviceThread.Start();
            icon = GetIcon(colorPanel.BackColor);
        }

        protected override void OnClosing(CancelEventArgs e) {
            Hide();
            e.Cancel = true;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool DestroyIcon(IntPtr handle);

        [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Function runs in a background thread and is intended to run infinitely.")]
        void DoDeviceWork() {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            while (true) {
                var timeSinceLastMessage = DateTime.UtcNow - _lastMessageWhenUtc;
                var deviceChanger = _deviceChangerFactory.Create(timeSinceLastMessage);
                var deviceState = deviceChanger.Change();
                var deviceIsOn = deviceState is DeviceState.On or DeviceState.SimulatedOn;
                var timeSinceLastDeviceStateLogShown = DateTime.UtcNow - _lastDeviceStateLogShownUtc;
                colorPanel.BackColor = deviceIsOn ? Config.ActiveColor : Color.FromArgb(128, 119, 136, 153);
                var onOffText = Enum.GetName(typeof(DeviceState), deviceState);
                DestroyIcon(icon.Handle); // https://stackoverflow.com/questions/12026664/a-generic-error-occurred-in-gdi-when-calling-bitmap-gethicon#comment23138558_12026812
                icon = GetIcon(colorPanel.BackColor);
                notifyIcon1.Icon = icon;
                AppIcon = icon;
                notifyIcon1.Text = $@"BusyLight - {onOffText}";
                LedLabelText = $@"LED ({onOffText}): ";
                if (TimeSpan.FromSeconds(1) < timeSinceLastDeviceStateLogShown || deviceState != _lastDeviceState) {
                    _mainLogger.Log(notifyIcon1.Text);
                    _lastDeviceStateLogShownUtc = DateTime.UtcNow;
                }
                _lastDeviceState = deviceState;
                Thread.Sleep(250);
            }
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
            try {
                _receiveLogger.Log("This log shows when activity is received.");
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
                ResetEvent.WaitOne();
            }
            catch (Exception e) {
                _receiveLogger.Log($"Unable to subscribe ({e.Message}). Check your AMQP URL configuration, and then close and restart.");
            }
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Function runs in a background thread and is intended to run infinitely.")]
        void DoPublishingWork() {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            _sendLogger.Log("This log shows when activity is sent.");
            while (true) {
                var secondsToWaitBeforeNextCheck = 1;
                try {
                    var activityPublished = _microphoneActivityPublisher.PublishMicrophoneActivity();
                    if (activityPublished) {
                        secondsToWaitBeforeNextCheck = Config.PublishIntervalSeconds;
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
            _lightDevice?.Dispose();
            Application.Exit();
        }

        void openStripMenuItem_Click(object sender, EventArgs e) {
            ShowWindow();
        }

        void ShowWindow() {
            var activeColor = Config.ActiveColor;
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

        private void toolStripMenuItem1_Click(object sender, EventArgs e) {
            Process.Start("https://github.com/lancehilliard/BusyLight");
        }
    }
}
