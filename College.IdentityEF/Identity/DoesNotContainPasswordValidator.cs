using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace College.IdentityEF.Identity
{
    public class DoesNotContainPasswordValidator<TUser> : IPasswordValidator<TUser>
        where TUser : class
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            var username = await manager.GetUserNameAsync(user);

            if (username == password) return IdentityResult.Failed(new IdentityError { Description = "Senha não pode ser igual ao campo passwor" });

            if (password.Contains("password")) return IdentityResult.Failed(new IdentityError { Description = "Senha não pode ser igual a = password" });

            return IdentityResult.Success;
        }
    }
}
