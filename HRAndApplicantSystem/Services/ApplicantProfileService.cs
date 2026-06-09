using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Utilities;

namespace HRAndApplicantSystem.Services
{
    public class ApplicantProfileService
    {
        private readonly IApplicantRepository applicantRepository;

        public ApplicantProfileService(IApplicantRepository appRepo = null)
        {
            applicantRepository = appRepo ?? new ApplicantRepository(new DatabaseHelper());
        }

        public void ShowProfileMenu(Applicant applicant, string username)
        {
            bool profileMenuRunning = true;
            while (profileMenuRunning)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     MY PROFILE MANAGEMENT                    ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                Console.WriteLine("1. View My Profile");
                Console.WriteLine("2. Edit My Profile");
                Console.WriteLine("3. Back to Dashboard");
                Console.Write("\nChoose an option: ");

                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ViewProfile(applicant);
                        break;
                    case "2":
                        EditProfile(applicant, username);
                        // Refresh applicant data after editing
                        var updatedApplicant = applicantRepository.GetApplicantByUsername(username);
                        if (updatedApplicant != null)
                        {
                            applicant.FirstName = updatedApplicant.FirstName;
                            applicant.LastName = updatedApplicant.LastName;
                            applicant.ContactNo = updatedApplicant.ContactNo;
                            applicant.Address = updatedApplicant.Address;
                            applicant.Education = updatedApplicant.Education;
                            applicant.Skills = updatedApplicant.Skills;
                        }
                        break;
                    case "3":
                        profileMenuRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        System.Threading.Thread.Sleep(2000);
                        break;
                }
            }
        }

        public void ViewProfile(Applicant applicant)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     YOUR APPLICANT PROFILE                   ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.WriteLine("=== Personal Information ===\n");
            Console.WriteLine($"First Name:        {applicant.FirstName}");
            Console.WriteLine($"Last Name:         {applicant.LastName}");
            Console.WriteLine($"Username:          {applicant.Username}");

            Console.WriteLine("\n=== Contact Information ===\n");
            Console.WriteLine($"Contact Number:    {applicant.ContactNo}");
            Console.WriteLine($"Address:           {applicant.Address}");

            Console.WriteLine("\n=== Professional Information ===\n");
            Console.WriteLine($"Education:         {applicant.Education}");
            Console.WriteLine($"Skills:            {applicant.Skills}");

            Console.WriteLine("\n" + new string('=', 45));
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }

        public void EditProfile(Applicant applicant, string username)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     EDIT YOUR PROFILE                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            // Get current values to show as defaults
            var currentApplicant = applicantRepository.GetApplicantByUsername(username);
            if (currentApplicant == null)
            {
                Console.WriteLine("Error: Could not retrieve current profile.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            // Edit Personal Information
            Console.WriteLine("=== Personal Information ===\n");
            string firstName = InputValidator.GetOptionalInput($"First Name [{currentApplicant.FirstName}]: ", currentApplicant.FirstName);
            string lastName = InputValidator.GetOptionalInput($"Last Name [{currentApplicant.LastName}]: ", currentApplicant.LastName);

            // Edit Contact Information
            Console.WriteLine("\n=== Contact Information ===\n");
            string contactNo = InputValidator.GetOptionalInput($"Contact Number [{currentApplicant.ContactNo}]: ", currentApplicant.ContactNo);
            string address = InputValidator.GetOptionalInput($"Address [{currentApplicant.Address}]: ", currentApplicant.Address);

            // Edit Professional Information
            Console.WriteLine("\n=== Professional Information ===\n");
            string education = InputValidator.GetOptionalInput($"Education [{currentApplicant.Education}]: ", currentApplicant.Education);
            string skills = InputValidator.GetOptionalInput($"Skills [{currentApplicant.Skills}]: ", currentApplicant.Skills);

            // Create updated applicant object
            Applicant updatedApplicant = new Applicant
            {
                ApplicantID = currentApplicant.ApplicantID,
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                ContactNo = contactNo,
                Address = address,
                Education = education,
                Skills = skills
            };

            // Save changes
            if (applicantRepository.SaveApplicantInfo(username, updatedApplicant))
            {
                Console.WriteLine("\n✓ Profile updated successfully!");
                System.Threading.Thread.Sleep(2000);
            }
            else
            {
                Console.WriteLine("\n✗ Failed to update profile. Please try again.");
                System.Threading.Thread.Sleep(2000);
            }
        }
    }
}
