using System.Text.RegularExpressions;
using Serilog;

namespace TorrentExtractor.Domain.Services
{
    public class TorrentDomainService : ITorrentDomainService
    {
        public static Regex RegexMultipleSeasonEpisodeFormats = new Regex(@"(.*?)\.[sS]?(\d{1,2})x?[eE]?(\d{2})\.(.*)", RegexOptions.IgnoreCase);
        public static Regex RegexSeasonEpisodeSxxExxFormat = new Regex(@"[sS](\d{1,2})[eE]?(\d{2})", RegexOptions.IgnoreCase);
        public static Regex RegexMultipleNumbers = new Regex(@"(.*?)\.[sS]?(\d{1,2})x?[eE]?(\d{2})\.(\d{3,4})(.*)", RegexOptions.IgnoreCase);

        public string GetFormattedTvShowFileName(string fileName)
        {
            Log.Debug("Enter Format File Name");

            Log.Debug("Original Filename: {0}", fileName);

            var tempFile = fileName;

            var standardFormat = RegexSeasonEpisodeSxxExxFormat.Matches(fileName);

            if (standardFormat.Count > 0)
            {
                Log.Debug("The file already contains the SxxExx format.");
                return fileName;
            }

            // check if filename has year + season and episode numbers without SxxExx format
            if (RegexMultipleNumbers.Matches(fileName).Count > 0)
            {
                var matchesMultipleNumbers = RegexMultipleNumbers.Matches(fileName);

                foreach (Match match in matchesMultipleNumbers)
                {
                    var name = match.Groups[1].ToString();
                    var namePartTwo = match.Groups[2].ToString();
                    var namePartThree = match.Groups[3].ToString();
                    var numbers = match.Groups[4].ToString();
                    var ending = match.Groups[5].ToString();

                    var season = numbers.Length == 3 ? int.Parse(numbers.Substring(0, 1)) : int.Parse(numbers.Substring(0, 2));
                    var episode = numbers.Length == 3 ? int.Parse(numbers.Substring(1)) : int.Parse(numbers.Substring(2));

                    var formattedSeason = season <= 9 ? "0" + season : season.ToString();
                    var formattedEpisode = episode <= 9 ? "0" + episode : episode.ToString();

                    tempFile = $@"{name}.{namePartTwo}{namePartThree}.s{formattedSeason}e{formattedEpisode}{ending}";
                    Log.Debug("Modified Filename: {0}", tempFile);
                    break;
                }
            }
            else
            {
                var matches = RegexMultipleSeasonEpisodeFormats.Matches(fileName);

                foreach (Match match in matches)
                {
                    var name = match.Groups[1].ToString();
                    var season = int.Parse(match.Groups[2].ToString());
                    var episode = int.Parse(match.Groups[3].ToString());
                    var ending = match.Groups[4].ToString();

                    var formattedSeason = season <= 9 ? "0" + season : season.ToString();
                    var formattedEpisode = episode <= 9 ? "0" + episode : episode.ToString();

                    tempFile = $@"{name}.s{formattedSeason}e{formattedEpisode}.{ending}";
                    Log.Debug("Modified Filename: {0}", tempFile);
                    break;
                }
            }

            if (tempFile == fileName)
                Log.Debug("The File Name wasn't modified.");

            Log.Debug("Exit Format File Name");

            return tempFile;
        }
    }
}