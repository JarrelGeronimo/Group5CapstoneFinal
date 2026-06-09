using HRAndApplicantSystem.Models;
using ApplicationModel = HRAndApplicantSystem.Models.Application;

namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Repository interface for application data access operations
    /// </summary>
    public interface IApplicationRepository
    {
        /// <summary>
        /// Creates a new draft application
        /// </summary>
        int CreateDraftApplication(int applicantID, int jobID);

        /// <summary>
        /// Gets all applications for an applicant
        /// </summary>
        List<dynamic> GetApplicantApplications(int applicantID);

        /// <summary>
        /// Gets applications filtered by status
        /// </summary>
        List<dynamic> GetApplicationsByStatus(string status);

        /// <summary>
        /// Gets applications for a specific job
        /// </summary>
        List<dynamic> GetApplicationsByJob(int jobID);

        /// <summary>
        /// Gets pending applications for screening
        /// </summary>
        List<dynamic> GetPendingApplicationsForScreening();

        /// <summary>
        /// Updates application status
        /// </summary>
        bool UpdateApplicationStatus(int applicationID, string status, string reason, string updatedBy);

        /// <summary>
        /// Deletes an application
        /// </summary>
        bool DeleteApplication(int applicationID);

        /// <summary>
        /// Gets application details for screening
        /// </summary>
        dynamic GetApplicationDetailsForScreening(int applicationID);

        /// <summary>
        /// Checks if all job requirements have been submitted
        /// </summary>
        bool CheckAllJobRequirementsSubmitted(int applicantID, int jobID);

        /// <summary>
        /// Gets application by ID
        /// </summary>
        ApplicationModel GetApplicationByID(int applicationID);

        /// <summary>
        /// Checks if applicant has applied for a specific job
        /// </summary>
        bool HasApplicantAppliedForJob(int applicantID, int jobID);

        /// <summary>
        /// Submits an application for an applicant
        /// </summary>
        bool SubmitJobApplication(int applicantID, int jobID);
    }
}
