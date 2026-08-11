using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Todo.Api.Common.Configuration;

namespace Todo.Api.Services
{
    public class ServiceBusPublisher : IServiceBusPublisher
    {
        private readonly ServiceBusSender _sender;

        public ServiceBusPublisher(ServiceBusClient client, IOptions<ServiceBusOptions> options)
        {
            _sender = client.CreateSender(options.Value.TopicName);
        }

        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(json)
            {
                ContentType = "application/json"
            };

            await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);
        }
    }
}
