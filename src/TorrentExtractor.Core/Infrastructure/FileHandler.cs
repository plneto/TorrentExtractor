using System;
using System.IO;
using Serilog;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives;

namespace TorrentExtractor.Core.Infrastructure
{
    public class FileHandler : IFileHandler
    {
        private readonly IFileFormatter _fileFormatter;

        public FileHandler(IFileFormatter fileFormatter)
        {
            _fileFormatter = fileFormatter;
        }

        public bool CopyFile(string fileOrigin, string destination, bool isTvShow)
        {
            Log.Debug("Enter CopyFile");
            Log.Debug($"Origin: {fileOrigin}, Destination: {destination}");

            var formattedFileName = isTvShow
                ? _fileFormatter.FormatTvShowFileName(Path.GetFileName(fileOrigin))
                : Path.GetFileName(fileOrigin);

            var success = false;

            try
            {
                File.Copy(fileOrigin, $@"{destination}\{formattedFileName}", false);
                Log.Debug($"File {formattedFileName} copied to {destination}");

                success = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error Copying File");

                if (!ex.Message.Contains("already exists"))
                    throw;
            }

            Log.Debug("Exit CopyFile");

            return success;
        }

        public bool ExtractFile(string file, string destinationFolder, bool isTvShow)
        {
            Log.Debug("Enter ExtractFile");

            var success = false;

            using (var rarFile = RarArchive.Open(file))
            {
                foreach (var archiveEntry in rarFile.Entries)
                {
                    try
                    {
                        archiveEntry.WriteToDirectory(destinationFolder);
                        Log.Debug($"File {file} extracted to {destinationFolder}");

                        var formattedFileName = isTvShow
                            ? _fileFormatter.FormatTvShowFileName(archiveEntry.Key)
                            : archiveEntry.Key;

                        File.Move($@"{destinationFolder}\{archiveEntry.Key}", $@"{destinationFolder}\{formattedFileName}");

                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error Extracting File");

                        if (!ex.Message.Contains("already exists"))
                            throw;
                    }
                }
            }

            Log.Debug("Exit ExtractFile");

            return success;
        }
    }
}