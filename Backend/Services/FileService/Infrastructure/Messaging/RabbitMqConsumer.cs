using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MediatR;
using FileService.Features.Processing.Commands;

namespace FileService.Infrastructure.Messaging
{
    public class RabbitMqConsumer : IHostedService, IAsyncDisposable
    {
        private readonly IOptions<RabbitMqOptions> _options;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RabbitMqConsumer> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private string _consumerTag = string.Empty;
        private CancellationTokenSource? _stoppingCts;

        public RabbitMqConsumer(IOptions<RabbitMqOptions> options, IServiceScopeFactory scopeFactory, ILogger<RabbitMqConsumer> logger)
        {
            _options = options;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _options.Value.HostName,
                    Port = _options.Value.Port,
                    UserName = _options.Value.UserName,
                    Password = _options.Value.Password
                };
                
                _connection = await factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
                
                await _channel.QueueDeclareAsync(
                    queue: _options.Value.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: cancellationToken);

                var consumer = new MessageConsumer(_channel, _scopeFactory, _logger, _stoppingCts);
                
                _consumerTag = await _channel.BasicConsumeAsync(
                    queue: _options.Value.QueueName,
                    autoAck: false,
                    consumerTag: string.Empty,
                    noLocal: false,
                    exclusive: false,
                    arguments: null,
                    consumer: consumer,
                    cancellationToken: cancellationToken);
                
                _logger.LogInformation("RabbitMQ consumer started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting RabbitMQ consumer");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _stoppingCts?.Cancel();
            
            try
            {
                if (_channel != null)
                {
                    if (!string.IsNullOrEmpty(_consumerTag))
                        await _channel.BasicCancelAsync(_consumerTag, false, cancellationToken);
                    await _channel.CloseAsync(cancellationToken);
                }

                if (_connection != null)
                    await _connection.CloseAsync(cancellationToken);

                _logger.LogInformation("RabbitMQ consumer stopped successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping RabbitMQ consumer");
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (_channel != null)
                    await _channel.DisposeAsync();
                
                if (_connection != null)
                    await _connection.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing RabbitMQ resources");
            }
            finally
            {
                _stoppingCts?.Dispose();
            }
        }

        private class MessageConsumer : IAsyncBasicConsumer
        {
            private readonly IChannel _channel;
            private readonly IServiceScopeFactory _scopeFactory;
            private readonly ILogger<RabbitMqConsumer> _logger;
            private readonly CancellationTokenSource _stoppingCts;

            public IChannel Channel => _channel;
            public string ConsumerTag { get; set; } = string.Empty;

            public MessageConsumer(IChannel channel, IServiceScopeFactory scopeFactory, ILogger<RabbitMqConsumer> logger, CancellationTokenSource stoppingCts)
            {
                _channel = channel;
                _scopeFactory = scopeFactory;
                _logger = logger;
                _stoppingCts = stoppingCts;
            }

            public Task HandleBasicCancelAsync(string consumerTag, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task HandleBasicCancelOkAsync(string consumerTag, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task HandleBasicConsumeOkAsync(string consumerTag, CancellationToken cancellationToken = default)
            {
                ConsumerTag = consumerTag;
                return Task.CompletedTask;
            }

            public async Task HandleBasicDeliverAsync(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IReadOnlyBasicProperties properties, ReadOnlyMemory<byte> body, CancellationToken cancellationToken = default)
            {
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                try
                {
                    var json = Encoding.UTF8.GetString(body.ToArray());
                    var evt = JsonSerializer.Deserialize<VideoUploadedEvent>(json);

                    if (evt != null)
                    {
                        var cmd = new ProcessVideoCommand(evt.FileId, evt.StoragePath);
                        await mediator.Send(cmd, _stoppingCts?.Token ?? CancellationToken.None);

                        await _channel.BasicAckAsync(deliveryTag, false, _stoppingCts?.Token ?? CancellationToken.None);

                        _logger.LogInformation($"Processed video upload event for file {evt.FileId}");
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize VideoUploadedEvent");
                        await _channel.BasicNackAsync(deliveryTag, false, false, _stoppingCts?.Token ?? CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing RabbitMQ message");
                    try
                    {
                        await _channel.BasicNackAsync(deliveryTag, false, false, _stoppingCts?.Token ?? CancellationToken.None);
                    }
                    catch
                    {
                        // Log and continue if Nack fails
                    }
                }
            }

            public Task HandleChannelShutdownAsync(object channel, ShutdownEventArgs reason)
            {
                _logger.LogWarning($"Channel shutdown: {reason?.ReplyText}");
                return Task.CompletedTask;
            }
        }
    }
}
