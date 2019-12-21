using System.Collections.Generic;
using System.Threading.Tasks;

namespace TorrentExtractor.Core.Infrastructure
{
    public interface IFileFinder
    {
        List<string> FindMediaFiles(string path);

        List<string> FindCompressedFiles(string path);

        bool IsMediaFile(string file);
    }
}