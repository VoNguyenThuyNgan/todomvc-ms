using AutoMapper;
using Carter;
using MongoDB.Entities;

namespace Todo.Api.Features.Reminders.DismissReminder
{
    public class DismissReminderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app
                .MapGroup("api/reminders")
                .WithTags("Reminders");

            group.MapPatch("/{id}/dismiss", DismissReminder)
                .WithName("DismissReminder")
                .WithSummary("Dismiss reminder")
                .WithDescription("Dismisses a reminder.")
                .Produces<ReminderDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> DismissReminder(string id, IMapper mapper)
        {
            var reminder = await DB.Find<Reminder>()
                .OneAsync(id);

            if (reminder is null)
            {
                return Results.Problem(
                    title: "Reminder not found",
                    detail: $"Reminder with id '{id}' was not found.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            reminder.State = ReminderState.Dismissed;

            await reminder.SaveAsync();

            var response = mapper.Map<ReminderDto>(reminder);

            return Results.Ok(response);
        }
    }
}
