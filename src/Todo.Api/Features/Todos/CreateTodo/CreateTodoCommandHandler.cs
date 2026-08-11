using AutoMapper;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Features.Todos;

namespace Todo.Api.Features.Todos.CreateTodo;

public class CreateTodoCommandHandler
    : IRequestHandler<CreateTodoCommand, TodoDto>
{
    private readonly IMapper _mapper;

    public CreateTodoCommandHandler(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<TodoDto> Handle(
        CreateTodoCommand request,
        CancellationToken cancellationToken)
    {
        var todo = _mapper.Map<TodoItem>(request);

        todo.CreateAt = DateTime.UtcNow;
        todo.IsCompleted = false;

        await todo.SaveAsync();

        return _mapper.Map<TodoDto>(todo);
    }
}