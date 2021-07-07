using System;
using System.Globalization;
using System.Text;
using RabbitMQ.Client;

namespace BusyLight.Core {
    public interface IActivityPublisher {
        void PublishMicrophoneUse();
    }

    public class ActivityPublisher : IActivityPublisher {
        readonly ConnectionFactory _connectionFactory;
        public ActivityPublisher(ConnectionFactory connectionFactory) {
            _connectionFactory = connectionFactory;
        }

        public void PublishMicrophoneUse() {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Type = "microphone";
            channel.QueueDeclare(Constants.QueueName, durable: false, exclusive: false, autoDelete: true, null);
            var message = string.Empty;
            var data = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(string.Empty, Constants.QueueName, basicProperties, data);
        }
    }
}