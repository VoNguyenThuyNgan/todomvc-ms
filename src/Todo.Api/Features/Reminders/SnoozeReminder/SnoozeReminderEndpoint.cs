using AutoMapper;
using Carter;
using FluentValidation;
using MongoDB.Entities;

namespace Todo.Api.Features.Reminders.SnoozeReminder
{
    public class SnoozeReminderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app
                .MapGroup("api/reminders")
                .WithTags("Reminders");

            group.MapPatch("/{id}/snooze", Handle)
                .WithName("SnoozeReminder")
                .WithSummary("Snooze reminder")
                .WithDescription("Delays a reminder for a specific number of minutes.")
                .Produces<ReminderDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> Handle(string id, SnoozeReminderRequest request, IValidator<SnoozeReminderRequest> validator, IMapper mapper)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(
                    validationResult.ToDictionary());
            }

            var reminder = await DB.Find<Reminder>()
                .OneAsync(id);

            if (reminder is null)
            {
                return Results.Problem(
                    title: "Reminder not found",
                    detail: $"Reminder with id '{id}' was not found.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            if (reminder.State == ReminderState.Dismissed)
            {
                return Results.Problem(
                    title: "Reminder already dismissed",
                    detail: "A dismissed reminder cannot be snoozed.",
                    statusCode: StatusCodes.Status409Conflict);
            }

            reminder.State = ReminderState.Snoozed;
            reminder.SnoozeUntil = DateTime.UtcNow.AddMinutes(request.Minutes);

            await reminder.SaveAsync();

            var response = mapper.Map<ReminderDto>(reminder);

            return Results.Ok(response);
        }


    }
}
