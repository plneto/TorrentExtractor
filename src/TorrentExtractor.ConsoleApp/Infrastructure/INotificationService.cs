using TorrentExtractor.ConsoleApp.Models;

namespace TorrentExtractor.ConsoleApp.Infrastructure
{
    public interface INotificationService
    {
        void SendEmail(Email email);
    }
}