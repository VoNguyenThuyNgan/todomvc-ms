namespace Todo.Api.Features.Reminders.GetUpComingReminders
{
    public class UpcomingTodoDto
    {
        public string TodoId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime DueAt { get; set; }
    }
}
