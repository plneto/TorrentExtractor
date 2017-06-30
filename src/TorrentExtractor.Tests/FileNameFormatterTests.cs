using FluentAssertions;
using Moq;
using Serilog;
using TorrentExtractor.Core.Infrastructure;
using Xunit;

namespace TorrentExtractor.Tests
{
    public class FileNameFormatterTests
    {
        public Mock<ILogger> MockLogger;

        public FileNameFormatterTests()
        {
            MockLogger = new Mock<ILogger>();
        }

        [Fact]
        public void FormatSeasonAndEpisode_Success()
        {
            var fileFormatter = new FileFormatter(MockLogger.Object);

            var formattedFileName1 = fileFormatter.FormatTvShowFileName("tv.show.5x02.hdtv-lol.mp4");
            var formattedFileName2 = fileFormatter.FormatTvShowFileName("tv.show.1101.hdtv-lol.mp4");
            var formattedFileName3 = fileFormatter.FormatTvShowFileName("tv.show.s09e01.hdtv-lol.mp4");
            var formattedFileName4 = fileFormatter.FormatTvShowFileName("tv.show.s10e01.hdtv-lol.mp4");
            var formattedFileName5 = fileFormatter.FormatTvShowFileName("tv.show.901.hdtv-lol.mp4");
            var formattedFileName6 = fileFormatter.FormatTvShowFileName("tv.show.s01e01e02.hdtv-lol.mp4");
            var formattedFileName7 = fileFormatter.FormatTvShowFileName("tv.show.5x12.hdtv-lol.mp4");
            var formattedFileName8 = fileFormatter.FormatTvShowFileName("tv.show.15x12.hdtv-lol.mp4");
            var formattedFileName9 = fileFormatter.FormatTvShowFileName("tv.show.2016.s05e07.hdtv-lol.mp4");
            var formattedFileName10 = fileFormatter.FormatTvShowFileName("tv.show.2016.507.hdtv-lol.mp4");

            formattedFileName1.Should().BeEquivalentTo("tv.show.s05e02.hdtv-lol.mp4");
            formattedFileName2.Should().BeEquivalentTo("tv.show.s11e01.hdtv-lol.mp4");
            formattedFileName3.Should().BeEquivalentTo("tv.show.s09e01.hdtv-lol.mp4");
            formattedFileName4.Should().BeEquivalentTo("tv.show.s10e01.hdtv-lol.mp4");
            formattedFileName5.Should().BeEquivalentTo("tv.show.s09e01.hdtv-lol.mp4");
            formattedFileName6.Should().BeEquivalentTo("tv.show.s01e01e02.hdtv-lol.mp4");
            formattedFileName7.Should().BeEquivalentTo("tv.show.s05e12.hdtv-lol.mp4");
            formattedFileName8.Should().BeEquivalentTo("tv.show.s15e12.hdtv-lol.mp4");
            formattedFileName9.Should().BeEquivalentTo("tv.show.2016.s05e07.hdtv-lol.mp4");
            formattedFileName10.Should().BeEquivalentTo("tv.show.2016.s05e07.hdtv-lol.mp4");
        }
    }
}
