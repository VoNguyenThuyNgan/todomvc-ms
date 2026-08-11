using System.Threading.Channels;
using Todo.Bff.Features.Reminders;

namespace Todo.Bff.Services;

public interface IReminderEventStream
{
    void Subscribe(Channel<ReminderDto> channel);

    void Unsubscribe(Channel<ReminderDto> channel);

    Task PublishAsync(
        ReminderDto reminder,
        CancellationToken cancellationToken = default);
}