using Autofac;
using TorrentExtractor.Core.Infrastructure;

namespace TorrentExtractor.Core.Autofac
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<FileFinder>().As<IFileFinder>();
            builder.RegisterType<FileFormatter>().As<IFileFormatter>();
            builder.RegisterType<FileHandler>().As<IFileHandler>();
        }
    }
}
