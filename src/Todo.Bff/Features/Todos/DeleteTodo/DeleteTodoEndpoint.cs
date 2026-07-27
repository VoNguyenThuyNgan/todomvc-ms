using Carter;
using Todo.Bff.Clients.Todos;
using Todo.Bff.Extensions;

namespace Todo.Bff.Features.Todos.DeleteTodo
{
    public class DeleteTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/bff/todos")
                           .WithTags("Todos");

            group.MapDelete("/{id}", Handle)
                .WithName("BffDeleteTodo")
                .WithSummary("Delete todo")
                .WithDescription("Proxy request to Todo.Api to delete a todo by id.")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> Handle(string id, ITodoApiClient client)
        {
            var response = await client.DeleteTodoAsync(id);

            return await response.ToResultAsync();
        }
    }
}
