using AutoMapper;
using Carter;
using FluentValidation;
using MongoDB.Entities;
using System.ComponentModel.DataAnnotations;
using Todo.Api.Features.Todos;

namespace Todo.Api.Features.Reminders
{
    public class RemindersModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app
                .MapGroup("api/reminders")
                .WithTags("Reminders");

            group.MapGet("/", GetReminders)
                .WithName("GetReminders")
                .WithSummary("Get reminders")
                .WithDescription("Gets reminders by state.")
                .Produces<List<ReminderDto>>(StatusCodes.Status200OK);

            group.MapGet("/upcoming", GetUpcomingReminders)
                .WithName("GetUpcomingReminders")
                .WithSummary("Get upcoming reminders")
                .WithDescription("Gets todos that are approaching their due date.")
                .Produces<List<UpcomingTodoDto>>(StatusCodes.Status200OK);

            group.MapPatch("/{id}/snooze", SnoozeReminder)
                .WithName("SnoozeReminder")
                .WithSummary("Snooze reminder")
                .WithDescription("Delays a reminder for a specific number of minutes.")
                .Produces<ReminderDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPatch("/{id}/dismiss", DismissReminder)
                .WithName("DismissReminder")
                .WithSummary("Dismiss reminder")
                .WithDescription("Dismisses a reminder.")
                .Produces<ReminderDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> GetReminders(ReminderState? state, IMapper mapper)
        {
            state ??= ReminderState.Pending;

            var reminders = await DB.Find<Reminder>()
                .Match(x => x.State == state)
                .ExecuteAsync();

            var response = mapper.Map<List<ReminderDto>>(reminders);

            return Results.Ok(response);
        }

        private static async Task<IResult> GetUpcomingReminders(string? within)
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

        private static async Task<IResult> SnoozeReminder(string id, SnoozeReminderRequest request, IValidator<SnoozeReminderRequest> validator, IMapper mapper)
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

        private static async Task<IResult> DismissReminder(string id, IMapper mapper)
        {
            var reminder = await DB.Find<Reminder>()
                .OneAsync(id);

            if (reminder is null)
            {
                return Results.Problem(
                    title: "Reminder not found",
                    detail: $"Reminder with id '{id}' was not found.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            reminder.State = ReminderState.Dismissed;

            await reminder.SaveAsync();

            var response = mapper.Map<ReminderDto>(reminder);

            return Results.Ok(response);
        }
    }
}
