using CommandLine;

namespace TorrentExtractor.ConsoleApp.Models
{
    public class TorrentOptions
    {
        // Example: "C:\Apps\TorrentExtractor\TorrentExtractor.ConsoleApp.exe" -n "%N" -l "%L" -f "%F" -r "%R" -d "%D" -c "%C" -z "%Z" -t "%T" -i "%I"
        //
        // qBittorrent options
        //
        // %N - Torrent name
        // %L - Category
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

        [Option('r', "rootpath", HelpText = "Root path (first torrent subdirectory path)")]
        public string RootPath { get; set; }

        [Option('d', "savepath", HelpText = "Save path")]
        public string SavePath { get; set; }

        [Option('c', "numberoffiles", HelpText = "Number of files")]
        public int NumberOfFiles { get; set; }

        [Option('z', "torrentsize", HelpText = "Torrent size (bytes)")]
        public int TorrentSize { get; set; }

        [Option('t', "currenttracker", HelpText = "Current tracker")]
        public string CurrentTracker { get; set; }

        [Option('i', "infohash", HelpText = "Info hash")]
        public string InfoHash { get; set; }
    }
}