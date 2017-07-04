using System.Collections.Generic;

namespace TorrentExtractor.Core.Settings
{
    public class TorrentSettings
    {
        public string TvShowsLabel { get; set; }
        public string MoviesLabel { get; set; }
        public string DefaultLabel { get; set; }
        public string TvShowsDirectory { get; set; }
        public string MoviesDirectory { get; set; }
        public List<string> SupportedFileFormats { get; set; }
        public string DownloadsDirectory { get; set; }
    }
}
