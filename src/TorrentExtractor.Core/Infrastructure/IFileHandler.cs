namespace TorrentExtractor.Core.Infrastructure
{
    public interface IFileHandler
    {
        bool CopyFile(string fileOrigin, string destination, bool isTvShow);

        bool ExtractFile(string file, string destinationDirectory, bool isTvShow);
    }
}