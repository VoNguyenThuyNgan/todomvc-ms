namespace Todo.Bff.Features.Todos.CreateTodo
{
    public class CreateTodoRequest
    {
        public string Title { get; set; } = default!;
        public DateTime? DueAt { get; set; }
    }
}
