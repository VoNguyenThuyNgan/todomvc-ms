namespace Todo.Api.Features.Todos
{
    public class TodoDtos
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreateAt { get; set; }
    }

    public class CreateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
    }

    public class UpdateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }

    public class ToggleAllTodosRequest
    {
        public bool IsCompleted { get; set; }
    }
}
