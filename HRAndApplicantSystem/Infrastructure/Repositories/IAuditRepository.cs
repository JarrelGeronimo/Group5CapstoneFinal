namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Repository interface for audit logging operations
    /// </summary>
    public interface IAuditRepository
    {
        /// <summary>
        /// Logs an audit trail entry
        /// </summary>
        bool LogAuditTrail(string userType, string username, string action, string details = null);

        /// <summary>
        /// Gets audit log entries
        /// </summary>
        List<dynamic> GetAuditLogs(int pageSize = 100, int pageNumber = 1);

        /// <summary>
        /// Gets audit logs for a specific user
        /// </summary>
        List<dynamic> GetUserAuditLogs(string username, int pageSize = 100);
    }
}
