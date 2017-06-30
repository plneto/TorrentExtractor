using System;
using System.IO;
using Serilog;
using TorrentExtractor.Core.Models;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives;

namespace TorrentExtractor.Core.Infrastructure
{
    public class FileHandler : IFileHandler
    {
        private readonly IFileFormatter _fileFormatter;
        private readonly ILogger _logger;

        public FileHandler(IFileFormatter fileFormatter, ILogger logger)
        {
            _fileFormatter = fileFormatter;
            _logger = logger;
        }

        public bool CopyFile(string fileOrigin, string destination, Torrent torrentDetails)
        {
            _logger.Debug("Enter CopyFile");
            _logger.Debug($"Origin: {fileOrigin}, Destination: {destination}");

            var formattedFileName = torrentDetails.IsTvShow
                ?_fileFormatter.FormatTvShowFileName(Path.GetFileName(fileOrigin))
                : Path.GetFileName(fileOrigin);

            var success = false;

            try
            {
                File.Copy(fileOrigin, $@"{destination}\{formattedFileName}", false);
                _logger.Debug($"File {formattedFileName} copied to {destination}");

                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error Copying File");

                if (!ex.Message.Contains("already exists"))
                    throw;
            }

            _logger.Debug("Exit CopyFile");

            return success;
        }

        public bool ExtractFile(string file, string destinationDirectory, Torrent torrentDetails)
        {
            _logger.Debug("Enter ExtractFile");

            var success = false;

            using (var rarFile = RarArchive.Open(file))
            {
                foreach (var archiveEntry in rarFile.Entries)
                {
                    try
                    {
                        archiveEntry.WriteToDirectory(destinationDirectory);
                        _logger.Debug($"File {file} extracted to {destinationDirectory}");
                        
                        var formattedFileName = torrentDetails.IsTvShow
                            ? _fileFormatter.FormatTvShowFileName(archiveEntry.Key) 
                            : archiveEntry.Key;

                        File.Move($@"{destinationDirectory}\{archiveEntry.Key}", $@"{destinationDirectory}\{formattedFileName}");

                        success = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Error Extracting File");

                        if (!ex.Message.Contains("already exists"))
                            throw;
                    }
                }
            }

            _logger.Debug("Exit ExtractFile");

            return success;
        }
    }
}