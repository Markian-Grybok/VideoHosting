using RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FileService.Infrastructure.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher, IAsyncDisposable
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqPublisher> _logger;

        public RabbitMqPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublisher> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task PublishAsync<T>(T message, string queueName, CancellationToken ct) where T : class
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _options.HostName,
                    Port = _options.Port,
                    UserName = _options.UserName,
                    Password = _options.Password
                };

                using (var connection = await factory.CreateConnectionAsync(cancellationToken: ct))
                using (var channel = await connection.CreateChannelAsync(cancellationToken: ct))
                {
                    var queueDeclareOk = await channel.QueueDeclareAsync(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null,
                        cancellationToken: ct);

                    var messageJson = JsonSerializer.Serialize(message);
                    var messageBytes = System.Text.Encoding.UTF8.GetBytes(messageJson);

                    var basicProperties = new BasicProperties
                    {
                        Persistent = true,
                        ContentType = "application/json"
                    };

                    await channel.BasicPublishAsync(
                        exchange: string.Empty,
                        routingKey: queueName,
                        mandatory: false,
                        basicProperties: basicProperties,
                        body: messageBytes,
                        cancellationToken: ct);

                    _logger.LogInformation($"Message published to queue: {queueName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error publishing message to queue: {queueName}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            await ValueTask.CompletedTask;
        }
    }
}
