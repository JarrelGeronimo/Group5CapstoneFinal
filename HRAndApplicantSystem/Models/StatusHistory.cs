namespace HRAndApplicantSystem.Models
{
    public class StatusHistory
    {
        public int HistoryID { get; set; }

        public int ApplicationID { get; set; }

        public string Status { get; set; }

        public string Remarks { get; set; }

        public DateTime DateChanged { get; set; }

        public string ChangedBy { get; set; } // Username or UserID who made the change
    }
}