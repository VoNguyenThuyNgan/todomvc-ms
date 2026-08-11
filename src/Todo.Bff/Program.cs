using Azure.Messaging.ServiceBus;
using Carter;
using Microsoft.Extensions.Options;
using Todo.Bff.Clients.Reminders;
using Todo.Bff.Clients.Todos;
using Todo.Bff.Common.Configuration;
using Todo.Bff.Features.Reminders;
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
builder.Services.AddSingleton<
    IReminderEventStream,
    ReminderEventStream>();

// Service Bus
builder.Services.Configure<ServiceBusOptions>(
    builder.Configuration.GetSection(
        ServiceBusOptions.SectionName));
builder.Services.AddSingleton(sp =>
{
    var options = sp
        .GetRequiredService<IOptions<ServiceBusOptions>>()
        .Value;

    return new ServiceBusClient(
        options.ConnectionString);
});

builder.Services.AddHostedService<ReminderWorker>();



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
