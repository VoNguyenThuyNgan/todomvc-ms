using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Notification.Channels.Email
{
    public class FakeEmailSender : IEmailSender
    {
        private readonly ILogger<FakeEmailSender> _logger;

        public FakeEmailSender(
            ILogger<FakeEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(
            string recipient,
            string subject,
            string body,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                """
            ===== FAKE EMAIL =====
            To: {Recipient}
            Subject: {Subject}
            Body:
            {Body}
            ======================
            """,
                recipient,
                subject,
                body);

            return Task.CompletedTask;
        }
    }
}
