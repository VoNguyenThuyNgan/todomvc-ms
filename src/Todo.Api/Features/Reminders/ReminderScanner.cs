using MongoDB.Entities;
using Todo.Api.Features.Todos;
namespace Todo.Api.Features.Reminders
{
    public class ReminderScanner : BackgroundService
    {
        private readonly ILogger<ReminderScanner> _logger;

        public ReminderScanner(ILogger<ReminderScanner> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
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

            foreach (var todo in todos)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var existingReminder = await DB.Find<Reminder>()
                    .Match(reminder =>
                        reminder.TodoId == todo.ID &&
                        reminder.State != ReminderState.Dismissed)
                    .ExecuteFirstAsync();

                if (existingReminder is not null)
                {
                    continue;
                }

                var reminder = new Reminder
                {
                    TodoId = todo.ID,
                    DueAt = todo.DueAt!.Value,
                    State = ReminderState.Pending,
                    FireAt = now
                };

                await reminder.SaveAsync();

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

            foreach (var todo in completedTodos)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var reminders = await DB.Find<Reminder>()
                    .Match(reminder =>
                        reminder.TodoId == todo.ID &&
                        reminder.State != ReminderState.Dismissed)
                    .ExecuteAsync();

                foreach (var reminder in reminders)
                {
                    reminder.State = ReminderState.Dismissed;
                    reminder.SnoozeUntil = null;

                    await reminder.SaveAsync();

                    _logger.LogInformation(
                        "Reminder {ReminderId} dismissed because Todo {TodoId} was completed.",
                        reminder.ID,
                        todo.ID);
                }
            }
        }

        private async Task DismissOrphanedRemindersAsync(CancellationToken cancellationToken)
        {
            var reminders = await DB.Find<Reminder>()
                .Match(reminder =>
                    reminder.State != ReminderState.Dismissed)
                .ExecuteAsync();

            foreach (var reminder in reminders)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var todo = await DB.Find<TodoItem>()
                    .OneAsync(reminder.TodoId);

                if (todo is null)
                {
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
}
