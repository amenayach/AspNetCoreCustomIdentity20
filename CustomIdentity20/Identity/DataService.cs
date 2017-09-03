using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomIdentity20.Identity
{
    public class DataService
    {
        private List<ApplicationUser> _users = new List<ApplicationUser>();

        public DataService()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "data.json");

            if (File.Exists(filePath))
            {
                var data = File.ReadAllText(filePath);

                if (!string.IsNullOrWhiteSpace(data))
                {
                    var array = JArray.Parse(data);

                    foreach (JToken token in array)
                    {
                        _users.Add(new ApplicationUser()
                        {
                            Id = token["Id"].ToString(),
                            UserId = token.GetValue<string>("UserId"),
                            UserName = token.GetValue<string>("UserName"),
                            Email = token.GetValue<string>("Email"),
                            PhoneNumber = token.GetValue<string>("PhoneNumber"),
                            PhoneNumberConfirmed = token.GetValue<bool>("PhoneNumberConfirmed"),
                            PasswordHash = token.GetValue<string>("PasswordHash"),
                            TwoFactorEnabled = token.GetValue<bool>("TwoFactorEnabled"),
                            City = token.GetValue<string>("City"),
                            Claims = ((JArray)token["Claims"])
                                .Select(m =>
                                    new Claim(
                                        m.GetValue<string>("Type"),
                                        m.GetValue<string>("Value"),
                                        m.GetValue<string>("ValueType"))).ToList()
                        });
                    }

                }
            }
        }

        public ApplicationUser GetUser(Func<ApplicationUser, bool> predicate)
        {
            return _users.FirstOrDefault(predicate);
        }

        public ApplicationUser AddUser(ApplicationUser user)
        {
            if (string.IsNullOrWhiteSpace(user.Id))
            {
                user.Id = Guid.NewGuid().ToString();
            }

            user.Claims = new List<Claim>
            {
                new Claim("UserHeroType", "superman"),
                new Claim("UserHeroBirthdate", DateTime.Now.ToString("dd-MM-yyyy"))
            };

            _users.Add(user);

            SaveUsers();

            return user;
        }

        public ApplicationUser UpdateUser(ApplicationUser user)
        {
            var targetUser = _users.FirstOrDefault(m => m.Id == user.Id);

            targetUser.UserName = user.UserName;
            targetUser.City = user.City;
            targetUser.Email = user.Email;

            SaveUsers();

            return user;
        }

        public void DeleteUser(ApplicationUser user)
        {
            _users = _users.Where(m => m.Id != user.Id).ToList();

            SaveUsers();
        }

        public void AddClaim(ApplicationUser user, Claim claim)
        {
            if (user.Claims == null) user.Claims = new List<Claim>();

            user.Claims.Add(claim);

            SaveUsers();
        }

        public void AddClaims(ApplicationUser user, List<Claim> claims)
        {
            if (user.Claims == null) user.Claims = new List<Claim>();

            user.Claims.AddRange(claims);

            SaveUsers();
        }

        public void RemoveClaims(ApplicationUser user, List<Claim> claims)
        {
            if (user.Claims == null) user.Claims = new List<Claim>();

            var types = claims.Select(m => m.Type).ToList();

            user.Claims.RemoveAll(m => types.Contains(m.Type));

            SaveUsers();
        }

        public List<ApplicationUser> GetUsers()
        {
            return _users;
        }

        private void SaveUsers()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "data.json");

            File.WriteAllText(filePath, JArray.FromObject(_users).ToString());
        }
    }

    public static class JsonExtensions
    {
        public static T GetValue<T>(this JToken token, string key)
        {
            if (token[key] == null)
            {
                return default(T);
            }
            return token.Value<T>(key);
        }
    }
}
