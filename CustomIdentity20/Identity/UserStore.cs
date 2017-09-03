using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CustomIdentity20.Identity
{
    public class UserStore : IUserStore<ApplicationUser>,
                                        IRoleStore<ApplicationRole>,
                                        IUserPasswordStore<ApplicationUser>,
                                        IUserClaimStore<ApplicationUser>
    {

        private readonly DataService _dataService = new DataService();

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                user.UserName = userName;
                _dataService.UpdateUser(user);
            }, cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            _dataService.AddUser(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            _dataService.UpdateUser(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            _dataService.DeleteUser(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_dataService.GetUser(m => m.Id == userId));
        }

        Task<ApplicationUser> IUserStore<ApplicationUser>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(_dataService.GetUser(m => string.Equals(m.UserName, normalizedUserName, StringComparison.OrdinalIgnoreCase)));
        }

        public void Dispose()
        {

        }

        /*============================================ UserRole ============================================*/

        Task<ApplicationRole> IRoleStore<ApplicationRole>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<ApplicationRole> IRoleStore<ApplicationRole>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<IdentityResult> IRoleStore<ApplicationRole>.CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        Task<IdentityResult> IRoleStore<ApplicationRole>.UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        Task<IdentityResult> IRoleStore<ApplicationRole>.DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        Task<string> IRoleStore<ApplicationRole>.GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult("role");
        }

        Task<string> IRoleStore<ApplicationRole>.GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult("role");
        }

        Task IRoleStore<ApplicationRole>.SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => { }, cancellationToken);
        }

        Task<string> IRoleStore<ApplicationRole>.GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult("role");
        }

        Task IRoleStore<ApplicationRole>.SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => { }, cancellationToken);
        }

        /*============================================ UserRole ============================================*/


        Task IUserPasswordStore<ApplicationUser>.SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;

            return Task.FromResult(true);
        }

        Task<string> IUserPasswordStore<ApplicationUser>.GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        Task<bool> IUserPasswordStore<ApplicationUser>.HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        /*============================================ UserClaimStore ============================================*/

        Task<IList<Claim>> IUserClaimStore<ApplicationUser>.GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult((IList<Claim>)user.Claims);
        }

        Task IUserClaimStore<ApplicationUser>.AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            _dataService.AddClaims(user, claims.ToList());

            return Task.FromResult(true);
        }

        Task IUserClaimStore<ApplicationUser>.ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            _dataService.RemoveClaims(user, new List<Claim>() { claim });

            _dataService.AddClaims(user, new List<Claim>() { newClaim });

            return Task.FromResult(true);
        }

        Task IUserClaimStore<ApplicationUser>.RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            _dataService.RemoveClaims(user, claims.ToList());

            return Task.FromResult(true);
        }

        Task<IList<ApplicationUser>> IUserClaimStore<ApplicationUser>.GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            return Task.Run(
                () => (IList<ApplicationUser>) _dataService.GetUsers()
                    .Where(u => u.Claims.FirstOrDefault(c => c.Type == claim.Type && c.Value == claim.Value) != null)
                    .ToList(), cancellationToken);
        }
    }
}
