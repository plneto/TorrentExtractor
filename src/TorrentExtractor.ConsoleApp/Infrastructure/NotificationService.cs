using MailKit.Net.Smtp;
using MimeKit;
using Serilog;
using TorrentExtractor.ConsoleApp.Models;
using TorrentExtractor.Core.Settings;

namespace TorrentExtractor.ConsoleApp.Infrastructure
{
    public class NotificationService : INotificationService
    {
        private readonly EmailSettings _settings;

        public NotificationService(EmailSettings settings)
        {
            _settings = settings;
        }

        public void SendEmail(Email email)
        {
            Log.Debug("Enter SendEmail");

            Log.Debug($"Recipients: {string.Join(", ", email.Recipients)}, Subject: {email.Subject}, Body: {email.Body}");

            // Compose a message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderAddress));

            foreach (var toAddress in _settings.ToAddresses)
            {
                message.To.Add(new MailboxAddress(toAddress));
            }

            if (_settings.BccAddresses != null)
            {
                foreach (var bccAddress in _settings.BccAddresses)
                {
                    message.Bcc.Add(new MailboxAddress(bccAddress));
                }
            }

            var body = new TextPart { Text = email.Body };

            message.Subject = email.Subject;
            message.Body = body;

            // Send it!
            using (var client = new SmtpClient())
            {
                // XXX - Should this be a little different?
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(_settings.SmtpServerAddress, _settings.SmtpServerPort, _settings.SmtpUseSsl);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_settings.SmtpUsername, _settings.SmtpPassword);

                client.Send(message);
                client.Disconnect(true);
            }

            Log.Debug("Exit SendEmail");
        }
    }
}