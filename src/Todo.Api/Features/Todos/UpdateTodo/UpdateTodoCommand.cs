using MediatR;

namespace Todo.Api.Features.Todos.UpdateTodo;

public record UpdateTodoCommand(
    string Id,
    string Title,
    bool IsCompleted,
    DateTime? DueAt
) : IRequest<TodoDto?>;