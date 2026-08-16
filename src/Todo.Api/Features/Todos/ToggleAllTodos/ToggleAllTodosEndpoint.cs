using Carter;
using MongoDB.Entities;

namespace Todo.Api.Features.Todos.ToggleAllTodos
{
    public class ToggleAllTodosEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/todos").WithTags("Todos");

            group.MapPatch("/toggle-all", Handle)
                .WithName("ToggleAllTodos")
                .WithSummary("Toggle all todos")
                .WithDescription("Mark all todos as completed or active.");
        }

        private static async Task<IResult> Handle(ToggleAllTodosRequest request)
        {
            var todos = await DB.Find<TodoItem>()
                .ExecuteAsync();

            DateTime? completedAt = request.IsCompleted
                ? DateTime.UtcNow
                : null;

            foreach (var todo in todos)
            {
                if (request.IsCompleted)
                {
                    if (!todo.IsCompleted)
                    {
                        todo.IsCompleted = true;
                        todo.CompletedAt = completedAt;
                    }
                }
                else
                {
                    todo.IsCompleted = false;
                    todo.CompletedAt = null;
                }
                await todo.SaveAsync();
            }

            return Results.NoContent();
        } 
    }
}
