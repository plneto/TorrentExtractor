using System.Collections.Generic;
using System.Linq;
using TorrentExtractor.Domain.Infrastructure;

namespace TorrentExtractor.Domain
{
    public class Torrent : Entity, IAggregateRoot
    {
        public Torrent(IEnumerable<TorrentFile> files, string label, bool isTvShow)
        {
            Files = files;
            Label = label;
            IsTvShow = isTvShow;
        }

        public IEnumerable<TorrentFile> Files { get; private set; }

        public string Label { get; private set; }

        public bool IsSingleFile => Files.Count() == 1;

        public bool IsTvShow { get; private set; }
    }
}