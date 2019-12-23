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
using TorrentExtractor.Domain.AggregateModels.TorrentAggregate;
using TorrentExtractor.Domain.Events;
using TorrentExtractor.Domain.Extensions;
using TorrentExtractor.Domain.Services;

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
            private readonly TorrentSettings _torrentSettings;
            private readonly ITorrentDomainService _torrentDomainService;

            public Handler(
                IMediator mediator,
                IFileHandler fileHandler,
                TorrentSettings torrentSettings,
                ITorrentDomainService torrentDomainService)
            {
                _mediator = mediator;
                _fileHandler = fileHandler;
                _torrentSettings = torrentSettings;
                _torrentDomainService = torrentDomainService;
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
                    _fileHandler.CopyFile(mediaFile.Path, destinationFolder);

                    if (!torrent.IsTvShow) continue;

                    var newDestination = Path.Combine(destinationFolder, mediaFile.FileName);
                    var formattedName = _torrentDomainService.GetFormattedTvShowFileName(mediaFile.FileName);

                    _fileHandler.RenameFile(newDestination, formattedName);
                }

                foreach (var compressedFile in torrent.CompressedFiles)
                {
                    _fileHandler.ExtractFile(compressedFile.Path, destinationFolder);

                    if (!torrent.IsTvShow) continue;

                    var rarArchiveFiles = _fileHandler.GetRarArchiveFilenames(compressedFile.Path);

                    foreach (var rarArchiveFile in rarArchiveFiles)
                    {
                        var extractedFile = Path.Combine(destinationFolder, rarArchiveFile);
                        var formattedName = _torrentDomainService.GetFormattedTvShowFileName(rarArchiveFile);
                        _fileHandler.RenameFile(extractedFile, formattedName);
                    }
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