using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using CommandLine;
using Serilog;
using TorrentExtractor.Core.Autofac;
using TorrentExtractor.Core.Helpers;
using TorrentExtractor.Core.Infrastructure;
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
                    logger.Debug("Args: {0}", string.Join(" ", args));
                else
                    logger.Information("No args provided");

                var options = Parser.Default.ParseArguments<TorrentOptions>(args);

                if (options.Tag == ParserResultType.NotParsed)
                {
                    logger.Error("Error parsing arguments");

                    var errors = new List<Error>();
                    options.WithNotParsed(x => errors = x.ToList());

                    foreach (var error in errors)
                    {
                        logger.Error(error.ToString());
                    }

                    logger.Information("Program Exited");
                    return;
                }

                var torrentOptions = new TorrentOptions();

                options.WithParsed(x => torrentOptions = x);

                var isTvShow = torrentOptions.Category == torrentSettings.TvShowsCategory
                               || (string.IsNullOrWhiteSpace(torrentOptions.Category)
                                   && torrentSettings.DefaultCategory == torrentSettings.TvShowsCategory);

                var path = torrentOptions.ContentPath;

                var isSingleFile = IsFile(torrentOptions.ContentPath);

                var destinationFolder = isTvShow
                    ? torrentSettings.TvShowsDirectory
                    : torrentSettings.MoviesDirectory;

                if (isSingleFile)
                {
                    if (FileHelper.IsMediaFile(torrentOptions.ContentPath, torrentSettings.SupportedFileFormats))
                        fileHandler.CopyFile(torrentOptions.ContentPath, destinationFolder, isTvShow);
                    else
                        fileHandler.ExtractFile(torrentOptions.ContentPath, destinationFolder, isTvShow);
                }
                else
                {
                    var compressedFiles = fileFinder.FindCompressedFiles(torrentOptions.ContentPath);

                    foreach (var file in compressedFiles)
                    {
                        fileHandler.ExtractFile(file, destinationFolder, isTvShow);
                    }

                    var mediaFiles = fileFinder.FindMediaFiles(torrentOptions.ContentPath, torrentSettings.SupportedFileFormats);

                    foreach (var file in mediaFiles)
                    {
                        fileHandler.CopyFile(file, destinationFolder, isTvShow);
                    }
                }

                var downloadName = string.IsNullOrWhiteSpace(torrentOptions.TorrentName)
                    ? torrentOptions.ContentPath.Substring(torrentOptions.ContentPath.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                    : torrentOptions.TorrentName;

                notify.SendEmail(new Email
                {
                    Recipients = emailSettings.ToAddresses,
                    Subject = $"Download Finished - {downloadName}",
                    Body = $"File(s) moved to {(isTvShow ? torrentSettings.TvShowsDirectory : torrentSettings.MoviesDirectory)}"
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

        private static bool IsFile(string path)
        {
            var attr = File.GetAttributes(path);

            return !attr.HasFlag(FileAttributes.Directory);
        }
    }
}