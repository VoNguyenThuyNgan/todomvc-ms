namespace Todo.Api.Common.Configuration
{
    public class ServiceBusOptions
    {
        public const string SectionName = "ServiceBus";
        public string ConnectionString { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
    }
}
