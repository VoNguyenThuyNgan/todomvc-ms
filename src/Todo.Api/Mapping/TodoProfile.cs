using AutoMapper;
using Todo.Api.DTOs;
using Todo.Api.Entities;

namespace Todo.Api.Mapping
{
    public class TodoProfile : Profile
    {
        public TodoProfile() {
            CreateMap<TodoItem, TodoDto>()
                .ForMember(
                    destination => destination.Id,
                    options => options.MapFrom(source => source.ID));

            CreateMap<CreateTodoRequest, TodoItem>();
            CreateMap<UpdateTodoRequest, TodoItem>();
        }
    }
}
