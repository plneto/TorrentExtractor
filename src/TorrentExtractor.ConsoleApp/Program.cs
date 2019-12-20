using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using MediatR;
using Serilog;
using TorrentExtractor.Core.Helpers;
using TorrentExtractor.Core.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TorrentExtractor.ConsoleApp.Helpers;
using TorrentExtractor.ConsoleApp.Infrastructure;
using TorrentExtractor.ConsoleApp.Models;
using TorrentExtractor.Core.Settings;

namespace TorrentExtractor.ConsoleApp
{
    internal class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        public static IServiceProvider ServiceProvider { get; set; }

        private static void Main(string[] args)
        {
            Init();

            var torrentSettings = ServiceProvider.GetService<TorrentSettings>();
            var emailSettings = ServiceProvider.GetService<EmailSettings>();
            var fileHandler = ServiceProvider.GetService<IFileHandler>();
            var fileFinder = ServiceProvider.GetService<IFileFinder>();
            var notify = ServiceProvider.GetService<INotificationService>();

            Log.Debug("Program Started");

            try
            {
                var torrentOptions = GetTorrentOptions(args);

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
                Log.Error(ex, "Error Extracting Files");
            }

            Log.Debug("Program Finished!");
        }

        private static void Init()
        {
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = configBuilder.Build();

            var logFilePath = Configuration["LoggingSettings:LogFilePath"];

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(logFilePath)
                .WriteTo.ColoredConsole()
                .CreateLogger();

            Log.Information("Logger online");

            var torrentSettings = new TorrentSettings();
            var emailSettings = new EmailSettings();
            var loggingSettings = new LoggingSettings();

            Configuration.GetSection(nameof(TorrentSettings)).Bind(torrentSettings);
            Configuration.GetSection(nameof(EmailSettings)).Bind(emailSettings);
            Configuration.GetSection(nameof(LoggingSettings)).Bind(loggingSettings);

            IServiceCollection services = new ServiceCollection();

            services.AddSingleton(ctx => torrentSettings);
            services.AddSingleton(ctx => emailSettings);
            services.AddSingleton(ctx => loggingSettings);

            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient<IFileFinder, FileFinder>();
            services.AddTransient<IFileFormatter, FileFormatter>();
            services.AddTransient<IFileHandler, FileHandler>();
            services.AddTransient<INotificationService, NotificationService>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public static TorrentOptions GetTorrentOptions(string[] args)
        {
            args = ArgsHelper.RemoveEmptyArgs(args);

            if (args.Length > 0)
                Log.Debug("Args: {0}", string.Join(" ", args));
            else
                Log.Information("No args provided");

            var options = Parser.Default.ParseArguments<TorrentOptions>(args);

            if (options.Tag == ParserResultType.NotParsed)
            {
                Log.Error("Error parsing arguments");

                var errors = new List<Error>();
                options.WithNotParsed(x => errors = x.ToList());

                foreach (var error in errors)
                {
                    Log.Error(error.ToString());
                }

                throw new Exception("Failed to parse arguments");
            }

            var torrentOptions = new TorrentOptions();

            options.WithParsed(x => torrentOptions = x);

            return torrentOptions;
        }

        private static bool IsFile(string path)
        {
            var attr = File.GetAttributes(path);

            return !attr.HasFlag(FileAttributes.Directory);
        }
    }
}