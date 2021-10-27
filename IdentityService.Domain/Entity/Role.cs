using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entity
{
    public class Role
    {
        public int Id { get; set; }

        public RoleType RoleName { get; set; }
    }

    public enum RoleType
    {
        Admin,
        User
    }
}
