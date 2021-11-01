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
    public class UserRepository
    {
        private readonly ApplicationContext _context;

        public UserRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByLoginOrEmail(string loginOrEmail)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == loginOrEmail || x.Email == loginOrEmail);
            if (user == null)
            {
                throw new ArgumentException("Wrong login or password");
            }

            return user;
        }

        public async Task<bool> IsUserExists(string login, string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == login || x.Email == email);

            return user != null;
        }

        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new ArgumentException($"User with id {id} not found");
            }

            return user;
        }

        public async Task AddUser(User user)
        {
            await _context.AddAsync(user);

            await _context.SaveChangesAsync();
        }
    }
}
