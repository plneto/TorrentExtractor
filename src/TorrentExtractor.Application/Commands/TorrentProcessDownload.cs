using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TorrentExtractor.Core.Helpers;
using TorrentExtractor.Core.Infrastructure;
using TorrentExtractor.Core.Models;
using TorrentExtractor.Core.Settings;

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
            private readonly IFileFinder _fileFinder;
            private readonly INotificationService _notificationService;
            private readonly TorrentSettings _torrentSettings;
            private readonly EmailSettings _emailSettings;

            public Handler(
                IFileHandler fileHandler,
                IFileFinder fileFinder,
                INotificationService notificationService,
                TorrentSettings torrentSettings,
                EmailSettings emailSettings)
            {
                _fileHandler = fileHandler;
                _fileFinder = fileFinder;
                _notificationService = notificationService;
                _torrentSettings = torrentSettings;
                _emailSettings = emailSettings;
            }

            public Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var isTvShow = request.Category == _torrentSettings.TvShowsCategory
                               || (string.IsNullOrWhiteSpace(request.Category)
                                   && _torrentSettings.DefaultCategory == _torrentSettings.TvShowsCategory);

                var isSingleFile = IsFile(request.ContentPath);

                var destinationFolder = isTvShow
                    ? _torrentSettings.TvShowsDirectory
                    : _torrentSettings.MoviesDirectory;

                if (isSingleFile)
                {
                    if (FileHelper.IsMediaFile(request.ContentPath, _torrentSettings.SupportedFileFormats))
                        _fileHandler.CopyFile(request.ContentPath, destinationFolder, isTvShow);
                    else
                        _fileHandler.ExtractFile(request.ContentPath, destinationFolder, isTvShow);
                }
                else
                {
                    var compressedFiles = _fileFinder.FindCompressedFiles(request.ContentPath);

                    foreach (var file in compressedFiles)
                    {
                        _fileHandler.ExtractFile(file, destinationFolder, isTvShow);
                    }

                    var mediaFiles = _fileFinder.FindMediaFiles(request.ContentPath, _torrentSettings.SupportedFileFormats);

                    foreach (var file in mediaFiles)
                    {
                        _fileHandler.CopyFile(file, destinationFolder, isTvShow);
                    }
                }

                var downloadName = string.IsNullOrWhiteSpace(request.TorrentName)
                    ? request.ContentPath.Substring(request.ContentPath.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                    : request.TorrentName;

                _notificationService.SendEmail(new Email
                {
                    Recipients = _emailSettings.ToAddresses,
                    Subject = $"Download Finished - {downloadName}",
                    Body = $"File(s) moved to {(isTvShow ? _torrentSettings.TvShowsDirectory : _torrentSettings.MoviesDirectory)}"
                });

                return Task.FromResult(Unit.Value);
            }

            private static bool IsFile(string path)
            {
                var attr = File.GetAttributes(path);

                return !attr.HasFlag(FileAttributes.Directory);
            }
        }
    }
}