namespace HRAndApplicantSystem.Models
{
    public class Application
    {
        public int ApplicationID { get; set; }

        public int ApplicantID { get; set; }

        public int JobID { get; set; }

        public string ApplicationStatus { get; set; }

        public DateTime DateApplied { get; set; }
    }
}