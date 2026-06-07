namespace HRAndApplicantSystem.Models
{
    /// <summary>
    /// Application status constants defining the workflow states
    /// </summary>
    public static class ApplicationStatus
    {
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
        public const string UnderReview = "Under Review";
        public const string Shortlisted = "Shortlisted";
        public const string InterviewScheduled = "Interview Scheduled";
        public const string Accepted = "Accepted";
        public const string Rejected = "Rejected";

        public static List<string> GetAllStatuses()
        {
            return new List<string>
            {
                Draft,
                Submitted,
                UnderReview,
                Shortlisted,
                InterviewScheduled,
                Accepted,
                Rejected
            };
        }

        public static bool IsEditableStatus(string status)
        {
            return status == Draft;
        }

        public static bool IsLockedStatus(string status)
        {
            return status != Draft && status != ""; // All statuses except Draft are locked
        }
    }

    /// <summary>
    /// Document type constants for application documents
    /// </summary>
    public static class DocumentType
    {
        public const string Resume = "Resume";
        public const string IdentityDocument = "ID";
        public const string Transcript = "Transcript";
        public const string Certificate = "Certificate";
        public const string CoverLetter = "Cover Letter";
        public const string Other = "Other";

        public static List<string> GetRequiredDocuments()
        {
            return new List<string>
            {
                Resume,
                IdentityDocument,
                Transcript,
                Certificate
            };
        }

        public static List<string> GetAllDocumentTypes()
        {
            return new List<string>
            {
                Resume,
                IdentityDocument,
                Transcript,
                Certificate,
                CoverLetter,
                Other
            };
        }
    }

    /// <summary>
    /// Document status constants
    /// </summary>
    public static class DocumentStatus
    {
        public const string Submitted = "Submitted";
        public const string Missing = "Missing";
        public const string Pending = "Pending";
        public const string Verified = "Verified";
        public const string Rejected = "Rejected";
    }
}
