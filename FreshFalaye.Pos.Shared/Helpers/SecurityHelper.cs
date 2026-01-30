using System.Security.Cryptography;
using System.Text;

namespace FreshFalaye.Pos.Shared.Helpers
{
    public static class SecurityHelper
    {
        public static string Hash(string input)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(
                sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }
    }
}
