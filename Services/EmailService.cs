using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;

public class EmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly IConfiguration _configuration;
    public EmailService(IOptions<EmailSettings> emailSettings, IConfiguration configuration)
    {
        _emailSettings = emailSettings.Value;
        _configuration = configuration;
    }
    public void SendEmail(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Support CareApp", _configuration["SMTP_USERNAME"]));
        message.To.Add(new MailboxAddress("Reciever Name", toEmail));
        message.Subject = subject;
        var textPart = new TextPart("plain")
        {
            Text = body
        };
        message.Body = textPart;
        using (var client = new SmtpClient())
        {
            client.Connect(_emailSettings.SmtpServer, _emailSettings.SmtpPort,
           SecureSocketOptions.StartTls);
            client.Authenticate(_configuration["SMTP_USERNAME"], _configuration["SMTP_PASSWORD"]);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}
