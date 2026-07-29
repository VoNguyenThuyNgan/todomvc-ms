using AutoMapper;
using Carter;
using MongoDB.Entities;

namespace Todo.Api.Features.Todos.ToggleTodo
{
    public class ToggleTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/todos").WithTags("Todos");

            group.MapPatch("/{id}/toggle", Hanlde)
                .WithName("ToggleTodo")
                .WithSummary("Toggle todo completion")
                .WithDescription("Toggle the completion status of a todo.")
                .Produces<TodoDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> Hanlde(string id, IMapper mapper)
        {
            var todo = await DB.Find<TodoItem>()
                .OneAsync(id);

            if (todo is null)
            {
                return Results.Problem(
                    title: "Todo not found",
                    detail: $"Todo with id `{id}` was not found",
                    statusCode: StatusCodes.Status404NotFound);
            }

            todo.IsCompleted = !todo.IsCompleted;
            await todo.SaveAsync();

            var response = mapper.Map<TodoDto>(todo);
            return Results.Ok(response);
        }
    }
}
