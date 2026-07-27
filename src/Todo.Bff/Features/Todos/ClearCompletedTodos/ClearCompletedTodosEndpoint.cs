using Carter;
using Todo.Bff.Clients.Todos;
using Todo.Bff.Extensions;

namespace Todo.Bff.Features.Todos.ClearCompleted
{
    public class ClearCompletedTodosEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/bff/todos")
                           .WithTags("Todos");

            group.MapDelete("/completed", Handle)
                .WithName("BffClearCompleted")
                .WithSummary("Clear completed todos")
                .WithDescription("Proxy request to Todo.Api to delete all completed todos.")
                .Produces(StatusCodes.Status204NoContent);
        }

        private static async Task<IResult> Handle(ITodoApiClient client)
        {
            var response = await client.ClearCompletedAsync();

            return await response.ToResultAsync();
        }
    }
}
