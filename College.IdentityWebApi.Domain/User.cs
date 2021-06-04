using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace College.IdentityWebApi.Domain
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }
        public string IdOrganization { get; set; }
        public string Member { get; set; } = "Member";
        public List<UserRole> UserRoles { get; set; }

    }
}
