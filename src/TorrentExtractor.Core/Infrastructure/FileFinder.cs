using System.Collections.Generic;
using System.IO;
using System.Linq;
using TorrentExtractor.Core.Settings;

namespace TorrentExtractor.Core.Infrastructure
{
    public class FileFinder : IFileFinder
    {
        private readonly TorrentSettings _torrentSettings;

        public FileFinder(TorrentSettings torrentSettings)
        {
            _torrentSettings = torrentSettings;
        }

        public List<string> FindMediaFiles(string path)
        {
            return GetMediaFiles(path, new List<string>());
        }

        public List<string> FindCompressedFiles(string path)
        {
            return GetRarFiles(path, new List<string>());
        }

        public bool IsMediaFile(string file)
        {
            return _torrentSettings.SupportedFileFormats
                    .Contains(file.Substring(file.LastIndexOf('.')));
        }

        private List<string> GetMediaFiles(string directory, List<string> list)
        {
            // get media files from main directory
            list.AddRange(Directory.GetFiles(directory)
                .Where(file => IsMediaFile(file) && !file.ToLower().Contains("sample")).ToList());

            // get media files from sub directories recursively
            foreach (var dir in Directory.GetDirectories(directory))
            {
                list.AddRange(from subDir in Directory.GetDirectories(dir)
                              from file in Directory.GetFiles(subDir)
                              where IsMediaFile(file) && !file.ToLower().Contains("sample")
                              select file);

                GetMediaFiles(dir, list);
            }

            return list;
        }

        private static List<string> GetRarFiles(string directory, List<string> list)
        {
            // get rar files from main directory
            foreach (var file in Directory.GetFiles(directory).Where(file => file.EndsWith(".rar") && !file.ToLower().Contains("sample")))
            {
                list.Add(file);
                break;
            }

            // get rar files from sub directories
            foreach (var dir in Directory.GetDirectories(directory))
            {
                foreach (var subDir in Directory.GetDirectories(dir))
                {
                    foreach (var file in Directory.GetFiles(subDir).Where(file => file.EndsWith(".rar") && !file.ToLower().Contains("sample")))
                    {
                        list.Add(file);
                        break;
                    }
                }

                GetRarFiles(dir, list);
            }

            return list;
        }
    }
}