using System.Collections.Generic;

namespace TorrentExtractor.Core.Infrastructure
{
    public interface IFileHandler
    {
        void CopyFile(string filePath, string destinationFolder);

        void ExtractFile(string filePath, string destinationFolder);

        void RenameFile(string filePath, string newFilename);

        IEnumerable<string> GetRarArchiveFilenames(string filePath);
    }
}