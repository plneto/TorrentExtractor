using System;
using Autofac;
using CommandLine;
using Serilog;
using TorrentExtractor.Core.Autofac;
using TorrentExtractor.Core.Helpers;
using TorrentExtractor.Core.Infrastructure;
using TorrentExtractor.Core.Models;
using Microsoft.Extensions.Configuration;
using TorrentExtractor.ConsoleApp.Autofac;
using TorrentExtractor.ConsoleApp.Helpers;
using TorrentExtractor.ConsoleApp.Infrastructure;
using TorrentExtractor.ConsoleApp.Models;
using TorrentExtractor.Core.Settings;

namespace TorrentExtractor.ConsoleApp
{
    internal class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            
            var container = InitContainer();

            var torrentSettings = container.Resolve<TorrentSettings>();
            var emailSettings = container.Resolve<EmailSettings>();
            var logger = container.Resolve<ILogger>();
            var fileHandler = container.Resolve<IFileHandler>();
            var fileFinder = container.Resolve<IFileFinder>();
            var notify = container.Resolve<INotificationService>();

            logger.Debug("Program Started");

            try
            {
                args = ArgsHelper.RemoveEmptyArgs(args);

                if (args.Length > 0)
                    logger.Debug("Args: {0}", string.Join(", ", args));
                else
                    logger.Information("No args provided");
                
                var options = Parser.Default.ParseArguments<TorrentOptions>(args);

                if (options.Tag == ParserResultType.NotParsed)
                    return;

                var torrentOptions = new TorrentOptions();

                options.WithParsed(x => torrentOptions = x);

                var torrent = new Torrent
                {
                    IsTvShow = torrentOptions.Label == torrentSettings.TvShowsLabel || (string.IsNullOrWhiteSpace(torrentOptions.Label) && torrentSettings.DefaultLabel == torrentSettings.TvShowsLabel),
                    Path = torrentOptions.SourceDirectory,
                    IsSingleFile = torrentOptions.SourceDirectory == torrentSettings.DownloadsDirectory
                };

                var destinationFolder = torrent.IsTvShow
                    ? torrentSettings.TvShowsDirectory
                    : torrentSettings.MoviesDirectory;
                
                if (torrent.IsSingleFile)
                {
                    if (FileHelper.IsMediaFile(torrentOptions.FileName, torrentSettings.SupportedFileFormats))
                        fileHandler.CopyFile(torrentOptions.FileName, destinationFolder, torrent);
                    else
                        fileHandler.ExtractFile(torrentOptions.FileName, destinationFolder, torrent);
                }
                else
                {
                    var compressedFiles = fileFinder.FindCompressedFiles(torrentOptions.SourceDirectory);

                    foreach (var file in compressedFiles)
                    {
                        fileHandler.ExtractFile(file, destinationFolder, torrent);
                    }

                    var mediaFiles = fileFinder.FindMediaFiles(torrentOptions.SourceDirectory, torrentSettings.SupportedFileFormats);

                    foreach (var file in mediaFiles)
                    {
                        fileHandler.CopyFile(file, destinationFolder, torrent);
                    }
                }

                var downloadName = string.IsNullOrWhiteSpace(torrentOptions.Title)
                    ? torrentOptions.SourceDirectory.Substring(torrentOptions.SourceDirectory.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                    : torrentOptions.Title;

                notify.SendEmail(new Email
                {
                    Recipients = emailSettings.ToAddresses,
                    Subject = $"Download Finished - {downloadName}",
                    Body = $"File(s) moved to {(torrent.IsTvShow ? torrentSettings.TvShowsDirectory : torrentSettings.MoviesDirectory)}"
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error Extracting Files");
            }

            logger.Debug("Program Finished!");
        }

        private static IContainer InitContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new CoreModule());
            builder.RegisterModule(new AppSettingsModule(Configuration));
            builder.RegisterModule(new LoggerModule(Configuration));
            builder.RegisterModule(new NotificationModule());

            return builder.Build();
        }
    }
}