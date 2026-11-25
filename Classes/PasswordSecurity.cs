using System.Security.Cryptography;
using System.Text;

namespace AMS.Classes
{
    internal static class PasswordSecurity
    {
        private const int Iterations = 120000;
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const string Prefix = "pbkdf2-sha256";

        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Derive(password, salt, Iterations);
            return $"{Prefix}${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string storedPassword, string password, out bool needsRehash)
        {
            needsRehash = false;
            string[] parts = storedPassword.Split('$');
            if (parts.Length == 4 && parts[0] == Prefix &&
                int.TryParse(parts[1], out int iterations) &&
                iterations >= 10000)
            {
                try
                {
                    byte[] salt = Convert.FromBase64String(parts[2]);
                    byte[] expected = Convert.FromBase64String(parts[3]);
                    byte[] actual = Derive(password, salt, iterations);
                    needsRehash = iterations < Iterations;
                    return CryptographicOperations.FixedTimeEquals(actual, expected);
                }
                catch (FormatException)
                {
                    return false;
                }
            }

            needsRehash = true;
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(storedPassword),
                Encoding.UTF8.GetBytes(password));
        }

        private static byte[] Derive(string password, byte[] salt, int iterations)
        {
            using var derive = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            return derive.GetBytes(HashSize);
        }
    }
}
