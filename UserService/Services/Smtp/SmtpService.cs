using System.Net;
using System.Net.Mail;
using UserService.Models.Smtp;
using UserService.Services;

namespace MireaHackBack.Services;

public class SmtpService : ISmtpService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _from;
    private readonly string _server;
    private readonly string _port;
    private readonly string _username;
    private readonly string _password;

    public SmtpService()
    {
        _from = Environment.GetEnvironmentVariable("SMTP_FROM") ?? "";
        _server = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? "";
        _port = Environment.GetEnvironmentVariable("SMTP_PORT") ?? "0";
        _username = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "";
        _password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";

        _smtpClient = new SmtpClient(_server, int.Parse(_port))
        {
            Credentials = new NetworkCredential(_username, _password),
            EnableSsl = true
        };
    }

    public bool SendSystemMail(EmailModel model)
    {
        if (_from == "") return false;
        try
        {
            MailMessage mailMessage = new(_from, model.To, model.Subject, model.Body);
            _smtpClient.Send(mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            // Handle any exceptions here
            Console.WriteLine($"SmtpService: {ex.Message}");
            return false;
        }
    }
}