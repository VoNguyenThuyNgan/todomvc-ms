
using Todo.Bff.Features.Reminders;
using Todo.Bff.Features.Todos;

namespace Todo.Bff.Clients
{
    public interface ITodoApiClient
    {
        // Todos
        Task<HttpResponseMessage> GetTodosAsync(TodoFilter? filter);
        Task<HttpResponseMessage> GetTodoByIdAsync(string id);
        Task<HttpResponseMessage> CreateTodoAsync(CreateTodoRequest request);
        Task<HttpResponseMessage> UpdateTodoAsync(string id, UpdateTodoRequest request);
        Task<HttpResponseMessage> ToggleTodoAsync(string id);
        Task<HttpResponseMessage> DeleteTodoAsync(string id);
        Task<HttpResponseMessage> ClearCompletedAsync();
        Task<HttpResponseMessage> ToggleAllTodosAsync(ToggleAllTodosRequest request);

        // Reminders
        Task<HttpResponseMessage> GetRemindersAsync(ReminderState? state);
        Task<HttpResponseMessage> GetUpcomingRemindersAsync(string? within);
        Task<HttpResponseMessage> SnoozeReminderAsync(string id, SnoozeReminderRequest request);
        Task<HttpResponseMessage> DismissReminderAsync(string id);
    }
}
