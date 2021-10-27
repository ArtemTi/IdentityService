using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entity
{
    public class UserToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string RefreshToken { get; set; }

    }
}
