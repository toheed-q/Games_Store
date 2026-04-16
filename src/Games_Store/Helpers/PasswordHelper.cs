using System.Security.Cryptography;
using System.Text;

namespace Games_Store.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }

        public static bool VerifyPassword(string input, string storedHash)
            => HashPassword(input) == storedHash;
    }
}
