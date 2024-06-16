using UserService.Models.Smtp;

namespace UserService.Services;

public interface ISmtpService
{
    public bool SendSystemMail(EmailModel model);
}