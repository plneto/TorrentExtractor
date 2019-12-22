using CommandLine;

namespace TorrentExtractor.Qbittorrent.ConsoleApp.Models
{
    public class QbittorrentOptions
    {
        // Example: "C:\Apps\TorrentExtractor\TorrentExtractor.Qbittorrent.ConsoleApp.exe" -n "%N" -l "%L" -g "%G" -f "%F" -r "%R" -d "%D" -c "%C" -z "%Z" -t "%T" -i "%I"
        //
        // qBittorrent options
        //
        // %N - Torrent name
        // %L - Category
        // %G - Tags (separated by comma)
        // %F - Content path (same as root path for multifile torrent)
        // %R - Root path (first torrent subdirectory path)
        // %D - Save path
        // %C - Number of files
        // %Z - Torrent size (bytes)
        // %T - Current tracker
        // %I - Info hash

        // Required options
        [Option('f', "contentpath", Required = true, HelpText = "Content path (same as root path for multifile torrent)")]
        public string ContentPath { get; set; }

        // Optional options
        [Option('n', "torrentname", HelpText = "Torrent Name")]
        public string TorrentName { get; set; }

        [Option('l', "category", HelpText = "Category")]
        public string Category { get; set; }

        [Option('g', "tags", HelpText = "Tags (separated by comma)")]
        public string Tags { get; set; }

        [Option('r', "rootpath", HelpText = "Root path (first torrent subdirectory path)")]
        public string RootPath { get; set; }

        [Option('d', "savepath", HelpText = "Save path")]
        public string SavePath { get; set; }

        [Option('c', "numberoffiles", HelpText = "Number of files")]
        public int NumberOfFiles { get; set; }

        [Option('z', "torrentsize", HelpText = "Torrent size (bytes)")]
        public long TorrentSize { get; set; }

        [Option('t', "currenttracker", HelpText = "Current tracker")]
        public string CurrentTracker { get; set; }

        [Option('i', "infohash", HelpText = "Info hash")]
        public string InfoHash { get; set; }
    }
}