using System;
using System.Collections.Generic;

namespace IdentityService.Domain.Entity
{
    public class User
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public DateTime RegisterDate { get; set; }

        public bool IsActive { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<UserToken> Tokens { get; set; }
    }
}
