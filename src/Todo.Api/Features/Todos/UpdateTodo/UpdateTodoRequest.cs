namespace Todo.Api.Features.Todos.UpdateTodo
{
    public class UpdateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime? DueAt { get; set; }
    }
}
