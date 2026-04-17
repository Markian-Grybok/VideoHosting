namespace FileService.Infrastructure.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string queueName, CancellationToken ct) where T : class;
    }
}
