using System.Linq;

namespace TorrentExtractor.ConsoleApp.Helpers
{
    public static class ArgsHelper
    {
        public static string[] RemoveEmptyArgs(string[] args)
        {
            for (var index = 0; index < args.Length; index++)
            {
                if (!args[index].StartsWith("-")) continue;

                if ((index + 1) < args.Length && args[index + 1].StartsWith("-"))
                {
                    // remove empty arg
                    args = args.Where(c => c != args[index]).ToArray();
                    index--;
                }
                else if ((index + 1) == args.Length && args[index].StartsWith("-"))
                {
                    // remove last empty arg
                    args = args.Where(c => c != args[index]).ToArray();
                }
            }

            return args;
        }
    }
}
