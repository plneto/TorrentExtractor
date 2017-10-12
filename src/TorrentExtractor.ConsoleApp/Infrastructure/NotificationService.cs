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
        private readonly ILogger _logger;

        public NotificationService(EmailSettings settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public void SendEmail(Email email)
        {
            _logger.Debug("Enter SendEmail");

            _logger.Debug($"Recipients: {string.Join(", ", email.Recipients)}, Subject: {email.Subject}, Body: {email.Body}");

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

            var body = new TextPart { Text = email.Body};

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

            _logger.Debug("Exit SendEmail");
        }
    }
}