using Carter;
using MongoDB.Entities;

namespace Todo.Api.Features.Todos.ClearCompletedTodos
{
    public class ClearCompletedTodosEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/todos").WithTags("Todos");

            group.MapDelete("/completed", Handle)
                .WithName("ClearCompleted")
                .WithSummary("Clear completed todos")
                .WithDescription("Deletes all completed todos.")
                .Produces(StatusCodes.Status204NoContent);
        }

        private static async Task<IResult> Handle()
        {
            var completedTodo = await DB.Find<TodoItem>()
                .Match(x => x.IsCompleted)
                .ExecuteAsync();

            foreach (var todo in completedTodo)
            {
                await todo.DeleteAsync();
            }

            return Results.NoContent();
        }
    }
}
