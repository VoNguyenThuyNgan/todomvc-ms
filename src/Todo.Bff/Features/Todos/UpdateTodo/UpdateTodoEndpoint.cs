using Carter;
using Todo.Bff.Clients.Todos;
using Todo.Bff.Extensions;

namespace Todo.Bff.Features.Todos.UpdateTodo
{
    public class UpdateTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/bff/todos")
                           .WithTags("Todos");

            group.MapPut("/{id}", Hanlde)
                .WithName("BffUpdateTodo")
                .WithSummary("Update todo")
                .WithDescription("Proxy request to Todo.Api to update an existing todo.")
                .Produces<TodoDto>(StatusCodes.Status200OK)
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> Hanlde(string id, UpdateTodoRequest request, ITodoApiClient client)
        {
            var response = await client.UpdateTodoAsync(id, request);

            return await response.ToResultAsync();
        }
    }
}
