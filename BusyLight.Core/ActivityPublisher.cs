using System;
using System.Globalization;
using System.Text;
using RabbitMQ.Client;

namespace BusyLight.Core {
    public class ActivityPublisher {
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
            var secondsSince1970Utc = (int)(DateTime.UtcNow - Constants.Utc1970).TotalSeconds;
            var data = Encoding.UTF8.GetBytes(secondsSince1970Utc.ToString(CultureInfo.InvariantCulture));
            channel.BasicPublish(string.Empty, Constants.QueueName, basicProperties, data);
        }
    }
}