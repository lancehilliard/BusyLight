using System;
using System.Text;
using RabbitMQ.Client;

namespace BusyLight.Core {
    public interface IActivityPublisher {
        void PublishMicrophoneUse();
    }

    public class ActivityPublisher : IActivityPublisher {
        readonly ConnectionFactory _connectionFactory;
        readonly ILogger _logger;
        public ActivityPublisher(ConnectionFactory connectionFactory, ILogger logger) {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public void PublishMicrophoneUse() {
            try {
                using var connection = _connectionFactory.CreateConnection();
                using var channel = connection.CreateModel();
                var basicProperties = channel.CreateBasicProperties();
                basicProperties.Type = "microphone";
                channel.QueueDeclare(Constants.QueueName, durable: false, exclusive: false, autoDelete: true, null);
                var message = string.Empty;
                var data = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(string.Empty, Constants.QueueName, basicProperties, data);
            }
            catch (Exception e) {
                _logger.Log($"Unable to publish ({e.Message}). Check your AMQP URL configuration, and then close and restart.");
            }
        }
    }
}