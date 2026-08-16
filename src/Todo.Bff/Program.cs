using Azure.Messaging.ServiceBus;
using Carter;
using Microsoft.Extensions.Options;
using Todo.Bff.Clients.Reminders;
using Todo.Bff.Clients.Statistics;
using Todo.Bff.Clients.Todos;
using Todo.Bff.Common.Configuration;
using Todo.Bff.Features.Reminders;
using Todo.Bff.Services;

var builder = WebApplication.CreateBuilder(args);

// Carter
builder.Services.AddCarter();

// Swagger
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Typed HttpClient
builder.Services.AddHttpClient("TodoApi", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["TodoApi:BaseUrl"]!);
});

builder.Services.AddScoped<ITodoApiClient, TodoApiClient>();
builder.Services.AddScoped<IReminderApiClient, ReminderApiClient>();
builder.Services.AddSingleton<IReminderEventStream, ReminderEventStream>();
builder.Services.AddScoped<IStatisticsApiClient, StatisticsApiClient>();
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("Angular");
app.MapCarter();
app.Run();
