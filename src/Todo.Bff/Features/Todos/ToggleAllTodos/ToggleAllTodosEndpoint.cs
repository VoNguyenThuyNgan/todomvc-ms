using Carter;
using Todo.Bff.Clients.Todos;
using Todo.Bff.Extensions;

namespace Todo.Bff.Features.Todos.ToggleAllTodos
{
    public class ToggleAllTodosEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/bff/todos")
                           .WithTags("Todos");

            group.MapPatch("/toggle-all", Handle)
                .WithName("BffToggleAllTodos")
                .WithSummary("Toggle all todos")
                .WithDescription("Proxy request to Todo.Api.")
                .Produces(StatusCodes.Status204NoContent);
        }

        private static async Task<IResult> Handle(ToggleAllTodosRequest request, ITodoApiClient client)
        {
            var response = await client.ToggleAllTodosAsync(request);

            return await response.ToResultAsync();
        }
    }
}
