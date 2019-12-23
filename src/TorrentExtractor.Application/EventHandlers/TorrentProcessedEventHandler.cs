using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TorrentExtractor.Core.Infrastructure;
using TorrentExtractor.Core.Models;
using TorrentExtractor.Core.Settings;
using TorrentExtractor.Domain.Events;

namespace TorrentExtractor.Application.EventHandlers
{
    public class TorrentProcessedEventHandler : INotificationHandler<TorrentProcessedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly EmailSettings _emailSettings;

        public TorrentProcessedEventHandler(INotificationService notificationService, EmailSettings emailSettings)
        {
            _notificationService = notificationService;
            _emailSettings = emailSettings;
        }

        public Task Handle(TorrentProcessedEvent notification, CancellationToken cancellationToken)
        {
            _notificationService.SendEmail(new Email
            {
                Recipients = _emailSettings.ToAddresses,
                Subject = $"Download Finished - {notification.Name}",
                Body = $"File(s) moved to {notification.Destination}"
            });

            return Task.CompletedTask;
        }
    }
}