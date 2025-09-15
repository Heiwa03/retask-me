
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace HelperLayer.Security{
    /// <summary>
    /// Helper class for password operations: hashing, verification, and validation.
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Checks if two passwords match.
        /// </summary>
        /// <param name="password">The original password.</param>
        /// <param name="repPassword">The repeated password.</param>
        /// <returns>True if passwords are identical; otherwise false.</returns>
        public static bool ValidateRegisterData(string password, string repPassword)
        {
            return password == repPassword;
        }

        /// <summary>
        /// Hashes a password using BCrypt with a work factor of 12.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password as a string.</returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        /// <summary>
        /// Verifies a password against a hashed password.
        /// </summary>
        /// <param name="password">The plain text password.</param>
        /// <param name="hashedPassword">The hashed password to verify against.</param>
        /// <returns>True if the password matches; otherwise false.</returns>
        public static bool VerifyHashedPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        /// <summary>
        /// Checks if a password meets strength requirements:
        /// must contain upper and lower case letters, digits, and special characters.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <returns>True if the password is strong; otherwise false.</returns>
        public static bool IsPasswordStrong(string password)
        {
            char[] charPass = password.ToCharArray();

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;
            bool hasSpecial = false;

            for (int i = 0; i < charPass.Length; i++)
            {
                if (char.IsUpper(charPass[i])) hasUpper = true;
                else if (char.IsLower(charPass[i])) hasLower = true;
                else if (char.IsDigit(charPass[i])) hasDigit = true;
                else hasSpecial = true;
            }

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }
    }
}