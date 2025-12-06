using Identity.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Security
{
    public class Pbkdf2PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32;  // 256 bits
        private const int Iterations = 100_000;
        private const char Delimiter = ';';

        public string Hash(string plainTextPassword)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword))
                throw new ArgumentException("Password cannot be empty");

            // generar salt
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            // derivar clave
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(plainTextPassword),
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize
            );

            // almacenar como "iterations;salt;base64hash"
            return string.Join(
                Delimiter,
                Iterations.ToString(),
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash)
            );
        }

        public bool Verify(string plainTextPassword, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            var parts = hashedPassword.Split(Delimiter);

            if (parts.Length != 3)
                return false;

            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] storedHash = Convert.FromBase64String(parts[2]);

            // derivar clave con los mismos parámetros
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(plainTextPassword),
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                storedHash.Length
            );

            return CryptographicOperations.FixedTimeEquals(storedHash, hashToCompare);
        }
    }
}
