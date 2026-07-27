using Carter;
using Todo.Bff.Clients.Todos;
using Todo.Bff.Extensions;


namespace Todo.Bff.Features.Todos.CreateTodo
{
    public class CreateTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/bff/todos")
                           .WithTags("Todos");

            group.MapPost("/", Handle)
                .WithName("BffCreateTodo")
                .WithSummary("Create todo")
                .WithDescription("Proxy request to Todo.Api to create a new todo.")
                .Produces<TodoDto>(StatusCodes.Status201Created)
                .ProducesValidationProblem();
        }

        private static async Task<IResult> Handle(CreateTodoRequest request, ITodoApiClient client)
        {
            var response = await client.CreateTodoAsync(request);

            return await response.ToResultAsync();
        }
    }
}