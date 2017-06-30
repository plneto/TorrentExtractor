using Autofac;
using AutofacSerilogIntegration;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace TorrentExtractor.ConsoleApp.Autofac
{
    public class LoggerModule : Module
    {
        private readonly IConfigurationRoot _configuration;

        public LoggerModule(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var logFilePath = _configuration["LoggingSettings:LogFilePath"];

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(logFilePath)
                .WriteTo.ColoredConsole()
                .CreateLogger();

            Log.Information("Logger online");

            builder.RegisterLogger();
        }
    }
}
