using Carter;
using Todo.Bff.Clients.Reminders;
using Todo.Bff.Extensions;

namespace Todo.Bff.Features.Reminders.GetReminders
{
    public class GetRemindersEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app
                .MapGroup("/bff/reminders")
                .WithTags("Reminders");

            group.MapGet("/", Handle)
                .WithName("BffGetReminders")
                .WithSummary("Get reminders")
                .WithDescription(
                    "Proxy request to Todo.Api to retrieve reminders by state.")
                .Produces<List<ReminderDto>>(StatusCodes.Status200OK);
        }

        private static async Task<IResult> Handle(
            ReminderState? state,
            IReminderApiClient client)
        {
            var response = await client.GetRemindersAsync(state);

            return await response.ToResultAsync();
        }
    }
}

