using System.Collections.Generic;
using System.Linq;
using TorrentExtractor.Domain.Infrastructure;

namespace TorrentExtractor.Domain.AggregateModels.TorrentAggregate
{
    public class Torrent : Entity, IAggregateRoot
    {
        private const string TvShowLabel = "tvshow";
        private const string MovieLabel = "movie";
        private readonly string[] _supportedCompressionFormats = { ".rar" };
        private readonly string[] _supportedMediaFormats = { ".avi", ".mp4", ".mkv" };

        public Torrent(string name, IEnumerable<TorrentFile> files, string label)
        {
            Name = name;
            Files = files.Where(x => !x.Path.ToLower().Contains("sample"));
            Label = label;
        }

        public string Name { get; private set; }

        public IEnumerable<TorrentFile> Files { get; private set; }

        public string Label { get; private set; }

        public bool IsTvShow => Label.ToLower() == TvShowLabel;

        public bool IsMovie => Label.ToLower() == MovieLabel;

        public IEnumerable<TorrentFile> CompressedFiles => Files
            .Where(x => _supportedCompressionFormats.Contains(x.Extension));

        public IEnumerable<TorrentFile> MediaFiles => Files
            .Where(x => _supportedMediaFormats.Contains(x.Extension));
    }
}