using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace College.Identity.Identity
{
    public class UserStore : IUserStore<User>, IUserPasswordStore<User>
    {
        public UserStore()
        {
           
        }

        public static DbConnection GetOpenConnection()
        {
            var connection = new SqlConnection("Integrated Security=SSPI;Persist Security Info=False;User ID=ABHNTBL6800353;Initial Catalog=College;Data Source=localhost");

            connection.Open();
            return connection;
        }


        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    "insert into [dbo].[Users]([Id],[UserName],[NormalizedUserName],[PasswordHash]) values (@id,@userName,@normalizedUserName, @password)",
                    new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        normalizedUserName = user.NormalizedUserName,
                        password = user.PasswordHash
                    });
            }

            return IdentityResult.Success;
        }

        public async  Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    "delete from Users where Id= @id",
                    new{ id = user.Id });
            }

            return IdentityResult.Success;
        }

        protected virtual void Dispose(bool disposing)
        {

        }
        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<User>(
                    "select * from Users where Id= @id",
                    new { id = userId });
            }
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<User>(
                    "select * from Users where NormalizedUserName= @name",
                    new { name = normalizedUserName });
            }
        }

        public  Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
            =>  Task.FromResult(user.NormalizedUserName);

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
            =>  Task.FromResult(user.Id);

        public  Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
            =>  Task.FromResult(user.UserName);

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    "update Users set Id=@id, UserName=@userName, NormalizedUserName=@normalizedUserName, Password=@password)",
                    new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        normalizedUserName = user.NormalizedUserName,
                        password = user.PasswordHash
                    });
            }

            return IdentityResult.Success;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.PasswordHash != null);
    }
}
