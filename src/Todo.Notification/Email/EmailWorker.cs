using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Channels;
using Todo.Notification.Channels;
using Todo.Notification.Configuration;
using Todo.Notification.Contracts;

namespace Todo.Notification.Email;

public class EmailWorker : BackgroundService
{
    private readonly ILogger<EmailWorker> _logger;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusOptions _options;
    private readonly INotificationChannel _channel;

    public EmailWorker(
        ILogger<EmailWorker> logger,
        ServiceBusClient client,
        IOptions<ServiceBusOptions> options,
        INotificationChannel channel)
    {
        _logger = logger;
        _client = client;
        _options = options.Value;
        _channel = channel;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Email Worker started.");

        await using var processor =
            _client.CreateProcessor(
                _options.TopicName,
                _options.SubscriptionName);

        processor.ProcessMessageAsync += OnMessageReceived;
        processor.ProcessErrorAsync += OnError;

        await processor.StartProcessingAsync(
            stoppingToken);

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
            JsonSerializer.Deserialize<ReminderNotificationEvent>(
                body);

        if (reminder is null)
        {
            _logger.LogWarning(
                "Could not deserialize reminder event.");

            return;
        }

        _logger.LogInformation(
            "Processing reminder {ReminderId} through {Channel}",
            reminder.Id,
            _channel.ChannelName);

        await _channel.SendAsync(
            reminder,
            args.CancellationToken);

        await args.CompleteMessageAsync(args.Message);
    }

    private Task OnError(
        ProcessErrorEventArgs args)
    {
        _logger.LogError(
            args.Exception,
            "Error while processing email notification. Entity: {EntityPath}",
            args.EntityPath);

        return Task.CompletedTask;
    }
}