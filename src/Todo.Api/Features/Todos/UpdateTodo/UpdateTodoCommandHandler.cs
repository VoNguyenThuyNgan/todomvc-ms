using AutoMapper;
using MediatR;
using MongoDB.Entities;
using Todo.Api.Features.Todos;

namespace Todo.Api.Features.Todos.UpdateTodo;

public class UpdateTodoCommandHandler
    : IRequestHandler<UpdateTodoCommand, TodoDto?>
{
    private readonly IMapper _mapper;

    public UpdateTodoCommandHandler(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<TodoDto?> Handle(
        UpdateTodoCommand request,
        CancellationToken cancellationToken)
    {
        var todo = await DB.Find<TodoItem>()
            .OneAsync(request.Id);

        if (todo is null)
        {
            return null;
        }

        todo.Title = request.Title;
        todo.IsCompleted = request.IsCompleted;
        todo.DueAt = request.DueAt;

        await todo.SaveAsync();

        return _mapper.Map<TodoDto>(todo);
    }
}