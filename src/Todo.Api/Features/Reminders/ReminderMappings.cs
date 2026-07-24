using AutoMapper;

namespace Todo.Api.Features.Reminders
{
    public class ReminderMappings : Profile
    {
        public ReminderMappings()
        {
            CreateMap<Reminder, ReminderDto>()
                .ForMember(
                    destination => destination.Id,
                    options => options.MapFrom(source => source.ID))
                .ForMember(
                    destination => destination.FiredAt,
                    options => options.MapFrom(source => source.FireAt));
        }
    }
}
