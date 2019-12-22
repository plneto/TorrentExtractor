using System.Collections.Generic;
using TorrentExtractor.Domain.Infrastructure;

namespace TorrentExtractor.Domain
{
    public class TorrentFile : ValueObject
    {
        public TorrentFile(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public string Extension => Path.Substring(Path.LastIndexOf('.'));

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Path;
        }
    }
}