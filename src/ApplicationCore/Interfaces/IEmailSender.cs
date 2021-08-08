using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace InventoryManagementSystem.ApplicationCore.Interfaces
{

    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage emailMessage);
    }
}
