using System.Text.Json;
using Todo.Bff.Clients.Reminders;
using Todo.Bff.Features.Reminders;

namespace Todo.Bff.Services;

public class ReminderEventService
{
    private readonly IReminderApiClient _client;

    public ReminderEventService(IReminderApiClient client)
    {
        _client = client;
    }

    public async Task<List<ReminderDto>> GetNewRemindersAsync(HashSet<string> knownIds, CancellationToken cancellationToken = default)
    {
        var response = await _client.GetRemindersAsync(ReminderState.Pending);
        response.EnsureSuccessStatusCode();

        var reminders =
            await response.Content.ReadFromJsonAsync<List<ReminderDto>>(
                cancellationToken: cancellationToken)
            ?? [];

        var newReminders = reminders
            .Where(x => !knownIds.Contains(x.Id))
            .ToList();

        foreach (var reminder in newReminders)
        {
            knownIds.Add(reminder.Id);
        }

        Console.WriteLine($"Pending: {reminders.Count}");
        Console.WriteLine($"Known: {knownIds.Count}");
        Console.WriteLine($"New: {newReminders.Count}");

        return newReminders;
    }

    public async Task WriteEventAsync(HttpResponse response, ReminderDto reminder, CancellationToken cancellationToken)
    {
        Console.WriteLine($"SEND EVENT: {reminder.Id}");

        var json = JsonSerializer.Serialize(reminder, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync($"id: {reminder.Id}\n", cancellationToken);
        await response.WriteAsync("event: reminder-fired\n", cancellationToken);
        await response.WriteAsync($"data: {json}\n\n", cancellationToken);

        await response.Body.FlushAsync(cancellationToken);
    }

    public async Task WriteHeartbeatAsync(HttpResponse response,CancellationToken cancellationToken)
    {
        await response.WriteAsync("event: heartbeat\n", cancellationToken);
        await response.WriteAsync("data: connected\n\n", cancellationToken);

        await response.Body.FlushAsync(cancellationToken);
    }
}