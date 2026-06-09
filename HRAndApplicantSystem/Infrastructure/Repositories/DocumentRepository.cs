using HRAndApplicantSystem.Database;

namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Document repository implementation - Adaptor pattern wrapping DatabaseHelper
    /// </summary>
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DatabaseHelper db;

        public DocumentRepository(DatabaseHelper databaseHelper = null)
        {
            db = databaseHelper ?? new DatabaseHelper();
        }

        public bool SubmitApplicantDocument(int applicantID, int jobID, int requirementTypeID, string remarks, string status)
        {
            return db.SubmitApplicantDocument(applicantID, jobID, requirementTypeID, remarks, status);
        }

        public List<dynamic> GetApplicantDocuments(int applicantID, int jobID)
        {
            return db.GetApplicantDocuments(applicantID, jobID);
        }

        public List<dynamic> GetJobSpecificRequirements(int jobID)
        {
            return db.GetJobSpecificRequirements(jobID);
        }

        public List<dynamic> GetJobRequirements(int jobID)
        {
            return db.GetJobRequirements(jobID);
        }
    }
}
