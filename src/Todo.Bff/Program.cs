using Carter;
using Todo.Bff.Clients.Reminders;
using Todo.Bff.Clients.Todos;
using Todo.Bff.Services;

var builder = WebApplication.CreateBuilder(args);

// Carter
builder.Services.AddCarter();

// Typed HttpClient
builder.Services.AddHttpClient("TodoApi", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["TodoApi:BaseUrl"]!);
});

builder.Services.AddScoped<ITodoApiClient, TodoApiClient>();
builder.Services.AddScoped<IReminderApiClient, ReminderApiClient>();
builder.Services.AddScoped<ReminderEventService>();

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
