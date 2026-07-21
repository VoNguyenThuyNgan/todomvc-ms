using AutoMapper;

namespace Todo.Api.Features.Todos
{
    public class TodoMappings : Profile
    {
        public TodoMappings() {
            CreateMap<TodoItem, TodoDtos>()
                .ForMember(
                    destination => destination.Id,
                    options => options.MapFrom(source => source.ID));

            CreateMap<CreateTodoRequest, TodoItem>();
            CreateMap<UpdateTodoRequest, TodoItem>();
        }
    }
}
