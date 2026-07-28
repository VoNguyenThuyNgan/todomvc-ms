using Todo.Bff.Features.Reminders;
using Todo.Bff.Features.Todos;
using Todo.Bff.Features.Todos.GetTodos;
using Todo.Bff.Features.Todos.CreateTodo;
using Todo.Bff.Features.Todos.UpdateTodo;
using Todo.Bff.Features.Todos.ToggleAllTodos;

namespace Todo.Bff.Clients.Todos
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
        Task<HttpResponseMessage> ToggleAllTodosAsync(ToggleAllTodosRequest request);
    }
}