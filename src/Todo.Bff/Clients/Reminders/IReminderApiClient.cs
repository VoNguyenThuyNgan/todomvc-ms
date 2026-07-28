using Todo.Bff.Features.Reminders;
using Todo.Bff.Features.Reminders.SnoozeReminder;

namespace Todo.Bff.Clients.Reminders
{
    public interface IReminderApiClient
    {
        Task<HttpResponseMessage> GetRemindersAsync(ReminderState? state);

        Task<HttpResponseMessage> GetUpcomingRemindersAsync(string? within);

        Task<HttpResponseMessage> SnoozeReminderAsync(string id, SnoozeReminderRequest request);

        Task<HttpResponseMessage> DismissReminderAsync(string id);
    }
}
