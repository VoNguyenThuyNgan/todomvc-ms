namespace Todo.Notification.Contracts;

public class ReminderNotificationEvent
{
    public string Id { get; set; } = string.Empty;
    public string TodoId { get; set; } = string.Empty;
    public string TodoTitle { get; set; } = string.Empty;
    public DateTime DueAt { get; set; }
    public int State { get; set; }
    public DateTime? SnoozeUntil { get; set; }
    public DateTime FiredAt { get; set; }

    public string? RecipientEmail { get; set; }
}