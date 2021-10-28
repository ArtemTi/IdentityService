using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


namespace IdentityService.Application.Dto
{
    public class DecodedJwt
    {
        public ClaimsPrincipal ClaimsPrincipal { get; set; }

        public JwtSecurityToken ValidatedToken { get; set; }
    }
}
