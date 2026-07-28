using AutoMapper;
using Carter;
using MongoDB.Entities;

namespace Todo.Api.Features.Reminders.GetReminders
{
    public class GetRemindersEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app
                .MapGroup("api/reminders")
                .WithTags("Reminders");

            group.MapGet("/", Handle)
                .WithName("GetReminders")
                .WithSummary("Get reminders")
                .WithDescription("Gets reminders by state.")
                .Produces<List<ReminderDto>>(StatusCodes.Status200OK);
        }

        private static async Task<IResult> Handle(ReminderState? state, IMapper mapper)
        {
            state ??= ReminderState.Pending;

            var reminders = await DB.Find<Reminder>()
                .Match(x => x.State == state)
                .ExecuteAsync();

            var response = mapper.Map<List<ReminderDto>>(reminders);
            return Results.Ok(response);
        }
    }
}
