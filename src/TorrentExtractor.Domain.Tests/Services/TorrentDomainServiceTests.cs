using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FluentAssertions;
using TorrentExtractor.Domain.Services;
using Xunit;

namespace TorrentExtractor.Domain.Tests.Services
{
    public class TorrentDomainServiceTests
    {
        [Theory]
        [ClassData(typeof(FileNameData))]
        public void FormatSeasonAndEpisode_Success(string fileName, string expectedFileName)
        {
            // Arrange
            var target = new TorrentDomainService();

            // Act
            var formattedFilename = target.GetFormattedTvShowFileName(fileName);

            // Assert
            formattedFilename.Should().Be(expectedFileName);
        }
    }

    public class FileNameData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "tv.show.5x02.hdtv-lol.mp4", "tv.show.s05e02.hdtv-lol.mp4" };
            yield return new object[] { "tv.show.1101.hdtv-lol.mp4", "tv.show.s11e01.hdtv-lol.mp4" };
            yield return new object[] { "tv.show.s09e01.hdtv-lol.mp4", "tv.show.s09e01.hdtv-lol.mp4" };
            yield return new object[] { "tv.show.s10e01.hdtv-lol.mp4", "tv.show.s10e01.hdtv-lol.mp4" };
            yield return new object[] { "tv.show.901.hdtv-lol.mp4", "tv.show.s09e01.hdtv-lol.mp4" };
            yield return new object[] { "tv.show.s01e01e02.hdtv-lol.mp4", "tv.show.s01e01e02.hdtv-lol.mp4" };
            yield return new object[] { "tv.show.5x12.hdtv-lol.mp4", "tv.show.s05e12.hdtv-lol.mp4" };
            yield return new object[] { "tv.show.15x12.hdtv-lol.mp4", "tv.show.s15e12.hdtv-lol.mp4" };
            yield return new object[] { "tv.show.2016.s05e07.hdtv-lol.mp4", "tv.show.2016.s05e07.hdtv-lol.mp4" };
            yield return new object[] { "tv.show.2016.507.hdtv-lol.mp4", "tv.show.2016.s05e07.hdtv-lol.mp4" };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}