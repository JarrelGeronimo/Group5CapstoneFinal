namespace HRAndApplicantSystem.Utilities
{
    /// <summary>
    /// Utility class for password hashing and verification
    /// Uses a simple hashing approach (note: production should use bcrypt properly configured)
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Hash a password using PBKDF2
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Hashed password with salt</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty");

            // Generate a salt
            byte[] salt = new byte[16];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Create the hash using PBKDF2
            var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, 10000, System.Security.Cryptography.HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20);

            // Combine salt and hash
            byte[] hashWithSalt = new byte[36];
            System.Buffer.BlockCopy(salt, 0, hashWithSalt, 0, 16);
            System.Buffer.BlockCopy(hash, 0, hashWithSalt, 16, 20);

            // Return as base64 string
            return Convert.ToBase64String(hashWithSalt);
        }

        /// <summary>
        /// Verify a plain text password against a hashed password
        /// </summary>
        /// <param name="plainPassword">Plain text password to verify</param>
        /// <param name="hashedPassword">Hashed password to verify against</param>
        /// <returns>True if password matches, false otherwise</returns>
        public static bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword) || string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            try
            {
                // Extract the salt from the hash
                byte[] hashWithSalt = Convert.FromBase64String(hashedPassword);
                byte[] salt = new byte[16];
                System.Buffer.BlockCopy(hashWithSalt, 0, salt, 0, 16);

                // Hash the provided password with the extracted salt
                var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(plainPassword, salt, 10000, System.Security.Cryptography.HashAlgorithmName.SHA256);
                byte[] hash = pbkdf2.GetBytes(20);

                // Compare the stored hash with the computed hash
                for (int i = 0; i < 20; i++)
                {
                    if (hashWithSalt[i + 16] != hash[i])
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Password verification error: {ex.Message}");
                return false;
            }
        }
    }
}
