using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using College.Identity.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using College.IdentityEF.Context;
using College.IdentityEF.Identity;

namespace College.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllersWithViews();

            var connectionString = "Integrated Security=SSPI;Persist Security Info=False;User ID=ABHNTBL6800353;Initial Catalog=College;Data Source=localhost";
            var migrationAssembly = typeof(Startup)
                .GetTypeInfo()
                .Assembly
                .GetName().Name;

            services.AddDbContext<UserDbContext>(
                options => options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(migrationAssembly))
                );

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequireDigit = false;// opcoes de senha
                options.Password.RequireNonAlphanumeric = false;// opcoes de senha
                options.Password.RequireUppercase = false;// opcoes de senha
                options.Password.RequireLowercase = false;//opcoes de senha
                options.Password.RequiredLength = 5; // opcoes de senha

                options.Lockout.MaxFailedAccessAttempts = 3; // numero de tentativas possiveis
                options.Lockout.AllowedForNewUsers = true; // habilita para novos usuarios

            })
                .AddEntityFrameworkStores<UserDbContext>()
                .AddDefaultTokenProviders()
                .AddPasswordValidator<DoesNotContainPasswordValidator<User>>();

            services.AddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsFactory>();

            services.Configure<DataProtectionTokenProviderOptions>( 
                options => options.TokenLifespan = TimeSpan.FromHours(3)
                );

            services.ConfigureApplicationCookie(options => options.LoginPath = "/Login/Login");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
