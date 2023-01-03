using Attendr.IdentityServer.Models.Email;

namespace Attendr.IdentityServer.Services
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
        Task SendEmailAsync(Message message);
    }
}