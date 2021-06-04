using College.IdentityWebApi.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace College.IdentityWebApi.Repository
{
    public class Context : IdentityDbContext<User, 
                                             Role, 
                                             int,
                                             IdentityUserClaim<int>, 
                                             UserRole, 
                                             IdentityUserLogin<int>,
                                             IdentityRoleClaim<int>, 
                                             IdentityUserToken<int>>
    {
        /// <summary>
        /// base -> herança da classe ou seja IdentityDbContext
        /// </summary>
        /// <param name="options"></param>
        public Context(DbContextOptions<Context> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.RoleId)
                        .IsRequired();

                userRole.HasOne(ur => ur.User)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.UserId)
                        .IsRequired();
            });

            builder.Entity<Organization>(organization =>
            {
                organization.ToTable("Organizations");
                organization.HasKey(column => column.Id);

                organization.HasMany<User>()
                        .WithOne()
                        .HasForeignKey(column => column.IdOrganization)
                        .IsRequired(false);
            });
        }

    }
}
