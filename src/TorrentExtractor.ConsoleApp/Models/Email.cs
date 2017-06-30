using System.Collections.Generic;

namespace TorrentExtractor.ConsoleApp.Models
{
    public class Email
    {
        public Email()
        {
            Recipients = new List<string>();
            Bcc = new List<string>();
            Attachments = new List<string>();
        }
        
        public string Subject { get; set; }
        
        public string Body { get; set; }
        
        public List<string> Recipients { get; set; }
        
        public List<string> Bcc { get; set; }
        
        public List<string> Attachments { get; set; }
    }
}