using Carter;
using Todo.Bff.Clients.Reminders;
using Todo.Bff.Extensions;

namespace Todo.Bff.Features.Reminders.DismissReminder
{
    public class DismissReminderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app
                .MapGroup("/bff/reminders")
                .WithTags("Reminders");

            group.MapPatch("/{id}/dismiss", DismissReminder)
                .WithName("BffDismissReminder")
                .WithSummary("Dismiss reminder")
                .WithDescription(
                    "Proxy request to Todo.Api to dismiss a reminder.")
                .Produces<ReminderDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> DismissReminder(
            string id,
            IReminderApiClient client)
        {
            var response = await client.DismissReminderAsync(id);

            return await response.ToResultAsync();
        }
    }
}
