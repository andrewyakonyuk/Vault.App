using System.Threading.Tasks;

namespace Vault.WebHost.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}