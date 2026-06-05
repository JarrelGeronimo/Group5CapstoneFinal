using HRAndApplicantSystem.Database;

namespace HRAndApplicantSystem.Applicant
{
    public class ApplicationService
    {
        private DatabaseHelper db = new DatabaseHelper();

        public void CollectApplicantInfo(string username)
        {
            Console.WriteLine("\n=== Complete Your Applicant Profile ===\n");

            Console.Write("First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("Contact Number: ");
            string contactNo = Console.ReadLine();

            Console.Write("Address: ");
            string address = Console.ReadLine();

            Console.Write("Education Background: ");
            string education = Console.ReadLine();

            Console.Write("Skills: ");
            string skills = Console.ReadLine();

            Applicant applicant = new Applicant
            {
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                ContactNo = contactNo,
                Address = address,
                Education = education,
                Skills = skills
            };

            if (db.SaveApplicantInfo(username, applicant))
            {
                Console.WriteLine("\nProfile saved successfully!");
            }
            else
            {
                Console.WriteLine("\nFailed to save profile. Please try again.");
            }
        }

        public void UpdateApplicantProfile(string username, Applicant existingApplicant)
        {
            Console.WriteLine("\n=== Update Your Applicant Profile ===\n");

            Console.Write($"First Name ({existingApplicant.FirstName}): ");
            string firstName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(firstName))
                firstName = existingApplicant.FirstName;

            Console.Write($"Last Name ({existingApplicant.LastName}): ");
            string lastName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(lastName))
                lastName = existingApplicant.LastName;

            Console.Write($"Contact Number ({existingApplicant.ContactNo}): ");
            string contactNo = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(contactNo))
                contactNo = existingApplicant.ContactNo;

            Console.Write($"Address ({existingApplicant.Address}): ");
            string address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address))
                address = existingApplicant.Address;

            Console.Write($"Education Background ({existingApplicant.Education}): ");
            string education = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(education))
                education = existingApplicant.Education;

            Console.Write($"Skills ({existingApplicant.Skills}): ");
            string skills = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(skills))
                skills = existingApplicant.Skills;

            Applicant updatedApplicant = new Applicant
            {
                ApplicantID = existingApplicant.ApplicantID,  // Preserve the ID
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                ContactNo = contactNo,
                Address = address,
                Education = education,
                Skills = skills
            };

            if (db.SaveApplicantInfo(username, updatedApplicant))
            {
                Console.WriteLine("\nProfile updated successfully!");
            }
            else
            {
                Console.WriteLine("\nFailed to update profile. Please try again.");
            }
        }

        public Applicant GetApplicantInfo(string username)
        {
            return db.GetApplicantByUsername(username);
        }

        public bool UpdateApplicantInfo(string username, Applicant applicant)
        {
            return db.UpdateApplicantInfo(username, applicant);
        }

        public void ApplyForJob()
        {
            // TODO: Implement job application functionality
        }

        public void GetApplications()
        {
            // TODO: Implement getting applicant's applications
        }

        public bool HasExistingApplication()
        {
            // TODO: Implement check for existing application
            return false;
        }
    }
}