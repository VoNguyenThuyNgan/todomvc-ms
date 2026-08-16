using MongoDB.Entities;
using Todo.Api.Features.Reminders.Contracts;
using Todo.Api.Features.Todos;
using Todo.Api.Services;
namespace Todo.Api.Features.Reminders
{
    public class ReminderScanner : BackgroundService
    {
        private readonly ILogger<ReminderScanner> _logger;
        private readonly IServiceBusPublisher _publisher;

        public ReminderScanner(ILogger<ReminderScanner> logger, IServiceBusPublisher publisher)
        {
            _logger = logger;
            _publisher = publisher;

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reminder scanner started.");

            using var timer = new PeriodicTimer(
                TimeSpan.FromSeconds(30));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await ScanAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "An error occurred while scanning reminders.");
                }
            }

            _logger.LogInformation("Reminder scanner stopped.");
        }

        private async Task ScanAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            await CreatePendingRemindersAsync(
                now,
                cancellationToken);
            await ReactivateSnoozedRemindersAsync(
                now,
                cancellationToken);
            await DismissCompletedTodoRemindersAsync(
                cancellationToken);
            await DismissOrphanedRemindersAsync(
                cancellationToken);
        }

        private async Task CreatePendingRemindersAsync(DateTime now, CancellationToken cancellationToken)
        {
            var todos = await DB.Find<TodoItem>()
                .Match(todo =>
                    todo.DueAt != null &&
                    todo.DueAt <= now &&
                    !todo.IsCompleted)
                .ExecuteAsync();

            var todoIds = todos
            .Select(todo => todo.ID)
            .ToList();

            var existingReminders = await DB.Find<Reminder>()
            .Match(reminder =>
                todoIds.Contains(reminder.TodoId))
            .ExecuteAsync();

            var reminderTodoIds = existingReminders
            .Select(reminder => reminder.TodoId)
            .ToHashSet();

            foreach (var todo in todos)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (reminderTodoIds.Contains(todo.ID))
                {
                    continue;
                }

                var reminder = new Reminder
                {
                    TodoId = todo.ID,
                    TodoTitle = todo.Title,
                    DueAt = todo.DueAt!.Value,
                    State = ReminderState.Pending,
                    FireAt = now
                };

                await reminder.SaveAsync();

                var notificationEvent = new ReminderNotificationEvent
                {
                    Id = reminder.ID,
                    TodoId = reminder.TodoId,
                    TodoTitle = reminder.TodoTitle,
                    DueAt = reminder.DueAt,
                    State = (int)reminder.State,
                    SnoozeUntil = reminder.SnoozeUntil,
                    FiredAt = reminder.FireAt
                };

                await _publisher.PublishAsync(notificationEvent, cancellationToken);

                _logger.LogInformation(
                    "Reminder created for Todo {TodoId}.",
                    todo.ID);
            }
        }

        private async Task ReactivateSnoozedRemindersAsync(DateTime now, CancellationToken cancellationToken)
        {
            var reminders = await DB.Find<Reminder>()
                .Match(reminder =>
                    reminder.State == ReminderState.Snoozed &&
                    reminder.SnoozeUntil != null &&
                    reminder.SnoozeUntil <= now)
                .ExecuteAsync();

            foreach (var reminder in reminders)
            {
                cancellationToken.ThrowIfCancellationRequested();

                reminder.State = ReminderState.Pending;
                reminder.SnoozeUntil = null;

                await reminder.SaveAsync();

                _logger.LogInformation(
                    "Reminder {ReminderId} reactivated from Snoozed to Pending.",
                    reminder.ID);
            }
        }
        private async Task DismissCompletedTodoRemindersAsync(CancellationToken cancellationToken)
        {
            var completedTodos = await DB.Find<TodoItem>()
                .Match(todo => todo.IsCompleted)
                .ExecuteAsync();

            var completedTodoIds = completedTodos
                .Select(todo => todo.ID)
                .ToList();

            var reminders = await DB.Find<Reminder>()
                .Match(reminder =>
                    completedTodoIds.Contains(reminder.TodoId) &&
                    reminder.State != ReminderState.Dismissed)
                .ExecuteAsync();

            foreach (var reminder in reminders)
            {
                cancellationToken.ThrowIfCancellationRequested();

                reminder.State = ReminderState.Dismissed;
                reminder.SnoozeUntil = null;

                    await reminder.SaveAsync();

                    _logger.LogInformation(
                        "Reminder {ReminderId} dismissed because Todo {TodoId} was completed.",
                        reminder.ID,
                        reminder.TodoId);
                
            }
        }

        private async Task DismissOrphanedRemindersAsync(CancellationToken cancellationToken)
        {
            var reminders = await DB.Find<Reminder>()
                .Match(reminder =>
                    reminder.State != ReminderState.Dismissed)
                .ExecuteAsync();

            var todoIds = reminders
           .Select(reminder => reminder.TodoId)
           .Distinct()
           .ToList();

            var existingTodos = await DB.Find<TodoItem>()
                .Match(todo => todoIds.Contains(todo.ID))
                .ExecuteAsync();

            var existingTodoIdSet = existingTodos
                .Select(todo => todo.ID)
                .ToHashSet();

            foreach (var reminder in reminders)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (existingTodoIdSet.Contains(reminder.TodoId))
                {
                    continue;
                }

                reminder.State = ReminderState.Dismissed;
                reminder.SnoozeUntil = null;

                await reminder.SaveAsync();

                _logger.LogInformation(
                    "Reminder {ReminderId} dismissed because Todo {TodoId} no longer exists.",
                    reminder.ID,
                    reminder.TodoId);
            }
        }
    }
}
