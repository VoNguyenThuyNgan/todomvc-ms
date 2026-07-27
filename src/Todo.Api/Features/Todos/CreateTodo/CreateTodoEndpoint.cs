using AutoMapper;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MongoDB.Entities;
using Todo.Api.Common.Validation;

namespace Todo.Api.Features.Todos.CreateTodo
{
    public class CreateTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/todos").WithTags("Todos");

            group.MapPost("/", Handle)
                .AddEndpointFilter<ValidationFilter<CreateTodoRequest>>()
                .WithName("CreateTodo")
                .WithSummary("Create todo")
                .WithDescription("Creates a new todo.")
                .Produces<TodoDto>(StatusCodes.Status201Created)
                .ProducesValidationProblem();
        }

        private static async Task<IResult> Handle(CreateTodoRequest request, IMapper mapper)
        {
            var todo = mapper.Map<TodoItem>(request);
            todo.CreateAt = DateTime.UtcNow;
            todo.IsCompleted = false;

            await todo.SaveAsync();

            var response = mapper.Map<TodoDto>(todo);

            return Results.CreatedAtRoute(
                "GetTodoById",
                new { id = todo.ID },
                response);
        }
    }
}
