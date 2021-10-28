using System.Collections.Generic;

namespace IdentityService.Domain.Entity
{
    public class Role
    {
        public int Id { get; set; }

        public RoleType RoleName { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }

    public enum RoleType
    {
        Admin,
        User
    }
}
