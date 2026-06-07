namespace HRAndApplicantSystem.Models
{
    public class ApplicationDocument
    {
        public int DocumentID { get; set; }

        public int ApplicantID { get; set; }

        public int RequirementTypeID { get; set; }

        public string DocumentStatus { get; set; } // Submitted, Missing, Pending Verification

        public string Remarks { get; set; }
    }
}
