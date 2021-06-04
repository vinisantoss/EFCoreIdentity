using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace College.IdentityEF.Identity
{
    public class Store2FA
    {
        public ClaimsPrincipal CreateStore2FA(string userId, string provider)
        {
            var identity = new ClaimsIdentity(new List<Claim> { 
            
                new Claim("sub", userId),
                new Claim("amr", provider)
            }, IdentityConstants.TwoFactorUserIdScheme);

            return new ClaimsPrincipal(identity);
        }
    }
}
