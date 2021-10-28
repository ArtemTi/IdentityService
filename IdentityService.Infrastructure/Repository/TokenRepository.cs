using IdentityService.Domain.Entity;
using IdentityService.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Repository
{
    public class TokenRepository
    {
        private readonly ApplicationContext _context;

        public TokenRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddTokenAsync(UserToken token)
        {
            await _context.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<UserToken> GetTokenInfoAsync(string refreshToken)
        {
            var userToken = await _context.UserTokens.FirstOrDefaultAsync(token => token.Token == refreshToken);

            return userToken;
        }

        public async Task RevorkeTokenAsync(UserToken token, string ip)
        {
            var userToken = await _context.UserTokens.FindAsync(token.Id);

            userToken.RevorkedAt = DateTime.UtcNow;
            userToken.RevorkedIp = ip;

            await _context.SaveChangesAsync();
        }
    }
}
