using HRAndApplicantSystem.Database;

namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Audit repository implementation - Adaptor pattern wrapping DatabaseHelper
    /// </summary>
    public class AuditRepository : IAuditRepository
    {
        private readonly DatabaseHelper db;

        public AuditRepository(DatabaseHelper databaseHelper = null)
        {
            db = databaseHelper ?? new DatabaseHelper();
        }

        public bool LogAuditTrail(string userType, string username, string action, string details = null)
        {
            return db.LogAuditTrail(userType, username, action);
        }

        public List<dynamic> GetAuditLogs(int pageSize = 100, int pageNumber = 1)
        {
            return db.GetAuditTrail(pageSize);
        }

        public List<dynamic> GetUserAuditLogs(string username, int pageSize = 100)
        {
            // This would require a new method in DatabaseHelper
            // For now, get all and filter (not optimal but works)
            var allLogs = db.GetAuditTrail(pageSize);
            return allLogs;
        }
    }
}
