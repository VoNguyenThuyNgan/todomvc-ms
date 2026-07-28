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

    public enum ReminderState
    {
        Pending = 0,
        Snoozed = 1,
        Dismissed = 2
    }
}
