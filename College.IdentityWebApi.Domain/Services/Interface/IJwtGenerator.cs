using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace College.IdentityWebApi.Domain.Services.Interface
{
    public interface IJwtGenerator
    {
        Task<string> GenerateJwtToken(User user);
    }
}
