namespace Todo.Bff.DTOs
{
    public class TodoDto
    {
        public string Id { get; set; } = default!;
        public string Title { get; set; } = default!;
        public bool IsCompleted { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
