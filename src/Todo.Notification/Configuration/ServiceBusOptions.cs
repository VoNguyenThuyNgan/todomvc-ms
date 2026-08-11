namespace Todo.Notification.Configuration;

public class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";
    public string ConnectionString { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
}