using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives;

namespace TorrentExtractor.Core.Infrastructure
{
    public class FileHandler : IFileHandler
    {
        public void CopyFile(string filePath, string destinationFolder)
        {
            Log.Debug($"Enter CopyFile -> " +
                      $"File Path: {filePath}, Destination Folder: {destinationFolder}");

            var fileName = Path.GetFileName(filePath);

            try
            {
                File.Copy(filePath,
                    $@"{destinationFolder}\{fileName}",
                    false);

                Log.Debug($"File {fileName} copied to {destinationFolder}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error Copying File");

                if (!ex.Message.Contains("already exists"))
                    throw;
            }

            Log.Debug("Exit CopyFile");
        }

        public void ExtractFile(string filePath, string destinationFolder)
        {
            Log.Debug("Enter ExtractFile");

            using (var rarFile = RarArchive.Open(filePath))
            {
                foreach (var archiveEntry in rarFile.Entries)
                {
                    try
                    {
                        archiveEntry.WriteToDirectory(destinationFolder);
                        Log.Debug($"File {filePath} extracted to {destinationFolder}");
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
        }

        public void RenameFile(string filePath, string newFilename)
        {
            Log.Debug($"Enter RenameFile -> File Path: {filePath}");

            var directory = Path.GetDirectoryName(filePath);

            try
            {
                File.Move(filePath,
                    $@"{directory}\{newFilename}");

                Log.Debug($"File {Path.GetFileName(filePath)} renamed to {newFilename}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error Renaming File");

                if (!ex.Message.Contains("already exists"))
                    throw;
            }

            Log.Debug("Exit RenameFile");
        }

        public IEnumerable<string> GetRarArchiveFilenames(string filePath)
        {
            using var rarFile = RarArchive.Open(filePath);

            return rarFile.Entries.Select(x => x.Key);
        }
    }
}