using AutoMapper;
using Carter;
using FluentValidation;
using MediatR;
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
                .WithName("UpdateTodo")
                .WithSummary("Update todo")
                .WithDescription("Updates an existing todo.")
                .Produces<TodoDto>(StatusCodes.Status200OK)
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> Handle(string id, UpdateTodoRequest request, ISender sender, CancellationToken cancellationToken)
        {
            var command = new UpdateTodoCommand(
           id,
           request.Title,
           request.IsCompleted,
           request.DueAt);

            var todo = await sender.Send(
                command,
                cancellationToken);

            if (todo is null)
            {
                return Results.Problem(
                    title: "Todo not found",
                    detail: $"To do with id `{id}` was not found.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            return Results.Ok(todo);
        }
    }
}
