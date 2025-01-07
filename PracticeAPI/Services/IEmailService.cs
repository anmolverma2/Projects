using PracticeAPI.Model;

namespace PracticeAPI.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailRequest request);
    }
}
