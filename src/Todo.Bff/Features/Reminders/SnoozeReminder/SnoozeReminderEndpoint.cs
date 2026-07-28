using Carter;
using Todo.Bff.Clients.Reminders;
using Todo.Bff.Extensions;

namespace Todo.Bff.Features.Reminders.SnoozeReminder
{
    public class SnoozeReminderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app
                .MapGroup("/bff/reminders")
                .WithTags("Reminders");

            group.MapPatch("/{id}/snooze", Handle)
                .WithName("BffSnoozeReminder")
                .WithSummary("Snooze reminder")
                .WithDescription(
                    "Proxy request to Todo.Api to snooze a reminder.")
                .Produces<ReminderDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

        }

        private static async Task<IResult> Handle(
            string id,
            SnoozeReminderRequest request,
            IReminderApiClient client)
        {
            var response = await client.SnoozeReminderAsync(
                id,
                request);

            return await response.ToResultAsync();
        }
    }
}
