namespace HRApplicantSystem.Models
{
    public class Applicant
    {
        public int ApplicantID { get; set; }

        public int AccountID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Gender { get; set; }

        public string ContactNumber { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        public string Education { get; set; }

        public string Skills { get; set; }

        public string WorkExperience { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
