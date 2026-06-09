using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Services.Business
{
    /// <summary>
    /// Business logic for applicant profile management.
    /// Handles profile data retrieval, updates, and validation.
    /// No UI/Console rendering - returns data for presentation layer.
    /// </summary>
    public class ApplicantProfileBusinessService
    {
        private readonly IApplicantRepository applicantRepository;
        private readonly DatabaseHelper db;

        public ApplicantProfileBusinessService(IApplicantRepository appRepo = null)
        {
            applicantRepository = appRepo ?? new ApplicantRepository(new DatabaseHelper());
            db = new DatabaseHelper();
        }

        /// <summary>
        /// Get applicant details by ID
        /// </summary>
        public Applicant GetApplicantDetails(int applicantId)
        {
            return db.GetApplicantByID(applicantId);
        }

        /// <summary>
        /// Update applicant first name
        /// </summary>
        public bool UpdateFirstName(int applicantId, string newFirstName, string username)
        {
            if (string.IsNullOrWhiteSpace(newFirstName))
                return false;
            
            var applicant = db.GetApplicantByID(applicantId);
            if (applicant == null) return false;
            
            applicant.FirstName = newFirstName.Trim();
            return db.UpdateApplicantInfo(username, applicant);
        }

        /// <summary>
        /// Update applicant last name
        /// </summary>
        public bool UpdateLastName(int applicantId, string newLastName, string username)
        {
            if (string.IsNullOrWhiteSpace(newLastName))
                return false;
            
            var applicant = db.GetApplicantByID(applicantId);
            if (applicant == null) return false;
            
            applicant.LastName = newLastName.Trim();
            return db.UpdateApplicantInfo(username, applicant);
        }

        /// <summary>
        /// Update applicant contact number
        /// </summary>
        public bool UpdateContactNo(int applicantId, string newContactNo, string username)
        {
            if (string.IsNullOrWhiteSpace(newContactNo))
                return false;

            var applicant = db.GetApplicantByID(applicantId);
            if (applicant == null) return false;
            
            applicant.ContactNo = newContactNo.Trim();
            return db.UpdateApplicantInfo(username, applicant);
        }

        /// <summary>
        /// Update applicant address
        /// </summary>
        public bool UpdateAddress(int applicantId, string newAddress, string username)
        {
            if (string.IsNullOrWhiteSpace(newAddress))
                return false;

            var applicant = db.GetApplicantByID(applicantId);
            if (applicant == null) return false;
            
            applicant.Address = newAddress.Trim();
            return db.UpdateApplicantInfo(username, applicant);
        }

        /// <summary>
        /// Update applicant education
        /// </summary>
        public bool UpdateEducation(int applicantId, string newEducation, string username)
        {
            if (string.IsNullOrWhiteSpace(newEducation))
                return false;

            var applicant = db.GetApplicantByID(applicantId);
            if (applicant == null) return false;
            
            applicant.Education = newEducation.Trim();
            return db.UpdateApplicantInfo(username, applicant);
        }

        /// <summary>
        /// Update applicant skills
        /// </summary>
        public bool UpdateSkills(int applicantId, string newSkills, string username)
        {
            if (string.IsNullOrWhiteSpace(newSkills))
                return false;

            var applicant = db.GetApplicantByID(applicantId);
            if (applicant == null) return false;
            
            applicant.Skills = newSkills.Trim();
            return db.UpdateApplicantInfo(username, applicant);
        }

        /// <summary>
        /// Get profile completion percentage
        /// </summary>
        public int GetProfileCompletionPercentage(Applicant applicant)
        {
            int fields = 0;
            int completed = 0;

            if (!string.IsNullOrWhiteSpace(applicant.FirstName)) completed++;
            fields++;

            if (!string.IsNullOrWhiteSpace(applicant.LastName)) completed++;
            fields++;

            if (!string.IsNullOrWhiteSpace(applicant.ContactNo)) completed++;
            fields++;

            if (!string.IsNullOrWhiteSpace(applicant.Address)) completed++;
            fields++;

            if (!string.IsNullOrWhiteSpace(applicant.Education)) completed++;
            fields++;

            if (!string.IsNullOrWhiteSpace(applicant.Skills)) completed++;
            fields++;

            return (completed * 100) / fields;
        }

        /// <summary>
        /// Check if phone number format is valid
        /// </summary>
        public bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.Length >= 10;
        }

        /// <summary>
        /// Get list of fields needing update
        /// </summary>
        public List<string> GetIncompleteFields(Applicant applicant)
        {
            var incomplete = new List<string>();

            if (string.IsNullOrWhiteSpace(applicant.FirstName))
                incomplete.Add("First Name");

            if (string.IsNullOrWhiteSpace(applicant.LastName))
                incomplete.Add("Last Name");

            if (string.IsNullOrWhiteSpace(applicant.ContactNo))
                incomplete.Add("Contact Number");

            if (string.IsNullOrWhiteSpace(applicant.Address))
                incomplete.Add("Address");

            if (string.IsNullOrWhiteSpace(applicant.Education))
                incomplete.Add("Education");

            if (string.IsNullOrWhiteSpace(applicant.Skills))
                incomplete.Add("Skills");

            return incomplete;
        }
    }
}
