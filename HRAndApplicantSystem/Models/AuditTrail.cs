using System;

namespace HRAndApplicantSystem.Models
{
    /// <summary>
    /// Represents an audit trail entry tracking user actions in the system
    /// </summary>
    public class AuditTrail
    {
        /// <summary>
        /// Unique identifier for the audit record
        /// </summary>
        public int AuditID { get; set; }

        /// <summary>
        /// Type of user who performed the action (Applicant, HR Staff, HR Manager, Admin)
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// User ID who performed the action
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Username of the user who performed the action
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Description of the action performed
        /// Examples: "Submitted Application", "Rejected Applicant", "Scheduled Interview"
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Timestamp when the action was performed
        /// </summary>
        public DateTime ActionDate { get; set; }

        public AuditTrail()
        {
            ActionDate = DateTime.Now;
        }

        public AuditTrail(int auditID, string userType, int userID, string username, string action, DateTime actionDate)
        {
            AuditID = auditID;
            UserType = userType ?? "Unknown";
            UserID = userID;
            Username = username ?? "Unknown";
            Action = action ?? "Unknown";
            ActionDate = actionDate;
        }

        public override string ToString()
        {
            return $"[{AuditID}] {ActionDate:yyyy-MM-dd HH:mm:ss} | {Username} ({UserType}) | {Action}";
        }
    }
}
