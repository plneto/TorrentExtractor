using System.Collections.Generic;
using System.Linq;
using TorrentExtractor.Domain.Infrastructure;

namespace TorrentExtractor.Domain
{
    public class Torrent : Entity, IAggregateRoot
    {
        public Torrent(IEnumerable<TorrentFile> files, string label)
        {
            Files = files;
            Label = label;
        }

        public IEnumerable<TorrentFile> Files { get; private set; }

        public string Label { get; private set; }

        public bool IsSingleFile => Files.Count() == 1;

        public bool IsTvShow => Label.ToLower() == "tvshow";

        public bool IsMovie => Label.ToLower() == "movie";
    }
}