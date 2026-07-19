using Carter;
using Todo.Bff.Clients;

var builder = WebApplication.CreateBuilder(args);

// Carter
builder.Services.AddCarter();

// Typed HttpClient
builder.Services.AddHttpClient<ITodoApiClient, TodoApiClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["TodoApi:BaseUrl"]!);
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("Angular");
app.MapCarter();
app.MapGet("/", () => "Hello World!");

app.Run();
