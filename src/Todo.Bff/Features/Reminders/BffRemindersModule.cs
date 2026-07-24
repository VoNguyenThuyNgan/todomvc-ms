using Carter;
using Todo.Bff.Clients;
using Todo.Bff.Extensions;
using Todo.Bff.Features.Reminders;

namespace Todo.Bff.Features.Reminders
{
    public class BffRemindersModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app
                .MapGroup("/bff/reminders")
                .WithTags("Reminders");

            group.MapGet("/", GetReminders)
                .WithName("BffGetReminders")
                .WithSummary("Get reminders")
                .WithDescription(
                    "Proxy request to Todo.Api to retrieve reminders by state.")
                .Produces<List<ReminderDto>>(StatusCodes.Status200OK);

            group.MapGet("/upcoming", GetUpcomingReminders)
                .WithName("BffGetUpcomingReminders")
                .WithSummary("Get upcoming reminders")
                .WithDescription(
                    "Proxy request to Todo.Api to retrieve upcoming todos.")
                .Produces<List<UpcomingTodoDto>>(StatusCodes.Status200OK);

            group.MapPatch("/{id}/snooze", SnoozeReminder)
                .WithName("BffSnoozeReminder")
                .WithSummary("Snooze reminder")
                .WithDescription(
                    "Proxy request to Todo.Api to snooze a reminder.")
                .Produces<ReminderDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPatch("/{id}/dismiss", DismissReminder)
                .WithName("BffDismissReminder")
                .WithSummary("Dismiss reminder")
                .WithDescription(
                    "Proxy request to Todo.Api to dismiss a reminder.")
                .Produces<ReminderDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> GetReminders(
            ReminderState? state,
            ITodoApiClient client)
        {
            var response = await client.GetRemindersAsync(state);

            return await response.ToResultAsync();
        }

        private static async Task<IResult> GetUpcomingReminders(
            string? within,
            ITodoApiClient client)
        {
            var response = await client.GetUpcomingRemindersAsync(within);

            return await response.ToResultAsync();
        }

        private static async Task<IResult> SnoozeReminder(
            string id,
            SnoozeReminderRequest request,
            ITodoApiClient client)
        {
            var response = await client.SnoozeReminderAsync(
                id,
                request);

            return await response.ToResultAsync();
        }

        private static async Task<IResult> DismissReminder(
            string id,
            ITodoApiClient client)
        {
            var response = await client.DismissReminderAsync(id);

            return await response.ToResultAsync();
        }
    }
}
