using System;
using System.Security.Cryptography;
using System.Text;

namespace MaqboolFashion.Data.Security
{
    public static class PasswordHasher
    {
        public static (string Hash, string Salt) CreateHash(string password)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);
                string saltStr = Convert.ToBase64String(salt);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
                {
                    byte[] hash = pbkdf2.GetBytes(20);
                    return (Convert.ToBase64String(hash), saltStr);
                }
            }
        }

        public static bool Verify(string password, string storedHash, string storedSalt)
        {
            byte[] salt = Convert.FromBase64String(storedSalt);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                return Convert.ToBase64String(hash) == storedHash;
            }
        }
    }
}