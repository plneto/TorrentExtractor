using System.Collections.Generic;

namespace TorrentExtractor.Core.Settings
{
    public class TorrentSettings
    {
        public string TvShowsCategory { get; set; }
        public string MoviesCategory { get; set; }
        public string DefaultCategory { get; set; }
        public string TvShowsDirectory { get; set; }
        public string MoviesDirectory { get; set; }
        public List<string> SupportedFileFormats { get; set; }
        public string DownloadsDirectory { get; set; }
    }
}
