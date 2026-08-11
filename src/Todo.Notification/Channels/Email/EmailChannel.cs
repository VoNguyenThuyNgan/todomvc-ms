using System;
using System.Collections.Generic;
using System.Text;
using Todo.Notification.Contracts;

namespace Todo.Notification.Channels.Email
{
    public class EmailChannel : INotificationChannel
    {
        private readonly IEmailSender _emailSender;

        public string ChannelName => "Email";

        public EmailChannel(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendAsync(
    ReminderNotificationEvent notification,
    CancellationToken cancellationToken)
        {
            var recipient = notification.RecipientEmail
                ?? "test@example.com";

            var subject = $"Reminder: {notification.TodoTitle}";

            var body = $"""
        Your todo reminder:

        {notification.TodoTitle}

        Due at: {notification.DueAt:u}
        """;

            await _emailSender.SendAsync(
                recipient,
                subject,
                body,
                cancellationToken);
        }
    }
}
