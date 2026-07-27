using AutoMapper;
using Carter;
using FluentValidation;
using MongoDB.Entities;
using Todo.Api.Common.Validation;

namespace Todo.Api.Features.Todos.UpdateTodo
{
    public class UpdateTodoEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/todos").WithTags("Todos");

            group.MapPut("/{id}", Handle)
                .AddEndpointFilter<ValidationFilter<UpdateTodoRequest>>()
                .WithName("UpdateTodo")
                .WithSummary("Update todo")
                .WithDescription("Updates an existing todo.")
                .Produces<TodoDto>(StatusCodes.Status200OK)
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> Handle(string id, UpdateTodoRequest request, IValidator<UpdateTodoRequest> validator, IMapper mapper)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var todo = await DB.Find<TodoItem>()
                .OneAsync(id);

            if (todo is null)
            {
                return Results.Problem(
                    title: "Todo not found",
                    detail: $"To do with id `{id}` was not found.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            mapper.Map(request, todo);

            await todo.SaveAsync();

            var response = mapper.Map<TodoDto>(todo);

            return Results.Ok(response);
        }
    }
}
