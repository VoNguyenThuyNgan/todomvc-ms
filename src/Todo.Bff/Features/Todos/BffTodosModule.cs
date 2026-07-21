using Carter;
using Todo.Bff.Clients;
using Todo.Bff.Extensions;

namespace Todo.Bff.Features.Todos
{
    public class BffTodosModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/bff/todos")
                           .WithTags("Todos");

            group.MapGet("/", GetTodos)
                .WithName("BffGetTodos")
                .WithSummary("Get todos")
                .WithDescription("Proxy request to Todo.Api to retrieve all todos or filter by status.")
                .Produces<List<TodoDto>>(StatusCodes.Status200OK);

            group.MapGet("/{id}", GetTodoById)
                .WithName("BffGetTodoById")
                .WithSummary("Get todo by id")
                .WithDescription("Proxy request to Todo.Api to retrieve a todo by id.")
                .Produces<TodoDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPost("/", CreateTodo)
                .WithName("BffCreateTodo")
                .WithSummary("Create todo")
                .WithDescription("Proxy request to Todo.Api to create a new todo.")
                .Produces<TodoDto>(StatusCodes.Status201Created)
                .ProducesValidationProblem();

            group.MapPut("/{id}", UpdateTodo)
                .WithName("BffUpdateTodo")
                .WithSummary("Update todo")
                .WithDescription("Proxy request to Todo.Api to update an existing todo.")
                .Produces<TodoDto>(StatusCodes.Status200OK)
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPatch("/{id}/toggle", ToggleTodo)
                .WithName("BffToggleTodo")
                .WithSummary("Toggle todo completion")
                .WithDescription("Proxy request to Todo.Api to toggle the completion status of a todo.")
                .Produces<TodoDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapDelete("/{id}", DeleteTodo)
                .WithName("BffDeleteTodo")
                .WithSummary("Delete todo")
                .WithDescription("Proxy request to Todo.Api to delete a todo by id.")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapDelete("/completed", ClearCompleted)
                .WithName("BffClearCompleted")
                .WithSummary("Clear completed todos")
                .WithDescription("Proxy request to Todo.Api to delete all completed todos.")
                .Produces(StatusCodes.Status204NoContent);

            group.MapPatch("/toggle-all", ToggleAllTodos)
                .WithName("BffToggleAllTodos")
                .WithSummary("Toggle all todos")
                .WithDescription("Proxy request to Todo.Api.")
                .Produces(StatusCodes.Status204NoContent);
        }

        private static async Task<IResult> GetTodos(TodoFilter? filter, ITodoApiClient client)
        {
            var response = await client.GetTodosAsync(filter);

            return await response.ToResultAsync();
        }

        private static async Task<IResult> GetTodoById(string id, ITodoApiClient client)
        {
            var response = await client.GetTodoByIdAsync(id);

            return await response.ToResultAsync();
        }

        private static async Task<IResult> CreateTodo(CreateTodoRequest request, ITodoApiClient client)
        {
            var response = await client.CreateTodoAsync(request);

            return await response.ToResultAsync();
        }

        private static async Task<IResult> UpdateTodo(string id, UpdateTodoRequest request, ITodoApiClient client)
        {
            var response = await client.UpdateTodoAsync(id, request);

            return await response.ToResultAsync();
        }

        private static async Task<IResult> ToggleTodo(string id, ITodoApiClient client)
        {
            var response = await client.ToggleTodoAsync(id);

            return await response.ToResultAsync();
        }

        private static async Task<IResult> DeleteTodo(string id, ITodoApiClient client)
        {
            var response = await client.DeleteTodoAsync(id);

            return await response.ToResultAsync();
        }

        private static async Task<IResult> ClearCompleted(ITodoApiClient client)
        {
            var response = await client.ClearCompletedAsync();

            return await response.ToResultAsync();
        }
        private static async Task<IResult> ToggleAllTodos(ToggleAllTodosRequest request, ITodoApiClient client)
        {
            var response = await client.ToggleAllTodosAsync(request);

            return await response.ToResultAsync();
        }

    }
}
