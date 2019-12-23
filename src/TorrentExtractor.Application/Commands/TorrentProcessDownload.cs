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
using TorrentExtractor.Domain.Events;
using TorrentExtractor.Domain.Extensions;

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
            private readonly IMediator _mediator;
            private readonly IFileHandler _fileHandler;
            private readonly INotificationService _notificationService;
            private readonly TorrentSettings _torrentSettings;
            private readonly EmailSettings _emailSettings;

            public Handler(
                IMediator mediator,
                IFileHandler fileHandler,
                INotificationService notificationService,
                TorrentSettings torrentSettings,
                EmailSettings emailSettings)
            {
                _mediator = mediator;
                _fileHandler = fileHandler;
                _notificationService = notificationService;
                _torrentSettings = torrentSettings;
                _emailSettings = emailSettings;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var files = GetFiles(request.ContentPath);

                var torrent = new Torrent(request.TorrentName, files, request.Category);

                if (!torrent.IsTvShow && !torrent.IsMovie)
                {
                    return Unit.Value;
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

                torrent.AddDomainEvent(new TorrentProcessedEvent(torrent.Name, destinationFolder));
                await _mediator.DispatchDomainEventsAsync(torrent);

                return Unit.Value;
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