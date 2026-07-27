using Carter;
using Todo.Bff.Clients.Todos;
using Todo.Bff.Extensions;

namespace Todo.Bff.Features.Todos.GetTodos
{
    public class GetTodosEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/bff/todos")
                           .WithTags("Todos");

            group.MapGet("/", Handle)
                .WithName("BffGetTodos")
                .WithSummary("Get todos")
                .WithDescription("Proxy request to Todo.Api to retrieve all todos or filter by status.")
                .Produces<List<TodoDto>>(StatusCodes.Status200OK);
        }


        private static async Task<IResult> Handle(TodoFilter? filter, ITodoApiClient client)
        {
            var response = await client.GetTodosAsync(filter);

            return await response.ToResultAsync();
        }
    }
}
