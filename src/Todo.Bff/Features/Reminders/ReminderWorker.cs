using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Todo.Bff.Common.Configuration;
using Todo.Bff.Services;

namespace Todo.Bff.Features.Reminders;

public class ReminderWorker : BackgroundService
{
    private readonly ILogger<ReminderWorker> _logger;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusOptions _options;
    private readonly IReminderEventStream _eventStream;

    public ReminderWorker(
        ILogger<ReminderWorker> logger,
        ServiceBusClient client,
        IOptions<ServiceBusOptions> options,
        IReminderEventStream eventStream
        )
    {
        _logger = logger;
        _client = client;
        _options = options.Value;
        _eventStream = eventStream;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Reminder Worker started.");

        await using var processor =
            _client.CreateProcessor(
                _options.TopicName,
                _options.SubscriptionName);

        processor.ProcessMessageAsync += OnMessageReceived;
        processor.ProcessErrorAsync += OnError;

        await processor.StartProcessingAsync(stoppingToken);

        try
        {
            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Application is shutting down.
        }

        await processor.StopProcessingAsync();
    }

    private async Task OnMessageReceived(
        ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();

        var reminder =
            JsonSerializer.Deserialize<ReminderDto>(body);

        if (reminder is null)
        {
            _logger.LogWarning(
                "Could not deserialize reminder event.");

            return;
        }

        _logger.LogInformation(
            "Received reminder {ReminderId} from ASB. DeliveryCount={DeliveryCount}, MessageId={MessageId}",
            reminder.Id, args.Message.DeliveryCount, args.Message.MessageId);

        await _eventStream.PublishAsync(reminder);
        await args.CompleteMessageAsync(args.Message);

        await Task.CompletedTask;
    }

    private Task OnError(
        ProcessErrorEventArgs args)
    {
        _logger.LogError(
            args.Exception,
            "Error while processing Service Bus message. Entity: {EntityPath}",
            args.EntityPath);

        return Task.CompletedTask;
    }
}