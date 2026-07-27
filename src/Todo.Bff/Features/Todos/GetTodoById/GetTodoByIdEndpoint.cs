using Carter;
using Todo.Bff.Clients.Todos;
using Todo.Bff.Extensions;

namespace Todo.Bff.Features.Todos.GetTodoById
{
    public class GetTodoByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/bff/todos")
                           .WithTags("Todos");

            group.MapGet("/{id}", Handle)
                .WithName("BffGetTodoById")
                .WithSummary("Get todo by id")
                .WithDescription("Proxy request to Todo.Api to retrieve a todo by id.")
                .Produces<TodoDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> Handle(string id, ITodoApiClient client)
        {
            var response = await client.GetTodoByIdAsync(id);

            return await response.ToResultAsync();
        }
    }
}
