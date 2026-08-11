using MediatR;

namespace Todo.Api.Features.Todos.CreateTodo;

public record CreateTodoCommand(
    string Title,
    DateTime? DueAt
) : IRequest<TodoDto>;