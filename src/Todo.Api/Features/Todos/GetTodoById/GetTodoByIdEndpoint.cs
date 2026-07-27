using AutoMapper;
using Carter;
using MongoDB.Entities;

namespace Todo.Api.Features.Todos.GetTodoById
{
    public class GetTodoByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/todos").WithTags("Todos");

            group.MapGet("/{id}", Handle)
                .WithName("GetTodoById")
                .WithSummary("Get todo by id")
                .WithDescription("Returns a todo by its id.")
                .Produces<TodoDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> Handle(string id, IMapper mapper)
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

            var response = mapper.Map<TodoDto>(todo);
            return Results.Ok(response);
        }
    }
}
