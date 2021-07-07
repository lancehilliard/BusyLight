using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using BlinkStickDotNet;
using BusyLight.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Constants = BusyLight.Core.Constants;

namespace BusyLight.LightSubscriber {
    public partial class Service1 : ServiceBase {
        Thread _messageThread;
        Thread _lightThread;

        public Service1() {
            InitializeComponent();
        }

        static readonly int AssumeMaxSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["AssumeMaxSeconds"]);
        static readonly ILightDevice LightDevice = new SquareLightDevice(BlinkStick.FindFirst());
        static readonly DeviceChangerFactory DeviceChangerFactory = new(LightDevice, ConfigurationManager.AppSettings["ActiveColor"], AssumeMaxSeconds);
        static readonly ConnectionFactory ConnectionFactory = new() {Uri = new Uri(ConfigurationManager.AppSettings["MessageQueueUrl"])};
        static readonly ManualResetEvent ResetEvent = new(false);
        DateTime _lastMessageWhenUtc;

        protected override void OnStart(string[] args) {
            _messageThread = new Thread(DoMessagingWork) {IsBackground = true};
            _messageThread.Start();
            _lightThread = new Thread(DoDeviceWork) {IsBackground = true};
            _lightThread.Start();
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

        void DoMessagingWork() {
            using var connection = ConnectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(Constants.QueueName, durable: false, exclusive: false, autoDelete: true, null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, deliveryEventArgs) => {
                var body = deliveryEventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var secondsSince1970Utc = Convert.ToInt64(message);
                var whenUtc = Constants.Utc1970.AddSeconds(secondsSince1970Utc);
                var totalSeconds = (DateTime.UtcNow - whenUtc).TotalSeconds;
                if (Math.Abs(totalSeconds) < AssumeMaxSeconds) {
                    _lastMessageWhenUtc = DateTime.UtcNow;
                }
                // ReSharper disable once AccessToDisposedClosure
                channel.BasicAck(deliveryEventArgs.DeliveryTag, false);
            };
            channel.BasicConsume(consumer, Constants.QueueName);
            ResetEvent.WaitOne();
        }

        protected override void OnStop() {
            LightDevice?.Dispose();
            _messageThread?.Abort();
            _lightThread?.Abort();
        }

        internal void TestStartupAndStop()
        {
            OnStart(null);
            Console.ReadKey();
            OnStop();
        }
    }
}