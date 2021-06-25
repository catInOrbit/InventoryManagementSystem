using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class UserSessionService : IUserSession
    {

        private ApplicationUser currentLoggedIn = new ApplicationUser();
        private Dictionary<string, string> _userResource = new Dictionary<string, string>();

        private string CurrentUserRole { get; set; }

        public async Task<ApplicationUser> GetCurrentSessionUser()
        {
            return await Task.FromResult(currentLoggedIn);
        }

        public async Task<string> GetCurrentSessionUserRole()
        {
            return await Task.FromResult(CurrentUserRole);
        }

        public async Task<bool> SaveUserResourceAccess(string userId, string resourceId)
        {
            if (_userResource.ContainsValue(resourceId))
                return await Task.FromResult(false);
            else
            {
                if(!_userResource.ContainsKey(userId))
                    _userResource.Add(userId, resourceId);
                return await Task.FromResult(true);
            }
        }

        public async Task<bool> RemoveUserFromResource(string userId, string resourceId)
        {
            foreach (var keyValuePair in _userResource)
            {
                if (keyValuePair.Value == resourceId && keyValuePair.Key == userId)
                {
                    _userResource.Remove(keyValuePair.Key);
                    return await Task.FromResult(true);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<Dictionary<string, string>> GetUserResourceDictionary()
        {
            return await Task.FromResult(_userResource);
        }

        public void InvalidateSession()
        {
            // _signInManager.SignOutAsync();
        }

        public Task SaveUserAsync(ApplicationUser userGet, string role)
        {
            Task.FromResult(CurrentUserRole = role);
            return Task.FromResult(currentLoggedIn = userGet);
        }
        
        private static string ToDebugString<TKey, TValue> (IDictionary<TKey, TValue> dictionary)
        {
            return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
        }
    }
}