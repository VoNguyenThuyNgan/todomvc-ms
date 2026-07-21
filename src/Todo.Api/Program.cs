using Carter;
using FluentValidation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Entities;
using Todo.Api.Common.Configuration;
using Todo.Api.Features.Todos;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.Configure<MongoDbOptions>(
    builder.Configuration.GetSection(MongoDbOptions.SectionName));

// Services
builder.Services.AddAutoMapper(typeof(TodoMappings));
builder.Services.AddValidatorsFromAssemblyContaining<CreateTodoRequestValidator>();
builder.Services.AddCarter();

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
