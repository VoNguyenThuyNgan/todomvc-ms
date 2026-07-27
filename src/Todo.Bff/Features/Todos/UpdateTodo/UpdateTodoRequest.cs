namespace Todo.Bff.Features.Todos.UpdateTodo
{
    public class UpdateTodoRequest
    {
        public string Title { get; set; } = default!;

        public bool IsCompleted { get; set; }
        public DateTime? DueAt { get; set; }
    }
}
