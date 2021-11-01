using IdentityService.Application.Dto;
using IdentityService.Domain.Entity;
using IdentityService.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Service
{
    public class AuthService
    {
        private readonly TokenService _tokenService;
        private readonly PasswordService _passwordService;
        private readonly UserRepository _userRepository;

        public AuthService(TokenService tokenService, PasswordService passwordService, UserRepository userRepository)
        {
            _tokenService = tokenService;
            _passwordService = passwordService;
            _userRepository = userRepository;
        }

        public async Task<JwtAuthToken> Login(LoginRequest loginRequest, string ip)
        {
            var user = await _userRepository.GetUserByLoginOrEmail(loginRequest.Login);
            if (_passwordService.Verify(loginRequest.Password, user.Password) == false)
            {
                throw new ArgumentException("Wrong login or password");
            }

            var userClaims = GetUserClaims(user);
            var tokens = await _tokenService.GenerateTokens(user, userClaims, ip);

            return tokens;
        }

        private Claim[] GetUserClaims(User user)
        {
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim("Id", user.Id.ToString())
            };

            return claims;
        }

        public async Task<JwtAuthToken> Register(RegisterRequest registerRequest, string ip)
        {
            if (await _userRepository.IsUserExists(registerRequest.Login, registerRequest.Email))
            {
                throw new ArgumentException("User with current login or email already exists");
            }

            var user = new User
            {
                Email = registerRequest.Email,
                Login = registerRequest.Login,
                Password = _passwordService.HashPassword(registerRequest.Password),
                RegisterDate = DateTime.UtcNow
            };

            await _userRepository.AddUser(user);

            var userClaims = GetUserClaims(user);
            var tokens = await _tokenService.GenerateTokens(user, userClaims, ip);

            return tokens;
        }

        public void RefreshTokens(JwtAuthToken tokens, string ip)
        {
            //_tokenService.Refresh()
        }
    }
}
