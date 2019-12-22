using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TorrentExtractor.Core.Infrastructure;
using TorrentExtractor.Core.Models;
using TorrentExtractor.Core.Settings;
using TorrentExtractor.Domain;

namespace TorrentExtractor.Application.Commands
{
    public class TorrentProcessDownload
    {
        public class Command : IRequest
        {
            public Command(string category, string contentPath, string torrentName)
            {
                Category = category;
                ContentPath = contentPath;
                TorrentName = torrentName;
            }

            public string Category { get; }

            public string ContentPath { get; }

            public string TorrentName { get; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IFileHandler _fileHandler;
            private readonly INotificationService _notificationService;
            private readonly TorrentSettings _torrentSettings;
            private readonly EmailSettings _emailSettings;

            public Handler(
                IFileHandler fileHandler,
                INotificationService notificationService,
                TorrentSettings torrentSettings,
                EmailSettings emailSettings)
            {
                _fileHandler = fileHandler;
                _notificationService = notificationService;
                _torrentSettings = torrentSettings;
                _emailSettings = emailSettings;
            }

            public Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var files = GetFiles(request.ContentPath);

                var torrent = new Torrent(request.TorrentName, files, request.Category);

                if (!torrent.IsTvShow && !torrent.IsMovie)
                {
                    return Task.FromResult(Unit.Value);
                }

                var destinationFolder = torrent.IsTvShow
                    ? _torrentSettings.TvShowsDirectory
                    : _torrentSettings.MoviesDirectory;

                foreach (var mediaFile in torrent.MediaFiles)
                {
                    _fileHandler.CopyFile(mediaFile.Path, destinationFolder, torrent.IsTvShow);
                }

                foreach (var compressedFile in torrent.CompressedFiles)
                {
                    _fileHandler.ExtractFile(compressedFile.Path, destinationFolder, torrent.IsTvShow);
                }

                _notificationService.SendEmail(new Email
                {
                    Recipients = _emailSettings.ToAddresses,
                    Subject = $"Download Finished - {torrent.Name}",
                    Body = $"File(s) moved to {destinationFolder}"
                });

                return Task.FromResult(Unit.Value);
            }

            private static IEnumerable<TorrentFile> GetFiles(string path)
            {
                var files = new List<TorrentFile>();

                if (File.Exists(path))
                {
                    files.Add(new TorrentFile(path));
                }
                else if (Directory.Exists(path))
                {
                    var torrentFiles = Directory.EnumerateFiles(
                        path,
                        "*.*",
                        SearchOption.AllDirectories);

                    files.AddRange(torrentFiles.Select(torrentFile => new TorrentFile(torrentFile)));
                }

                return files;
            }
        }
    }
}