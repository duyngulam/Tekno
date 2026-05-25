using Tekno.Application.Auth.Interfaces;

namespace Tekno.Infrastructure.Auth
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            try
            {
                if (passwordHash.StartsWith("$2", StringComparison.Ordinal))
                {
                    return BCrypt.Net.BCrypt.Verify(password, passwordHash);
                }

                return VerifyLegacySha256(password, passwordHash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return VerifyLegacySha256(password, passwordHash);
            }
        }

        private static bool VerifyLegacySha256(string password, string passwordHash)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            var computedHash = Convert.ToBase64String(hash);

            return string.Equals(computedHash, passwordHash, StringComparison.Ordinal);
        }
    }
}
