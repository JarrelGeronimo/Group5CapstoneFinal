using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Utilities;

namespace HRAndApplicantSystem.Services
{
    public class ApplicationService
    {
        private DatabaseHelper db = new DatabaseHelper();

        public void CollectApplicantInfo(string username)
        {
            Console.WriteLine("\n=== Complete Your Applicant Profile ===\n");

            string firstName = InputValidator.GetValidatedInput("First Name: ", "First Name");
            string lastName = InputValidator.GetValidatedInput("Last Name: ", "Last Name");
            string contactNo = InputValidator.GetValidatedInput("Contact Number: ", "Contact Number");
            string address = InputValidator.GetValidatedInput("Address: ", "Address");
            string education = InputValidator.GetValidatedInput("Education Background: ", "Education");
            string skills = InputValidator.GetValidatedInput("Skills: ", "Skills");

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

            string firstName = InputValidator.GetOptionalInput($"First Name ({existingApplicant.FirstName}): ", existingApplicant.FirstName);
            string lastName = InputValidator.GetOptionalInput($"Last Name ({existingApplicant.LastName}): ", existingApplicant.LastName);
            string contactNo = InputValidator.GetOptionalInput($"Contact Number ({existingApplicant.ContactNo}): ", existingApplicant.ContactNo);
            string address = InputValidator.GetOptionalInput($"Address ({existingApplicant.Address}): ", existingApplicant.Address);
            string education = InputValidator.GetOptionalInput($"Education Background ({existingApplicant.Education}): ", existingApplicant.Education);
            string skills = InputValidator.GetOptionalInput($"Skills ({existingApplicant.Skills}): ", existingApplicant.Skills);

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

        public void BrowseJobVacancies(Applicant applicant)
        {
            if (applicant == null || applicant.ApplicantID <= 0)
            {
                Console.WriteLine("Error: Applicant information not found.");
                return;
            }

            while (true)
            {
                Console.WriteLine("\n=== Browse Job Vacancies ===\n");

                var vacancies = db.GetAllJobVacancies();

                if (vacancies.Count == 0)
                {
                    Console.WriteLine("No job vacancies available at this time.");
                    break;
                }

                for (int i = 0; i < vacancies.Count; i++)
                {
                    var job = vacancies[i];
                    Console.WriteLine($"\n{i + 1}. {job.JobTitle}");
                    Console.WriteLine($"   Job ID: {job.JobID}");
                    Console.WriteLine($"   Details: {job.JobDetail}");
                    Console.WriteLine($"   Status: {job.Status}");
                }

                Console.WriteLine($"\n{vacancies.Count + 1}. Back to Dashboard");
                Console.Write("\nSelect a job to apply or choose option to go back: ");

                if (int.TryParse(Console.ReadLine()?.Trim(), out int choice) && choice >= 1 && choice <= vacancies.Count)
                {
                    var selectedJob = vacancies[choice - 1];
                    Console.Write($"\nDo you want to apply for {selectedJob.JobTitle}? (yes/no): ");
                    string confirm = (Console.ReadLine()?.Trim() ?? string.Empty).ToLower();

                    if (confirm == "yes" || confirm == "y")
                    {
                        if (db.SubmitJobApplication(applicant.ApplicantID, selectedJob.JobID))
                        {
                            Console.WriteLine($"✓ Successfully applied for {selectedJob.JobTitle}!");
                        }
                        else
                        {
                            Console.WriteLine("Failed to submit application. You may have already applied for this job.");
                        }
                    }
                }
                else if (choice == vacancies.Count + 1)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid selection. Please try again.");
                }
            }
        }

        public void ViewMyApplications(Applicant applicant)
        {
            if (applicant == null || applicant.ApplicantID <= 0)
            {
                Console.WriteLine("Error: Applicant information not found.");
                return;
            }

            Console.WriteLine("\n=== My Applications ===\n");

            var applications = db.GetApplicantApplications(applicant.ApplicantID);

            if (applications.Count == 0)
            {
                Console.WriteLine("You haven't applied for any jobs yet.");
                return;
            }

            Console.WriteLine($"{"Job Title",-40} {"Status",-15} {"Date Applied",-20}");
            Console.WriteLine(new string('-', 75));

            foreach (var app in applications)
            {
                string dateApplied = app.DateApplied.ToString("yyyy-MM-dd HH:mm");
                Console.WriteLine($"{app.JobTitle,-40} {app.Status,-15} {dateApplied,-20}");
            }

            Console.WriteLine();
        }

        // Placeholder methods for future usage
        public void ApplyForJob()
        {
            // TODO: Placeholder for future implementation
        }

        public void GetApplications()
        {
            // TODO: Placeholder for future implementation
        }

        public bool HasExistingApplication()
        {
            // TODO: Placeholder for future implementation
            return false;
        }
    }
}
