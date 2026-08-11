using Azure.Messaging.ServiceBus;
using Carter;
using FluentValidation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Entities;
using Todo.Api.Common.Configuration;
using Todo.Api.Features.Reminders;
using Todo.Api.Features.Todos.CreateTodo;
using Todo.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.Configure<MongoDbOptions>(
    builder.Configuration.GetSection(MongoDbOptions.SectionName));
builder.Services.Configure<ServiceBusOptions>(
    builder.Configuration.GetSection(ServiceBusOptions.SectionName));

// Services
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateTodoRequestValidator>();
builder.Services.AddCarter();
builder.Services.AddHostedService<ReminderScanner>();
//builder.Services.AddHostedService<ReminderWorker>();
builder.Services.AddSingleton(sp =>
{
    var options = sp
        .GetRequiredService<IOptions<ServiceBusOptions>>()
        .Value;

    return new ServiceBusClient(options.ConnectionString);
});
builder.Services.AddSingleton<IServiceBusPublisher, ServiceBusPublisher>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// MongoDB
var mongoOptions = app.Services
    .GetRequiredService<IOptions<MongoDbOptions>>()
    .Value;
var clientSettings = MongoClientSettings.FromConnectionString(mongoOptions.ConnectionString);

await DB.InitAsync(
    mongoOptions.DatabaseName,
    clientSettings);

app.UseHttpsRedirection();
app.MapCarter();

app.Run();
