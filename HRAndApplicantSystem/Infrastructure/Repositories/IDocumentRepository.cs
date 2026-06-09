namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Repository interface for document data access operations
    /// </summary>
    public interface IDocumentRepository
    {
        /// <summary>
        /// Submits or updates an applicant document
        /// </summary>
        bool SubmitApplicantDocument(int applicantID, int jobID, int requirementTypeID, string remarks, string status);

        /// <summary>
        /// Gets all documents for an applicant for a specific job
        /// </summary>
        List<dynamic> GetApplicantDocuments(int applicantID, int jobID);

        /// <summary>
        /// Gets job-specific requirements
        /// </summary>
        List<dynamic> GetJobSpecificRequirements(int jobID);

        /// <summary>
        /// Gets all requirements for a job
        /// </summary>
        List<dynamic> GetJobRequirements(int jobID);
    }
}
