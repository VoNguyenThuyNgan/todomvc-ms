using System.Threading.Channels;
using Todo.Bff.Features.Reminders;

namespace Todo.Bff.Services;

public class ReminderEventStream : IReminderEventStream
{
    private readonly List<Channel<ReminderDto>> _subscribers = [];

    private readonly object _lock = new();

    public void Subscribe(Channel<ReminderDto> channel)
    {
        lock (_lock)
        {
            _subscribers.Add(channel);
        }
    }

    public void Unsubscribe(Channel<ReminderDto> channel)
    {
        lock (_lock)
        {
            _subscribers.Remove(channel);
        }
    }

    public async Task PublishAsync(
        ReminderDto reminder,
        CancellationToken cancellationToken = default)
    {
        Channel<ReminderDto>[] subscribers;

        lock (_lock)
        {
            subscribers = _subscribers.ToArray();
        }

        foreach (var subscriber in subscribers)
        {
            await subscriber.Writer.WriteAsync(
                reminder,
                cancellationToken);
        }
    }
}