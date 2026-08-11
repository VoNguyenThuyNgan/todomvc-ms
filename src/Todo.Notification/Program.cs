using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using Todo.Notification;
using Todo.Notification.Channels;
using Todo.Notification.Channels.Email;
using Todo.Notification.Configuration;
using Todo.Notification.Email;

var builder = Host.CreateApplicationBuilder(args);

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

builder.Services.AddHostedService<EmailWorker>();
builder.Services.AddSingleton<IEmailSender, FakeEmailSender>();
builder.Services.AddSingleton<INotificationChannel, EmailChannel>();
builder.Services.AddHostedService<EmailWorker>();

var host = builder.Build();
host.Run();
