using CommandLine;

namespace TorrentExtractor.ConsoleApp.Models
{
    public class TorrentOptions
    {
        // Required options
        [Option('d', "directory", Required = true, HelpText = "Directory where files are saved")]
        public string SourceDirectory { get; set; }

        // Optional options
        [Option('n', "name", HelpText = "Name of downloaded file(for single file torrents)")]
        public string FileName { get; set; }

        [Option('t', "title", HelpText = "Title of torrent")]
        public string Title { get; set; }

        [Option('p', "previous-state", HelpText = "Previous state of torrent")]
        public string PreviousState { get; set; }

        [Option('l', "label", HelpText = "Label")]
        public string Label { get; set; }

        [Option('r', "tracker", HelpText = "Tracker")]
        public string Tracker { get; set; }

        [Option('m', "status", HelpText = "Status message string (same as status column)")]
        public string Status { get; set; }

        [Option('i', "hash", HelpText = "The hex encoded info hash.")]
        public string HexInfoHash { get; set; }

        [Option('s', "state", HelpText = "The state of the downloaded file.")]
        public string State { get; set; }

        [Option('k', "kind", HelpText = "The kind of the downloaded file (single|multi).")]
        public string DownloadKind { get; set; }
    }
}