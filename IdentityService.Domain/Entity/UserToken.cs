using System;

namespace IdentityService.Domain.Entity
{
    public class UserToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string Token { get; set; }

        public string CreateByIp { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime ExpireAt { get; set; }

        public DateTime? RevorkedAt { get; set; }

        public string RevorkedIp { get; set; }

        public bool IsExpire => DateTime.UtcNow > ExpireAt;

        public bool IsActive => RevorkedAt == null == IsExpire == false;
    }
}
