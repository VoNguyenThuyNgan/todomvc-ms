using Todo.Bff.DTOs;
using Todo.Bff.Enums;

namespace Todo.Bff.Clients
{
    public class TodoApiClient : ITodoApiClient
    {
        private readonly HttpClient _httpClient;
        
        public TodoApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetTodosAsync(TodoFilter? filter)
        {
            var url = "/api/todos";

            if (filter.HasValue)
            {
                url += $"?filter={filter.Value}";
            }

            return await _httpClient.GetAsync(url);
        }

        public async Task<HttpResponseMessage> GetTodoByIdAsync(string id)
        {
            return await _httpClient.GetAsync($"/api/todos/{id}");
        }

        public async Task<HttpResponseMessage> CreateTodoAsync(CreateTodoRequest request)
        {
            return await _httpClient.PostAsJsonAsync(
                "/api/todos",
                request);
        }

        public async Task<HttpResponseMessage> UpdateTodoAsync(string id, UpdateTodoRequest request)
        {
            return await _httpClient.PutAsJsonAsync(
                $"/api/todos/{id}",
                request);
        }

        public async Task<HttpResponseMessage> ToggleTodoAsync(string id)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Patch,
                $"/api/todos/{id}/toggle");

            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> DeleteTodoAsync(string id)
        {
            return await _httpClient.DeleteAsync(
                $"/api/todos/{id}");
        }

        public async Task<HttpResponseMessage> ClearCompletedAsync()
        {
            return await _httpClient.DeleteAsync(
                "/api/todos/completed");
        }
        public async Task<HttpResponseMessage> ToggleAllTodosAsync(ToggleAllTodosRequest request)
        {
            return await _httpClient.PatchAsJsonAsync(
                "/api/todos/toggle-all",
                request);
        }
    }
}
