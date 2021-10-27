using IdentityService.Application.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application
{
    public static class Configure
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<TokenService>();
            services.AddTransient<AuthService>();
        }
    }
}
