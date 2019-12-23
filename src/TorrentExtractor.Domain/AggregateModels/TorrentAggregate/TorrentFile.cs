using System.Collections.Generic;
using TorrentExtractor.Domain.Infrastructure;

namespace TorrentExtractor.Domain.AggregateModels.TorrentAggregate
{
    public class TorrentFile : ValueObject
    {
        public TorrentFile(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public string FileName => System.IO.Path.GetFileName(Path);

        public string Extension => System.IO.Path.GetExtension(Path);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Path;
        }
    }
}