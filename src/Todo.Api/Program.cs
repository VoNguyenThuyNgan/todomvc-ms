using MongoDB.Driver;
using MongoDB.Entities;
using Todo.Api.Entities;

var builder = WebApplication.CreateBuilder(args);

await DB.InitAsync("TodoMVCDb", "localhost", 27017);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/todos", async (TodoItem todo) =>
{
    await todo.SaveAsync();
    return Results.Created($"/todos/{todo.ID}", todo);
});

app.Run();
