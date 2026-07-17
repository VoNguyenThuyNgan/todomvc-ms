using AutoMapper;
using Carter;
using Todo.Api.DTOs;
using Todo.Api.Entities;
using Todo.Api.Enums;
using MongoDB.Entities;
using FluentValidation;

namespace Todo.Api.Modules
{
    public class TodoModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app
                .MapGroup("api/todos")
                .WithTags("Todos");

            group.MapGet("/", GetTodos)
                .WithName("GetTodos")
                .WithSummary("Get todos")
                .WithDescription("Get all todos or filter by status")
                .Produces<List<TodoDto>>(StatusCodes.Status200OK);

            group.MapGet("/{id}", GetTodoById)
                .WithName("GetTodoById")
                .WithSummary("Get todo by id")
                .WithDescription("Returns a todo by its id.")
                .Produces<TodoDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPost("/", CreateTodo)
                .WithName("CreateTodo")
                .WithSummary("Create todo")
                .WithDescription("Creates a new todo.")
                .Produces<TodoDto>(StatusCodes.Status201Created)
                .ProducesValidationProblem();

            group.MapPut("/{id}", UpdateTodo)
                .WithName("UpdateTodo")
                .WithSummary("Update todo")
                .WithDescription("Updates an existing todo.")
                .Produces<TodoDto>(StatusCodes.Status200OK)
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPatch("/{id}/toggle", ToggleTodo)
                .WithName("ToggleTodo")
                .WithSummary("Toggle todo completion")
                .WithDescription("Toggle the completion status of a todo.")
                .Produces<TodoDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapDelete("/{id}", DeleteTodo)
                .WithName("DeleteTodo")
                .WithSummary("Delete todo")
                .WithDescription("Deletes a todo by id.")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapDelete("/completed", ClearCompleted)
                .WithName("ClearCompleted")
                .WithSummary("Clear completed todos")
                .WithDescription("Deletes all completed todos.")
                .Produces(StatusCodes.Status204NoContent);
        }

        private static async Task<IResult> GetTodos(TodoFilter? filter, IMapper mapper)
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

        private static async Task<IResult> GetTodoById(string id, IMapper mapper)
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

        private static async Task<IResult> CreateTodo(CreateTodoRequest request, IValidator<CreateTodoRequest> validator, IMapper mapper)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

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

        private static async Task<IResult> UpdateTodo(string id, UpdateTodoRequest request, IValidator<UpdateTodoRequest> validator, IMapper mapper)
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

        private static async Task<IResult> ToggleTodo(string id, IMapper mapper)
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

        private static async Task<IResult> DeleteTodo(string id)
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

        private static async Task<IResult> ClearCompleted()
        {
            var completedTodo = await DB.Find<TodoItem>()
                .Match(x => x.IsCompleted)
                .ExecuteAsync();

            foreach (var todo in completedTodo)
            {
                await todo.DeleteAsync();
            }

            return Results.NoContent();
        }
    }
}
