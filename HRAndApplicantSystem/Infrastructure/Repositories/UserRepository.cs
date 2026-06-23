using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// User repository implementation - Adaptor pattern wrapping DatabaseHelper
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseHelper db;

        public UserRepository(DatabaseHelper databaseHelper = null)
        {
            db = databaseHelper ?? new DatabaseHelper();
        }

        public bool ValidateLogin(string username, string password)
        {
            return db.ValidateLogin(username, password);
        }

        public bool RegisterApplicant(string username, string password, string email)
        {
            // Note: email parameter is not used by DatabaseHelper.RegisterApplicant
            return db.RegisterApplicant(username, password);
        }

        public int GetUserRoleByUsername(string username)
        {
            return db.GetUserRoleByUsername(username);
        }

        public int GetUserIDByUsername(string username)
        {
            return db.GetUserIDByUsername(username);
        }

        public string GetUsernameByUserID(int userID)
        {
            return db.GetUsernameByUserID(userID);
        }

        public bool UsernameExists(string username)
        {
            return db.UsernameExists(username);
        }
    }
}
