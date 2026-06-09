using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Services.Business
{
    /// <summary>
    /// Business logic for screening operations.
    /// Handles screening data retrieval and decision logic.
    /// No UI/Console rendering - returns data for presentation layer.
    /// </summary>
    public class ScreeningBusinessService
    {
        private readonly DatabaseHelper db;

        public ScreeningBusinessService()
        {
            db = new DatabaseHelper();
        }

        /// <summary>
        /// Get application details for screening
        /// </summary>
        public dynamic GetApplicationDetailsForScreening(int applicationId)
        {
            return db.GetApplicationDetailsForScreening(applicationId);
        }

        /// <summary>
        /// Get applicant documents for review
        /// </summary>
        public List<dynamic> GetApplicantDocuments(int applicantId, int jobId)
        {
            return db.GetApplicantDocuments(applicantId, jobId);
        }

        /// <summary>
        /// Screen application with decision
        /// </summary>
        public bool ScreenApplication(int applicationId, string result, string remarks, string hrUsername)
        {
            return db.ScreenApplication(applicationId, result, remarks, hrUsername);
        }

        /// <summary>
        /// Update application status to under review
        /// </summary>
        public bool UpdateToUnderReview(int applicationId, string hrUsername)
        {
            return db.UpdateApplicationStatus(applicationId, ApplicationStatus.UnderReview, 
                "HR screening started", hrUsername);
        }

        /// <summary>
        /// Get screening result display message
        /// </summary>
        public string GetResultMessage(string result)
        {
            return result switch
            {
                "Qualified" => "Shortlisted (Qualified)",
                "Not Qualified" => "Rejected (Not Qualified)",
                _ => result
            };
        }

        /// <summary>
        /// Validate screening input
        /// </summary>
        public bool IsValidScreeningDecision(string choice)
        {
            return choice == "1" || choice == "2" || choice == "3";
        }

        /// <summary>
        /// Get decision from choice
        /// </summary>
        public (string result, string newStatus) GetDecisionFromChoice(string choice)
        {
            return choice == "1" 
                ? ("Qualified", ApplicationStatus.Shortlisted)
                : ("Not Qualified", ApplicationStatus.Rejected);
        }
    }
}
