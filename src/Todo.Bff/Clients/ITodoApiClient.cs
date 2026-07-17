using Todo.Bff.DTOs;
using Todo.Bff.Enums;

namespace Todo.Bff.Clients
{
    public interface ITodoApiClient
    {
        Task<HttpResponseMessage> GetTodosAsync(TodoFilter? filter);
        Task<HttpResponseMessage> GetTodoByIdAsync(string id);
        Task<HttpResponseMessage> CreateTodoAsync(CreateTodoRequest request);
        Task<HttpResponseMessage> UpdateTodoAsync(string id, UpdateTodoRequest request);
        Task<HttpResponseMessage> ToggleTodoAsync(string id);
        Task<HttpResponseMessage> DeleteTodoAsync(string id);
        Task<HttpResponseMessage> ClearCompletedAsync();
    }
}
