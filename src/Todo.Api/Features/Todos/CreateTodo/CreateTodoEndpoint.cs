using AutoMapper;
using Carter;
using FluentValidation;
using MediatR;
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
                .WithName("CreateTodo")
                .WithSummary("Create todo")
                .WithDescription("Creates a new todo.")
                .Produces<TodoDto>(StatusCodes.Status201Created)
                .ProducesValidationProblem();
        }

        private static async Task<IResult> Handle(CreateTodoRequest request, ISender sender, CancellationToken cancellationToken)
        {
            var command = new CreateTodoCommand(
            request.Title,
            request.DueAt);

            var todo = await sender.Send(
                command,
                cancellationToken);

            return Results.CreatedAtRoute(
                "GetTodoById",
                new { id = todo.Id },
                todo);
        }
    }
}
