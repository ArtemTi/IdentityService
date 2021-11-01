using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Service
{
    public class PasswordService
    {
        private const int SaltSize = 128 / 8;
        private const int IterationCount = 10000;
        private const int RequestedBytes = 256 / 8;

        public string HashPassword(string password)
        {
            var salt = new byte[SaltSize];
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(salt);

            var subKey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, IterationCount, RequestedBytes);
            var outputBytes = new byte[salt.Length + subKey.Length];

            Buffer.BlockCopy(subKey, 0, outputBytes, 0, subKey.Length);
            Buffer.BlockCopy(salt, 0, outputBytes, subKey.Length, salt.Length);

            return Convert.ToBase64String(outputBytes);
        }

        public bool Verify(string password, string hashedPassword)
        {
            var hashedBytes = Convert.FromBase64String(hashedPassword);
            var salt = new byte[SaltSize];
            var hashedSubKey = new byte[hashedBytes.Length - SaltSize];

            Buffer.BlockCopy(hashedBytes, 0, hashedSubKey, 0, hashedSubKey.Length);
            Buffer.BlockCopy(hashedBytes, hashedSubKey.Length, salt, 0, SaltSize);

            var actualSubKey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, IterationCount, RequestedBytes);

            return CryptographicOperations.FixedTimeEquals(actualSubKey, hashedSubKey);
        }
    }
}