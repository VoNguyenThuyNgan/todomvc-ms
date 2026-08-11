using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Notification.Channels.Email
{
    public interface IEmailSender
    {
        Task SendAsync(
            string recipient,
            string subject,
            string body,
            CancellationToken cancellationToken);
    }
}
