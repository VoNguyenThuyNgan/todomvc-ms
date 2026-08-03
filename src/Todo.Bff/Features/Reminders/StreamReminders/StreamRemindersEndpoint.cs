using Carter;
using Todo.Bff.Services;

namespace Todo.Bff.Features.Reminders.StreamReminders;

public class StreamRemindersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/bff/reminders/stream",Handle)
            .WithName("ReminderStream")
            .WithTags("Reminders")
            .WithSummary("Reminder SSE Stream")
            .WithDescription("Streams reminder events to frontend.");
    }

    private static async Task Handle(HttpContext context, ReminderEventService reminderEventService)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/event-stream";

        context.Response.Headers.CacheControl = "no-cache";
        context.Response.Headers.Append("X-Accel-Buffering", "no");

        await context.Response.Body.FlushAsync();

        var cancellationToken =
            context.RequestAborted;

        var knownIds = new HashSet<string>();

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var reminders =
                    await reminderEventService.GetNewRemindersAsync(knownIds, cancellationToken);

                if (reminders.Count == 0)
                {
                    Console.WriteLine("Heartbeat");
                    await reminderEventService.WriteHeartbeatAsync(context.Response,cancellationToken);
                }
                else
                {
                    foreach (var reminder in reminders)
                    {
                        Console.WriteLine($"Sending reminder {reminder.Id}");
                        await reminderEventService.WriteEventAsync(context.Response, reminder, cancellationToken);
                    }
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                break;
            }
        }
    }
}