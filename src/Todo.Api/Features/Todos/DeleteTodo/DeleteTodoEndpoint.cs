using Carter;
using MongoDB.Entities;

namespace Todo.Api.Features.Todos.DeleteTodo
{
    public class DeleteTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/todos").WithTags("Todos");

            group.MapDelete("/{id}", Handle)
                .WithName("DeleteTodo")
                .WithSummary("Delete todo")
                .WithDescription("Deletes a todo by id.")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> Handle(string id)
        {
            var todo = await DB.Find<TodoItem>()
                .OneAsync(id);

            if (todo is null)
            {
                return Results.Problem(
                    title: "Todo not found",
                    detail: $"Todo with id '{id}' was not found.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            await todo.DeleteAsync();
            return Results.NoContent();
        }
    }
}
