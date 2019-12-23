using FluentAssertions;
using TorrentExtractor.Domain.AggregateModels.TorrentAggregate;
using Xunit;

namespace TorrentExtractor.Domain.Tests.AggregateModels.TorrentAggregate
{
    public class TorrentFileTests
    {
        [Fact]
        public void TorrentFile_ValidParameters_Success()
        {
            // Arrange
            const string path = "C:\\path\\file.rar";

            // Act
            var torrentFile = new TorrentFile(path);

            // Assert
            torrentFile.Path.Should().Be(path);
        }

        [Fact]
        public void TorrentFile_ValidPath_ReturnCorrectExtension()
        {
            // Arrange
            const string path = "C:\\path\\file.rar";

            // Act
            var torrentFile = new TorrentFile(path);

            // Assert
            torrentFile.Extension.Should().Be(".rar");
        }

        [Fact]
        public void TorrentFile_ValidPath_ReturnCorrectFileName()
        {
            // Arrange
            const string path = "C:\\path\\file.rar";

            // Act
            var torrentFile = new TorrentFile(path);

            // Assert
            torrentFile.FileName.Should().Be("file.rar");
        }
    }
}