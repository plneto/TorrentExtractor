using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Configuration;
using TorrentExtractor.Core.Models;

namespace TorrentExtractor.ConsoleApp.Autofac
{
    public class AppSettingsModule : Module
    {
        private readonly IConfigurationRoot _configuration;

        public AppSettingsModule(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(ctx => new TorrentSettings
            {
                TvShowsLabel = _configuration["TorrentSettings:TvShowsLabel"],
                MoviesLabel = _configuration["TorrentSettings:MoviesLabel"],
                DefaultLabel = _configuration["TorrentSettings:DefaultLabel"],
                TvShowsDirectory = _configuration["TorrentSettings:TvShowsDirectory"],
                MoviesDirectory = _configuration["TorrentSettings:MoviesDirectory"],
                SupportedFileFormats = ParseArray("TorrentSettings:SupportedFileFormats"),
                DownloadsDirectory = _configuration["TorrentSettings:DownloadsDirectory"]
            })
            .AsSelf()
            .SingleInstance();

            builder.Register(ctx => new EmailSettings
            {
                ToAddresses = ParseArray("EmailSettings:ToAddresses"),
                BccAddresses = ParseArray("EmailSettings:BccAddresses"),
                SmtpUsername = _configuration["EmailSettings:SmtpUsername"],
                SmtpPassword = _configuration["EmailSettings:SmtpPassword"],
                SmtpServerAddress = _configuration["EmailSettings:SmtpServerAddress"],
                SmtpServerPort = int.Parse(_configuration["EmailSettings:SmtpServerPort"]),
                SmtpUseSsl = bool.Parse(_configuration["EmailSettings:SmtpUseSsl"]),
                SenderName = _configuration["EmailSettings:SenderName"],
                SenderAddress = _configuration["EmailSettings:SenderAddress"],
                FromPassword = _configuration["EmailSettings:FromPassword"]
            })
                .AsSelf()
                .SingleInstance();

            builder.Register(ctx => new LoggingSettings
            {
                LogFilePath = _configuration["LoggingSettings:LogFilePath"]
            })
            .AsSelf()
            .SingleInstance();
        }

        private List<string> ParseArray(string key)
        {
            var list = new List<string>();

            var count = 0;
            string value;

            while ((value = _configuration[$"{key}:{count}"]) != null)
            {
                list.Add(value);
                count++;
            }

            return list;
        }
    }
}
