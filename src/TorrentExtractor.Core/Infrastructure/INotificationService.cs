using TorrentExtractor.Core.Models;

namespace TorrentExtractor.Core.Infrastructure
{
    public interface INotificationService
    {
        void SendEmail(Email email);
    }
}