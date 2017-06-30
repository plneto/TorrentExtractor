using System.Collections.Generic;

namespace TorrentExtractor.Core.Models
{
    public class EmailSettings
    {
        public List<string> ToAddresses { get; set; }
        public List<string> BccAddresses { get; set; }
        public string SmtpServerAddress { get; set; }
        public int SmtpServerPort { get; set; }
        public bool SmtpUseSsl { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
        public string FromPassword { get; set; }
    }
}