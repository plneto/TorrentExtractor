using System.Collections.Generic;

namespace TorrentExtractor.Core.Helpers
{
    public static class FileHelper
    {
        public static bool IsMediaFile(string file, List<string> supportedMediaTypes)
        {
            return supportedMediaTypes.Contains(file.Substring(file.LastIndexOf('.')));
        }
    }
}
