using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Utilities;

namespace HRAndApplicantSystem.Services
{
    /// <summary>
    /// Application Draft Service - Not used with current database schema
    /// The database schema does not have a Draft status. Applications go directly to Submitted.
    /// Document uploads are managed via ApplicantDocuments table with RequirementTypes.
    /// </summary>
    public class ApplicationDraftService
    {
        private readonly DatabaseHelper db;

        public ApplicationDraftService()
        {
            db = new DatabaseHelper();
        }

        [Obsolete("Draft applications not supported in current schema. Applications go directly to Submitted status.")]
        public void ManageDraftApplication(Application application, Applicant applicant)
        {
            Console.WriteLine("Draft application management is not available in the current system.");
            Console.WriteLine("Applications are submitted directly to HR for review.");
        }
    }
}
