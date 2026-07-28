using Todo.Bff.Features.Reminders;
using Todo.Bff.Features.Todos.CreateTodo;
using Todo.Bff.Features.Todos.UpdateTodo;
using Todo.Bff.Features.Todos.ToggleAllTodos;
using Todo.Bff.Features.Todos.GetTodos;

namespace Todo.Bff.Clients.Todos
{
    public class TodoApiClient : ApiClientBase, ITodoApiClient
    {
        private readonly HttpClient _httpClient;
        
        public TodoApiClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        // Todos
        public Task<HttpResponseMessage> GetTodosAsync(TodoFilter? filter)
        {
            var url = "/api/todos";

            if (filter.HasValue)
            {
                url += $"?filter={filter.Value}";
            }

            return GetAsync(url);
        }

        public Task<HttpResponseMessage> GetTodoByIdAsync(string id)
        {
            return GetAsync($"/api/todos/{id}");
        }

        public Task<HttpResponseMessage> CreateTodoAsync(CreateTodoRequest request)
        {
            return PostAsync(
                "/api/todos",
                request);
        }

        public Task<HttpResponseMessage> UpdateTodoAsync(string id, UpdateTodoRequest request)
        {
            return PutAsync(
                $"/api/todos/{id}",
                request);
        }

        public Task<HttpResponseMessage> ToggleTodoAsync(string id)
        {
            return PatchAsync(
                $"/api/todos/{id}/toggle");
        }

        public Task<HttpResponseMessage> DeleteTodoAsync(string id)
        {
            return DeleteAsync($"/api/todos/{id}");
        }

        public Task<HttpResponseMessage> ClearCompletedAsync()
        {
            return DeleteAsync("/api/todos/completed");
        }
        public Task<HttpResponseMessage> ToggleAllTodosAsync(ToggleAllTodosRequest request)
        {
            return PatchAsync("/api/todos/toggle-all", request);
        }
    }
}
