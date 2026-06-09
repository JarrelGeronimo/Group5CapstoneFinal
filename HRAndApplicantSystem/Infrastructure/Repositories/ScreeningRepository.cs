using HRAndApplicantSystem.Database;

namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Screening repository implementation - Adaptor pattern wrapping DatabaseHelper
    /// </summary>
    public class ScreeningRepository : IScreeningRepository
    {
        private readonly DatabaseHelper db;

        public ScreeningRepository(DatabaseHelper databaseHelper = null)
        {
            db = databaseHelper ?? new DatabaseHelper();
        }

        public bool ScreenApplication(int applicationID, string result, string remarks, string screenerUsername)
        {
            return db.ScreenApplication(applicationID, result, remarks, screenerUsername);
        }
    }
}
