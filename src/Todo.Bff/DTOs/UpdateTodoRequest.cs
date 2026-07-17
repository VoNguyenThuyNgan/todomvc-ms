namespace Todo.Bff.DTOs
{
    public class UpdateTodoRequest
    {
        public string Title { get; set; } = default!;

        public bool IsCompleted { get; set; }
    }
}
