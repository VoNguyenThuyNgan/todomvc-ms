using MongoDB.Entities;
namespace Todo.Api.Features.Reminders
{
    public class Reminder : Entity
    {
        public string TodoId { get; set; } = string.Empty;
        public DateTime DueAt { get; set; }
        public ReminderState State { get; set; }
        public DateTime? SnoozeUntil { get; set; }
        public DateTime FireAt { get; set; }
    }
}
