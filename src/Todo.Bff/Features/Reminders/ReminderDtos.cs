namespace Todo.Bff.Features.Reminders
{
    public class ReminderDto
    {
        public string Id { get; set; } = string.Empty;
        public string TodoId { get; set; } = string.Empty;
        public DateTime DueAt { get; set; }
        public ReminderState State { get; set; }
        public DateTime? SnoozeUntil { get; set; }
        public DateTime FiredAt { get; set; }
    }

    public class UpcomingTodoDto
    {
        public string TodoId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime DueAt { get; set; }
    }

    public class SnoozeReminderRequest
    {
        public int Minutes { get; set; }
    }

    public enum ReminderState
    {
        Pending = 0,
        Snoozed = 1,
        Dismissed = 2
    }
}
