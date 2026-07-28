using Todo.Bff.Features.Reminders;
using Todo.Bff.Features.Reminders.SnoozeReminder;
namespace Todo.Bff.Clients.Reminders
{
    public class ReminderApiClient : IReminderApiClient
    {
        private readonly HttpClient _httpClient;

        public ReminderApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("TodoApi");
        }

        public async Task<HttpResponseMessage> GetRemindersAsync(ReminderState? state)
        {
            var url = "/api/reminders";

            if (state.HasValue)
            {
                url += $"?state={state.Value}";
            }

            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> GetUpcomingRemindersAsync(string? within)
        {
            var url = "/api/reminders/upcoming";

            if (!string.IsNullOrWhiteSpace(within))
            {
                url += $"?within={within}";
            }

            return await _httpClient.GetAsync(url);
        }

        public async Task<HttpResponseMessage> SnoozeReminderAsync(string id, SnoozeReminderRequest request)
        {
            return await _httpClient.PatchAsJsonAsync(
                $"/api/reminders/{id}/snooze",
                request);
        }

        public async Task<HttpResponseMessage> DismissReminderAsync(string id)
        {
            return await _httpClient.PatchAsync(
                $"/api/reminders/{id}/dismiss",
                null);
        }
    }
}
