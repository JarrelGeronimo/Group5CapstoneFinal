namespace HRAndApplicantSystem.Utilities
{
    /// <summary>
    /// Utility class for validating and sanitizing user input
    /// </summary>
    public static class InputValidator
    {
        /// <summary>
        /// Read a line from console, trim whitespace, and validate it's not empty
        /// </summary>
        public static string GetValidatedInput(string prompt, string fieldName = "Input")
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine($"Error: {fieldName} cannot be empty. Please try again.");
                    continue;
                }

                return input;
            }
        }

        /// <summary>
        /// Read a line from console, trim whitespace, and allow empty input with default fallback
        /// </summary>
        public static string GetOptionalInput(string prompt, string defaultValue = "")
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim() ?? string.Empty;
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
        }

        /// <summary>
        /// Validate username format (alphanumeric, 3-20 characters)
        /// </summary>
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            if (username.Length < 3 || username.Length > 20)
                return false;

            return System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");
        }

        /// <summary>
        /// Validate password strength (minimum 6 characters)
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 6;
        }

        /// <summary>
        /// Get and validate username input
        /// </summary>
        public static string GetValidatedUsername()
        {
            while (true)
            {
                string username = GetValidatedInput("Username: ", "Username");

                if (!IsValidUsername(username))
                {
                    Console.WriteLine("Error: Username must be 3-20 characters and contain only letters, numbers, or underscores.");
                    continue;
                }

                return username;
            }
        }

        /// <summary>
        /// Get and validate password input
        /// </summary>
        public static string GetValidatedPassword()
        {
            while (true)
            {
                string password = GetValidatedInput("Password: ", "Password");

                if (!IsValidPassword(password))
                {
                    Console.WriteLine("Error: Password must be at least 6 characters long.");
                    continue;
                }

                return password;
            }
        }

        /// <summary>
        /// Get and validate email format
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get and validate phone number (basic format: digits with optional - or space)
        /// </summary>
        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[\d\s\-\+\(\)]+$") && phone.Length >= 7;
        }
    }
}
