using MimeKit;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string otp);
}

public class EmailServices : IEmailService
{
    private readonly IConfiguration _config;

    public EmailServices(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendOtpEmailAsync(string toEmail, string otp)
    {
        var email = _config["EmailSettings:From"];
        var password = _config["EmailSettings:Password"];
        var smtpHost = "smtp.gmail.com";
        var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]);

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(email));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Your OTP Code";
        message.Body = new TextPart("plain") { Text = $"Your OTP is: {otp}" };

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
        //await client.ConnectAsync(smtpHost, smtpPort, true); // true = use SSL
        await client.AuthenticateAsync(email, password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}