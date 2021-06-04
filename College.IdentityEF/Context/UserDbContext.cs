using College.Identity.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace College.IdentityEF.Context
{
    public class UserDbContext : IdentityDbContext<User>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
