namespace Todo.Api.Features.Reminders
{
    public class ReminderDto
    {
        public string Id { get; set; } = default!;
        public string TodoId { get; set; } = default!;
        public string TodoTitle { get; set; } = default!;
        public DateTime DueAt { get; set; }
        public ReminderState State { get; set; }
        public DateTime? SnoozeUntil { get; set; }
        public DateTime FiredAt { get; set; }
    }
}
