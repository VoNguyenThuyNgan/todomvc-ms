using Carter;
using Todo.Bff.Clients.Todos;
using Todo.Bff.Extensions;

namespace Todo.Bff.Features.Todos.ToggleTodo
{
    public class ToggleTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/bff/todos")
                           .WithTags("Todos");

            group.MapPatch("/{id}/toggle", Handle)
                .WithName("BffToggleTodo")
                .WithSummary("Toggle todo completion")
                .WithDescription("Proxy request to Todo.Api to toggle the completion status of a todo.")
                .Produces<TodoDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> Handle(string id, ITodoApiClient client)
        {
            var response = await client.ToggleTodoAsync(id);

            return await response.ToResultAsync();
        }
    }
}
