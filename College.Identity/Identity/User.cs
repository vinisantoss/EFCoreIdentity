using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace College.Identity.Identity
{
    public class User : IdentityUser
    {
        public string NomeCompleto { get; set; }

    }
}
