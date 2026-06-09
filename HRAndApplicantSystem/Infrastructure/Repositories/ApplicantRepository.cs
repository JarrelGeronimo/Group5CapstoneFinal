using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Applicant repository implementation - Adaptor pattern wrapping DatabaseHelper
    /// </summary>
    public class ApplicantRepository : IApplicantRepository
    {
        private readonly DatabaseHelper db;

        public ApplicantRepository(DatabaseHelper databaseHelper = null)
        {
            db = databaseHelper ?? new DatabaseHelper();
        }

        public Applicant GetApplicantByUsername(string username)
        {
            return db.GetApplicantByUsername(username);
        }

        public Applicant GetApplicantByID(int applicantID)
        {
            return db.GetApplicantByID(applicantID);
        }

        public bool SaveApplicantInfo(string username, Applicant applicant)
        {
            return db.SaveApplicantInfo(username, applicant);
        }

        public bool UpdateApplicantInfo(string username, Applicant applicant)
        {
            return db.UpdateApplicantInfo(username, applicant);
        }
    }
}
