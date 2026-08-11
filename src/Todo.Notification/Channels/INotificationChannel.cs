using System;
using System.Collections.Generic;
using System.Text;
using Todo.Notification.Contracts;

namespace Todo.Notification.Channels
{
    public interface INotificationChannel
    {
        string ChannelName { get; }

        Task SendAsync(
            ReminderNotificationEvent notification,
            CancellationToken cancellationToken);
    }
}
