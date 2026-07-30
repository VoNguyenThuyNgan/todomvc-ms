using System.Text.Json;
using Todo.Bff.Clients.Reminders;
using Todo.Bff.Features.Reminders;

namespace Todo.Bff.Services;

public class ReminderEventService
{
    private readonly IReminderApiClient _client;
    private readonly HashSet<string> _knownReminderIds = [];

    public ReminderEventService(IReminderApiClient client)
    {
        _client = client;
    }

    public async Task<List<ReminderDto>> GetNewRemindersAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetRemindersAsync(ReminderState.Pending);
        response.EnsureSuccessStatusCode();

        var reminders =
            await response.Content.ReadFromJsonAsync<List<ReminderDto>>(
                cancellationToken: cancellationToken)
            ?? [];

        var newReminders = reminders
            .Where(x => !_knownReminderIds.Contains(x.Id))
            .ToList();

        foreach (var reminder in newReminders)
        {
            _knownReminderIds.Add(reminder.Id);
        }

        return newReminders;
    }

    public async Task WriteEventAsync(HttpResponse response, ReminderDto reminder, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(reminder);

        await response.WriteAsync(
                $"""
                event: reminder-fired
                data: {json}

                """,
            cancellationToken);

        await response.Body.FlushAsync(cancellationToken);
    }

    public async Task WriteHeartbeatAsync(HttpResponse response,CancellationToken cancellationToken)
    {
        await response.WriteAsync(
                """
                event: heartbeat
                data: connected

                """,
            cancellationToken);

        await response.Body.FlushAsync(cancellationToken);
    }
}