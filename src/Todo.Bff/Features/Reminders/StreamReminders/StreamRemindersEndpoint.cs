using Carter;
using System.Text.Json;
using System.Threading.Channels;
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

    private static async Task Handle(HttpContext context, IReminderEventStream eventStream)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/event-stream";

        context.Response.Headers.CacheControl = "no-cache";
        context.Response.Headers.Append("X-Accel-Buffering", "no");

        await context.Response.Body.FlushAsync();

        var cancellationToken = context.RequestAborted;
        var channel = Channel.CreateUnbounded<ReminderDto>();

        eventStream.Subscribe(channel);


        try
        {
            await context.Response.Body.FlushAsync(
                cancellationToken);

            await foreach (
                var reminder in channel.Reader.ReadAllAsync(
                    cancellationToken))
            {
                await WriteEventAsync(
                    context.Response,
                    reminder,
                    cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Client disconnected.
        }
        finally
        {
            eventStream.Unsubscribe(channel);
        }
    }

    private static async Task WriteEventAsync(
        HttpResponse response,
        ReminderDto reminder,
        CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(
            reminder,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase
            });

        await response.WriteAsync(
            $"id: {reminder.Id}\n",
            cancellationToken);

        await response.WriteAsync(
            "event: reminder-fired\n",
            cancellationToken);

        await response.WriteAsync(
            $"data: {json}\n\n",
            cancellationToken);

        await response.Body.FlushAsync(
            cancellationToken);
    }
}