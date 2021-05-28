using System.Threading.Tasks;
using Infrastructure.Identity.Models;

namespace Infrastructure.Services
{
    public interface IUserAuthentication
    {
        Task<ApplicationUser> GetById(string id);
        void InvalidateSession();
        void SaveUserAsync(ApplicationUser userGet);
        ApplicationUser GetCurrentSessionUser();


    }
}