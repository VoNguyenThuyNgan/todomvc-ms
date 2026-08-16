using Carter;
using Todo.Api.Services;

namespace Todo.Api.Features.Test;

public class TestServiceBusModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var environment = app.ServiceProvider
            .GetRequiredService<IHostEnvironment>();

        if (!environment.IsDevelopment())
        {
            return;
        }

        app.MapPost("/api/test/send", async (
            IServiceBusPublisher publisher,
            CancellationToken cancellationToken) =>
        {
            var message = new
            {
                Id = Guid.NewGuid(),
                Message = "Hello Azure Service Bus!",
                CreatedAt = DateTime.UtcNow
            };

            await publisher.PublishAsync(message, cancellationToken);

            return Results.Ok(new
            {
                Success = true,
                Message = "Message sent successfully."
            });
        });
    }
}