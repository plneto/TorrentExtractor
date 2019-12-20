namespace TorrentExtractor.Domain
{
    public class Torrent : IAggregateRoot
    {
        public Torrent(string path, bool isSingleFile, bool isTvShow)
        {
            Path = path;
            IsSingleFile = isSingleFile;
            IsTvShow = isTvShow;
        }

        public string Path { get; private set; }

        public bool IsSingleFile { get; private set; }

        public bool IsTvShow { get; private set; }
    }
}