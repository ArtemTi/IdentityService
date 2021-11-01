using IdentityService.Application.Dto;
using IdentityService.Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IpService _ipService;

        public AuthController(AuthService authService, IpService ipService)
        {
            _authService = authService;
            _ipService = ipService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginRequest loginRequest)
        {
            var tokens = await _authService.Login(loginRequest, _ipService.GetIp(HttpContext));

            return Ok(tokens);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterRequest registerRequest)
        {
            var tokens = await _authService.Register(registerRequest, _ipService.GetIp(HttpContext));

            return Ok(tokens);
        }
    }
}
