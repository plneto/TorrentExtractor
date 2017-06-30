using Autofac;
using TorrentExtractor.ConsoleApp.Infrastructure;

namespace TorrentExtractor.ConsoleApp.Autofac
{
    public class NotificationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<NotificationService>().As<INotificationService>();
        }
    }
}