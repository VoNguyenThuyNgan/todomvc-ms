namespace Todo.Bff.Features.Todos
{
    public class TodoDto
    {
        public string Id { get; set; } = default!;
        public string Title { get; set; } = default!;
        public bool IsCompleted { get; set; }
        public DateTime CreateAt { get; set; }
    }

    public class CreateTodoRequest
    {
        public string Title { get; set; } = default!;
    }

    public class UpdateTodoRequest
    {
        public string Title { get; set; } = default!;

        public bool IsCompleted { get; set; }
    }

    public class ToggleAllTodosRequest
    {
        public bool IsCompleted { get; set; }
    }
}
