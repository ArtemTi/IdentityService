using IdentityService.Application.Service;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.EF;
using IdentityService.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application
{
    public static class Configure
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.JwtOptionSection));

            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnectionString"));
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    var jwtOptions = configuration.GetSection(JwtOptions.JwtOptionSection).Get<JwtOptions>();

                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                    };
                });

            services.AddTransient<TokenRepository>();

            services.AddScoped<IpService>();
            services.AddTransient<TokenService>();
            services.AddTransient<PasswordService>();
            services.AddTransient<AuthService>();
        }
    }
}
