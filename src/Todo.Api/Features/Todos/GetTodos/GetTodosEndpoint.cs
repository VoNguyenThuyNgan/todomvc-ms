using AutoMapper;
using Carter;
using MongoDB.Entities;
namespace Todo.Api.Features.Todos.GetTodos
{
    public class GetTodosEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/todos").WithTags("Todos");

            group.MapGet("/", Handle)
                .WithName("GetTodos")
                .WithSummary("Get todos")
                .WithDescription("Get all todos or filter by status")
                .Produces<List<TodoDto>>(StatusCodes.Status200OK);
        }

        private static async Task<IResult> Handle(TodoFilter? filter, IMapper mapper)
        {
            filter ??= TodoFilter.All;

            List<TodoItem> todos;
            switch (filter)
            {
                case TodoFilter.Active:
                    todos = await DB.Find<TodoItem>()
                        .Match(x => !x.IsCompleted)
                        .ExecuteAsync();
                    break;

                case TodoFilter.Completed:
                    todos = await DB.Find<TodoItem>()
                        .Match(x => x.IsCompleted)
                        .ExecuteAsync();
                    break;

                default:
                    todos = await DB.Find<TodoItem>()
                        .ExecuteAsync();
                    break;
            }

            var response = mapper.Map<List<TodoDto>>(todos);
            return Results.Ok(response);
        }
    }
}
