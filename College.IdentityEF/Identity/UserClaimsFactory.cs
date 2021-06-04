using College.Identity.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace College.IdentityEF.Identity
{
    public class UserClaimsFactory : UserClaimsPrincipalFactory<User>
    {

        public UserClaimsFactory(UserManager<User> userManager, 
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {

        }
        
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("Member", user.Member));
            return identity;
        }


        public override Task<ClaimsPrincipal> CreateAsync(User user)
        {
            return base.CreateAsync(user);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}
