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
        context.Response.Headers.Append("Content-Type","text/event-stream");
        context.Response.Headers.Append("Cache-Control","no-cache");
        context.Response.Headers.Append("Connection","keep-alive");

        var cancellationToken =
            context.RequestAborted;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var reminders =
                    await reminderEventService.GetNewRemindersAsync(cancellationToken);

                if (reminders.Count == 0)
                {
                    await reminderEventService.WriteHeartbeatAsync(context.Response,cancellationToken);
                }
                else
                {
                    foreach (var reminder in reminders)
                    {
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