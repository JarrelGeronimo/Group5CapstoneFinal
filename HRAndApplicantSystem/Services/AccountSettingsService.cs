using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Utilities;

namespace HRAndApplicantSystem.Services
{
    public class AccountSettingsService
    {
        private readonly IUserRepository userRepository;
        private readonly DatabaseHelper db;

        public AccountSettingsService(IUserRepository userRepo = null)
        {
            userRepository = userRepo ?? new UserRepository(new DatabaseHelper());
            db = new DatabaseHelper();
        }

        public void ShowAccountSettings(string username)
        {
            bool managing = true;

            while (managing)
            {
                Console.Clear();
                Console.WriteLine("=== Account Settings ===");
                Console.WriteLine("1. Change Password");
                Console.WriteLine("2. Change Username");
                Console.WriteLine("3. Back to Dashboard");
                Console.Write("\nChoose an option: ");

                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ChangePassword(username);
                        break;
                    case "2":
                        username = ChangeUsername(username) ?? username;
                        break;
                    case "3":
                        managing = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ChangePassword(string username)
        {
            Console.Clear();
            Console.WriteLine("=== Change Password ===\n");

            string currentPassword = InputValidator.GetValidatedInput("Enter your current password: ", "Current password");
            string newPassword = InputValidator.GetValidatedInput("Enter your new password: ", "New password");
            string confirmPassword = InputValidator.GetValidatedInput("Confirm your new password: ", "Confirm password");

            if (newPassword != confirmPassword)
            {
                Console.WriteLine("\n❌ Passwords do not match. Please try again.");
                Console.ReadKey();
                return;
            }

            if (currentPassword == newPassword)
            {
                Console.WriteLine("\n❌ New password must be different from current password.");
                Console.ReadKey();
                return;
            }

            bool success = db.ChangeUserPassword(username, currentPassword, newPassword);

            if (success)
            {
                Console.WriteLine("\n✅ Password changed successfully!");
                string userRole = db.GetRoleNameByUsername(username);
                db.LogAuditTrail(userRole, username, "Changed password");
            }
            else
            {
                Console.WriteLine("\n❌ Password change failed. Current password may be incorrect.");
            }

            Console.ReadKey();
        }

        private string? ChangeUsername(string currentUsername)
        {
            Console.Clear();
            Console.WriteLine("=== Change Username ===\n");

            string password = InputValidator.GetValidatedInput("Enter your password to confirm: ", "Password");

            // Validate password first
            if (!db.ValidateLogin(currentUsername, password))
            {
                Console.WriteLine("\n❌ Invalid password. Username change cancelled.");
                Console.ReadKey();
                return null;
            }

            string newUsername = InputValidator.GetValidatedInput("Enter your new username: ", "New username");

            if (newUsername == currentUsername)
            {
                Console.WriteLine("\n❌ New username must be different from current username.");
                Console.ReadKey();
                return null;
            }

            // Get role before changing username
            string userRole = db.GetRoleNameByUsername(currentUsername);

            bool success = db.ChangeUsername(currentUsername, newUsername);

            if (success)
            {
                Console.WriteLine($"\n✅ Username changed successfully!");
                Console.WriteLine($"Your new username is: {newUsername}");
                Console.WriteLine("\nYou will need to log in again with your new username.");
                db.LogAuditTrail(userRole, newUsername, "Changed username");
                Console.ReadKey();
                return newUsername; // Return the new username
            }
            else
            {
                Console.WriteLine("\n❌ Username change failed. This username may already exist.");
                Console.ReadKey();
                return null;
            }
        }
    }
}
