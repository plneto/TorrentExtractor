namespace TorrentExtractor.Core.Models
{
    public class Torrent
    {
        public string Path { get; set; }
        
        public bool IsSingleFile { get; set; }
        
        public bool IsTvShow { get; set; }
    }
}