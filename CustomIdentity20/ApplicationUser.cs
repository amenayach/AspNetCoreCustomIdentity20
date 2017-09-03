using System.Collections.Generic;
using System.Security.Claims;

namespace CustomIdentity20
{
    public class ApplicationUser
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserNameNormalized => UserName?.ToUpper();
        public string Email { get; set; }
        public string EmailNormalized => Email?.ToUpper();
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string City { get; set; }

        public List<Claim> Claims { get; set; }
    }
}
