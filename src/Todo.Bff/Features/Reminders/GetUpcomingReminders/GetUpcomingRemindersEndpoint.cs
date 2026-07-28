using Carter;
using Todo.Bff.Clients.Reminders;
using Todo.Bff.Extensions;

namespace Todo.Bff.Features.Reminders.GetUpcomingReminders
{
    public class GetUpcomingRemindersEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app
                .MapGroup("/bff/reminders")
                .WithTags("Reminders");

            group.MapGet("/upcoming", Handle)
                .WithName("BffGetUpcomingReminders")
                .WithSummary("Get upcoming reminders")
                .WithDescription(
                    "Proxy request to Todo.Api to retrieve upcoming todos.")
                .Produces<List<UpcomingTodoDto>>(StatusCodes.Status200OK);
        }

        private static async Task<IResult> Handle(
            string? within,
            IReminderApiClient client)
        {
            var response = await client.GetUpcomingRemindersAsync(within);

            return await response.ToResultAsync();
        }
    }
}
