using System.Threading.Tasks;

namespace Vault.WebHost.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}