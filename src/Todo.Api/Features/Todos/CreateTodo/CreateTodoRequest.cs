namespace Todo.Api.Features.Todos.CreateTodo
{
    public class CreateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public DateTime? DueAt { get; set; }
    }
}
