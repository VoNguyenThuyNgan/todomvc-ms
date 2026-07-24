using MongoDB.Entities;
namespace Todo.Api.Features.Todos
{
    public class TodoItem : Entity
    {
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DueAt { get; set; }
    }
}
