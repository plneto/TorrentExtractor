using Autofac;
using Microsoft.Extensions.Configuration;
using TorrentExtractor.Core.Settings;

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

            var torrentSettings = new TorrentSettings();
            var emailSettings = new EmailSettings();
            var loggingSettings = new LoggingSettings();

            _configuration.GetSection(nameof(TorrentSettings)).Bind(torrentSettings);
            _configuration.GetSection(nameof(EmailSettings)).Bind(emailSettings);
            _configuration.GetSection(nameof(LoggingSettings)).Bind(loggingSettings);

            builder.Register(ctx => torrentSettings)
                .AsSelf()
                .SingleInstance();

            builder.Register(ctx => emailSettings)
                .AsSelf()
                .SingleInstance();

            builder.Register(ctx => loggingSettings)
                .AsSelf()
                .SingleInstance();
        }
    }
}
