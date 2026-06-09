using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Repository interface for applicant data access operations
    /// </summary>
    public interface IApplicantRepository
    {
        /// <summary>
        /// Gets applicant information by username
        /// </summary>
        Applicant GetApplicantByUsername(string username);

        /// <summary>
        /// Gets applicant information by applicant ID
        /// </summary>
        Applicant GetApplicantByID(int applicantID);

        /// <summary>
        /// Saves new applicant information
        /// </summary>
        bool SaveApplicantInfo(string username, Applicant applicant);

        /// <summary>
        /// Updates existing applicant information
        /// </summary>
        bool UpdateApplicantInfo(string username, Applicant applicant);
    }
}
