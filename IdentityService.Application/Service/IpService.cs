using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace IdentityService.Application.Service
{
    public class IpService
    {
        public IpService()
        {
        }

        public string GetIp(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                return httpContext.Request.Headers["X-Forwarded-For"];

            return httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
