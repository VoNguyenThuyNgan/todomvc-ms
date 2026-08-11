namespace Todo.Api.Services
{
    public interface IServiceBusPublisher
    {
        Task PublishAsync<T>(T message, CancellationToken cancellationToken = default);
    }
}
