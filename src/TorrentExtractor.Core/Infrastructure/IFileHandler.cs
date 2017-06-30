using TorrentExtractor.Core.Models;

namespace TorrentExtractor.Core.Infrastructure
{
    public interface IFileHandler
    {
        bool CopyFile(string fileOrigin, string destination, Torrent torrentDetails);

        bool ExtractFile(string file, string destinationDirectory, Torrent torrentDetails);
    }
}