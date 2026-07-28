using MongoDB.Entities;
using Todo.Api.Features.Todos;
using Carter;

namespace Todo.Api.Features.Reminders.GetUpComingReminders
{
    public class GetUpcomingRemindersEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app
                .MapGroup("api/reminders")
                .WithTags("Reminders");

            group.MapGet("/upcoming", Handle)
                .WithName("GetUpcomingReminders")
                .WithSummary("Get upcoming reminders")
                .WithDescription("Gets todos that are approaching their due date.")
                .Produces<List<UpcomingTodoDto>>(StatusCodes.Status200OK);
        }

        private static async Task<IResult> Handle(string? within)
        {
            var hours = ParseWithinHours(within);

            var now = DateTime.UtcNow;
            var until = now.AddHours(hours);

            var todos = await DB.Find<TodoItem>()
                .Match(x =>
                    x.DueAt != null &&
                    x.DueAt >= now &&
                    x.DueAt <= until &&
                    !x.IsCompleted)
                .ExecuteAsync();

            var response = todos.Select(todo => new UpcomingTodoDto
            {
                TodoId = todo.ID,
                Title = todo.Title,
                DueAt = todo.DueAt!.Value
            });

            return Results.Ok(response);
        }
        private static int ParseWithinHours(string? within)
        {
            if (string.IsNullOrWhiteSpace(within))
            {
                return 24;
            }

            if (within.EndsWith("h") && int.TryParse(within[..^1], out var hours) && hours > 0)
            {
                return hours;
            }

            return 24;
        }

    }
}
