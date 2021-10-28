using IdentityService.Application.Dto;
using IdentityService.Domain.Entity;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.EF;
using IdentityService.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Service
{
    public class TokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly TokenRepository _repository;
        private readonly byte[] _secretBytes;

        private const string SecurityAlgorithm = SecurityAlgorithms.HmacSha256Signature;

        public TokenService(IOptions<JwtOptions> options, TokenRepository repository)
        {
            _jwtOptions = options.Value;
            _repository = repository;
            _secretBytes = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
        }

        public async Task<JwtAuthToken> GenerateTokens(User user, Claim[] claims, string ip)
        {
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: now.AddMinutes(_jwtOptions.Lifetime),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secretBytes), SecurityAlgorithm));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refreshToken = new UserToken
            {
                UserId = user.Id,
                Token = GenerateRefreshToken(),
                CreateTime = now,
                ExpireAt = now.AddDays(7),
                CreateByIp = ip,
                RevorkedAt = null
            };

            await _repository.AddTokenAsync(refreshToken);

            return new JwtAuthToken
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }

        private string GenerateRefreshToken()
        {
            using var rng = new RNGCryptoServiceProvider();
            var bytes = new byte[64];
            rng.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }

        public async Task<JwtAuthToken> Refresh(string accessToken, string refreshToken, string ip)
        {
            var decodedToken = DecodeJwtToken(accessToken);
            if (decodedToken.ValidatedToken == null || !decodedToken.ValidatedToken.Header.Alg.Equals(SecurityAlgorithm))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var userName = decodedToken.ClaimsPrincipal.Identity?.Name;
            var tokenData = await _repository.GetTokenInfoAsync(refreshToken);

            if (tokenData == null)
            {
                throw new SecurityTokenException("Invalid token");
            }
          
            if (tokenData.User.Login != userName || tokenData.ExpireAt < DateTime.UtcNow)
            {
                throw new SecurityTokenException("Invalid token");
            }

            return await GenerateTokens(tokenData.User, decodedToken.ClaimsPrincipal.Claims.ToArray(), ip);
        }

        private DecodedJwt DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _jwtOptions.Issuer,
                        ValidAudience = _jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(_secretBytes)
                    },
                    out var validatedToken);

            return new DecodedJwt
            {
                ClaimsPrincipal = principal, 
                ValidatedToken = validatedToken as JwtSecurityToken
            };
        }

        public async Task RevorkeToken(string refreshToken, string accessToken, string ip)
        {
            var decodedToken = DecodeJwtToken(accessToken);
            if (decodedToken.ValidatedToken == null || !decodedToken.ValidatedToken.Header.Alg.Equals(SecurityAlgorithm))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var userName = decodedToken.ClaimsPrincipal.Identity?.Name;
            var tokenData = await _repository.GetTokenInfoAsync(refreshToken);

            if (tokenData == null)
            {
                throw new SecurityTokenException("Invalid token");
            }

            if (tokenData.User.Login != userName || tokenData.ExpireAt < DateTime.UtcNow)
            {
                throw new SecurityTokenException("Invalid token");
            }

            await _repository.RevorkeTokenAsync(tokenData, ip);
        }
    }
}
