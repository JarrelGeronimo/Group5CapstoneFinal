using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Repository interface for user data access operations
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Validates user login credentials
        /// </summary>
        bool ValidateLogin(string username, string password);

        /// <summary>
        /// Registers a new applicant user
        /// </summary>
        bool RegisterApplicant(string username, string password);

        /// <summary>
        /// Gets the role ID for a user by username
        /// </summary>
        int GetUserRoleByUsername(string username);

        /// <summary>
        /// Gets the user ID by username
        /// </summary>
        int GetUserIDByUsername(string username);

        /// <summary>
        /// Gets the username by user ID
        /// </summary>
        string GetUsernameByUserID(int userID);

        /// <summary>
        /// Checks if a username already exists
        /// </summary>
        bool UsernameExists(string username);
    }
}
