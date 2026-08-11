using AutoMapper;
using Todo.Api.Features.Todos.CreateTodo;
using Todo.Api.Features.Todos.UpdateTodo;

namespace Todo.Api.Features.Todos
{
    public class TodoMappings : Profile
    {
        public TodoMappings() {
            CreateMap<TodoItem, TodoDto>()
                .ForMember(
                    destination => destination.Id,
                    options => options.MapFrom(source => source.ID));

            CreateMap<CreateTodoRequest, TodoItem>();
            CreateMap<UpdateTodoRequest, TodoItem>();
            CreateMap<CreateTodoCommand, TodoItem>();
        }
    }
}
