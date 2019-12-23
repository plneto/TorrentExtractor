using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using TorrentExtractor.Domain.AggregateModels.TorrentAggregate;
using Xunit;

namespace TorrentExtractor.Domain.Tests.AggregateModels.TorrentAggregate
{
    public class TorrentTests
    {
        private readonly Fixture _fixture;

        public TorrentTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Torrent_ValidParameters_Success()
        {
            // Arrange
            var name = _fixture.Create<string>();
            var files = _fixture.CreateMany<TorrentFile>().ToList();
            var label = _fixture.Create<string>();

            // Act
            var torrent = new Torrent(name, files, label);

            // Assert
            torrent.Name.Should().Be(name);
            torrent.Files.Should().BeEquivalentTo(files);
            torrent.Label.Should().Be(label);
        }

        [Fact]
        public void Torrent_TvShowLabel_IsTvShowTrue()
        {
            // Arrange
            var name = _fixture.Create<string>();
            var files = _fixture.CreateMany<TorrentFile>().ToList();
            const string label = "tvshow";

            // Act
            var torrent = new Torrent(name, files, label);

            // Assert
            torrent.IsTvShow.Should().BeTrue();
        }

        [Fact]
        public void Torrent_MovieLabel_IsMovieTrue()
        {
            // Arrange
            var name = _fixture.Create<string>();
            var files = _fixture.CreateMany<TorrentFile>().ToList();
            const string label = "movie";

            // Act
            var torrent = new Torrent(name, files, label);

            // Assert
            torrent.IsMovie.Should().BeTrue();
        }

        [Fact]
        public void Torrent_SampleFiles_NotIncluded()
        {
            // Arrange
            const string videoFile = "c:\\path\\file.mkv";
            const string sampleFile = "c:\\path\\sample.mkv";

            var name = _fixture.Create<string>();
            var files = new List<TorrentFile>
            {
                new TorrentFile(videoFile),
                new TorrentFile(sampleFile)
            };
            var label = _fixture.Create<string>();

            // Act
            var torrent = new Torrent(name, files, label);

            // Assert
            torrent.Files.Should().HaveCount(1);
            torrent.Files.Single().Path.Should().Be(videoFile);
        }

        [Fact]
        public void Torrent_CompressedAndMediaFiles_ReturnCorrectValues()
        {
            // Arrange
            const string videoFile = "c:\\path\\file.mkv";
            const string compressedFile = "c:\\path\\file.rar";

            var name = _fixture.Create<string>();
            var files = new List<TorrentFile>
            {
                new TorrentFile(videoFile),
                new TorrentFile(compressedFile)
            };
            var label = _fixture.Create<string>();

            // Act
            var torrent = new Torrent(name, files, label);

            // Assert
            torrent.Files.Should().HaveCount(2);
            torrent.MediaFiles.Should().HaveCount(1);
            torrent.MediaFiles.Single().Path.Should().Be(videoFile);
            torrent.CompressedFiles.Should().HaveCount(1);
            torrent.CompressedFiles.Single().Path.Should().Be(compressedFile);
        }
    }
}