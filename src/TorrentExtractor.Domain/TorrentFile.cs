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

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Path;
        }
    }
}