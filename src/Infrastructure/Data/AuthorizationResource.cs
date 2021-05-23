using System.Collections.Generic;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data
{
    public class AuthorizationResource
    {
        private readonly ApplicationUser _user;
        private readonly IList<string> _userRoles;
        private readonly string _page;

        public AuthorizationResource(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, string page)
        {
            // _userManager = userManager;
            // _roleManager = roleManager;
            _page = page;
        }
    }
}