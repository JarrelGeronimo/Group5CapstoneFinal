using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Application repository implementation - Adaptor pattern wrapping DatabaseHelper
    /// </summary>
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly DatabaseHelper db;

        public ApplicationRepository(DatabaseHelper databaseHelper = null)
        {
            db = databaseHelper ?? new DatabaseHelper();
        }

        public int CreateDraftApplication(int applicantID, int jobID)
        {
            return db.CreateDraftApplication(applicantID, jobID);
        }

        public List<dynamic> GetApplicantApplications(int applicantID)
        {
            return db.GetApplicantApplications(applicantID);
        }

        public List<dynamic> GetApplicationsByStatus(string status)
        {
            return db.GetApplicationsByStatus(status);
        }

        public List<dynamic> GetApplicationsByJob(int jobID)
        {
            return db.GetApplicationsByJob(jobID);
        }

        public List<dynamic> GetPendingApplicationsForScreening()
        {
            return db.GetPendingApplicationsForScreening();
        }

        public bool UpdateApplicationStatus(int applicationID, string status, string reason, string updatedBy)
        {
            return db.UpdateApplicationStatus(applicationID, status, reason, updatedBy);
        }

        public bool DeleteApplication(int applicationID)
        {
            return db.DeleteApplication(applicationID);
        }

        public dynamic GetApplicationDetailsForScreening(int applicationID)
        {
            return db.GetApplicationDetailsForScreening(applicationID);
        }

        public bool CheckAllJobRequirementsSubmitted(int applicantID, int jobID)
        {
            return db.CheckAllJobRequirementsSubmitted(applicantID, jobID);
        }

        public Application GetApplicationByID(int applicationID)
        {
            var apps = db.GetApplicantApplications(-1);
            // This is a workaround since DatabaseHelper doesn't have GetApplicationByID
            // We'll add it to DatabaseHelper later
            throw new NotImplementedException("Use database query directly");
        }

        public bool HasApplicantAppliedForJob(int applicantID, int jobID)
        {
            return db.HasApplicantAppliedForJob(applicantID, jobID);
        }

        public bool SubmitJobApplication(int applicantID, int jobID)
        {
            return db.SubmitJobApplication(applicantID, jobID);
        }
    }
}
