using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Services
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
                    Console.WriteLine($"   Available Slots: {job.JobSlots}");
                    Console.WriteLine($"   Status: {(job.JobSlots > 0 ? "Open" : "Closed")}");
                }

                Console.WriteLine($"\n{vacancies.Count + 1}. Back to Dashboard");
                Console.Write("\nSelect a job to apply or choose option to go back: ");

                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= vacancies.Count)
                {
                    var selectedJob = vacancies[choice - 1];
                    Console.Write($"\nDo you want to apply for {selectedJob.JobTitle}? (yes/no): ");
                    string confirm = Console.ReadLine()?.ToLower();

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

        public void ApplyForJob()
        {
            // This method is now replaced by BrowseJobVacancies()
        }

        public void GetApplications()
        {
            // This method is now replaced by ViewMyApplications()
        }

        public bool HasExistingApplication()
        {
            // TODO: Implement check for existing application
            return false;
        }
    }
}