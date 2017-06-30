using System.Collections.Generic;

namespace TorrentExtractor.Core.Infrastructure
{
    public interface IFileFinder
    {
        List<string> FindMediaFiles(string path, List<string> supportedMediaTypes);

        List<string> FindCompressedFiles(string path);
    }
}