using College.IdentityWebApi.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using College.IdentityWebApi.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using College.IdentityWebApi.Domain.Services.Interface;
using College.IdentityWebApi.Domain.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace College.IdentityWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

       
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var connectionString = Configuration.GetConnectionString("ConnectionStrings:SqlServer") == null ?
                "Integrated Security=SSPI;Persist Security Info=False;User ID=ABHNTBL6800353;Initial Catalog=CollegeAPI;Data Source=localhost" :
                Configuration.GetConnectionString("ConnectionStrings:SqlServer");

            var migrationAssembly = typeof(Startup)
                .GetTypeInfo()
                .Assembly
                .GetName().Name;

            services.AddDbContext<Context>(
                options => options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(migrationAssembly))
                );

            services.AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;// exigencia de confirmacao de email

                options.Password.RequireDigit = false;// opcoes de senha
                options.Password.RequireNonAlphanumeric = false;// opcoes de senha
                options.Password.RequireUppercase = false;// opcoes de senha
                options.Password.RequireLowercase = false;//opcoes de senha
                options.Password.RequiredLength = 5; // opcoes de senha

                options.Lockout.MaxFailedAccessAttempts = 3; // numero de tentativas possiveis logout
                options.Lockout.AllowedForNewUsers = true; // habilita para novos usuarios logout

            })
                .AddRoles<Role>()
                .AddEntityFrameworkStores<Context>()
                .AddRoleValidator<RoleValidator<Role>>()
                .AddRoleManager<RoleManager<Role>>()
                .AddSignInManager<SignInManager<User>>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,// validador emissor de chave igual a true
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),//de onde busca o token
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    });

            services.AddMvc( options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(policy)); // toda vez que a controller é criada não é preciso colocar authorize (data annotation)
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


            var mappingConfig = new MapperConfiguration(mapper =>
            {
                mapper.AddProfile(new AutoMapperProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();


            services.AddCors();

            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddSingleton(mapper);

        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(config => config.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); // habilita requisicao cruzada

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
