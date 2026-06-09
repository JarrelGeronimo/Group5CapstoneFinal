using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;

namespace HRAndApplicantSystem.Services
{
    public class ApplicantSearchService
    {
        private readonly IApplicantRepository applicantRepository;
        private readonly DatabaseHelper db;

        public ApplicantSearchService(IApplicantRepository appRepo = null)
        {
            applicantRepository = appRepo ?? new ApplicantRepository(new DatabaseHelper());
            db = new DatabaseHelper();
        }

        /// <summary>
        /// Display Applicant Search & Profile Menu
        /// </summary>
        public void ShowApplicantSearchMenu()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     APPLICANT SEARCH & PROFILES              ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                Console.WriteLine("1. Search by Name");
                Console.WriteLine("2. Search by Skills");
                Console.WriteLine("3. View All Applicants");
                Console.WriteLine("4. View Applicant Profile");
                Console.WriteLine("5. Back\n");

                Console.Write("Choose an option: ");
                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        SearchByName();
                        break;
                    case "2":
                        SearchBySkills();
                        break;
                    case "3":
                        ViewAllApplicants();
                        break;
                    case "4":
                        ViewApplicantProfile();
                        break;
                    case "5":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("\nInvalid option. Please try again.");
                        System.Threading.Thread.Sleep(1500);
                        break;
                }
            }
        }

        private void SearchByName()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     SEARCH APPLICANTS BY NAME                ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.Write("Enter applicant name (or part of name): ");
            string searchName = Console.ReadLine()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(searchName))
            {
                Console.WriteLine("\nPlease enter a valid name.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            var results = db.SearchApplicantsByName(searchName);

            if (results.Count == 0)
            {
                Console.WriteLine($"\nNo applicants found matching '{searchName}'.");
            }
            else
            {
                Console.WriteLine($"\nFound {results.Count} applicant(s):\n");
                DisplayApplicantsList(results);
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void SearchBySkills()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     SEARCH APPLICANTS BY SKILLS              ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.Write("Enter skill keyword: ");
            string skillKeyword = Console.ReadLine()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(skillKeyword))
            {
                Console.WriteLine("\nPlease enter a valid skill.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            var results = db.SearchApplicantsBySkills(skillKeyword);

            if (results.Count == 0)
            {
                Console.WriteLine($"\nNo applicants found with skill '{skillKeyword}'.");
            }
            else
            {
                Console.WriteLine($"\nFound {results.Count} applicant(s) with '{skillKeyword}':\n");
                DisplayApplicantsList(results);
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void ViewAllApplicants()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     ALL APPLICANTS                           ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var allApplicants = db.GetAllApplicants(1, 100); // Get first 100

            if (allApplicants.Count == 0)
            {
                Console.WriteLine("No applicants found.");
            }
            else
            {
                Console.WriteLine($"Total Applicants: {allApplicants.Count}\n");
                DisplayApplicantsList(allApplicants);
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void ViewApplicantProfile()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     VIEW APPLICANT PROFILE                   ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.Write("Enter Applicant ID: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int applicantId) || applicantId <= 0)
            {
                Console.WriteLine("\nInvalid Applicant ID.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            var profile = db.GetApplicantFullProfile(applicantId);

            if (profile == null)
            {
                Console.WriteLine($"\nApplicant with ID {applicantId} not found.");
            }
            else
            {
                DisplayFullProfile(profile);
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void DisplayApplicantsList(List<dynamic> applicants)
        {
            Console.WriteLine($"{"#",-3} {"Name",-20} {"Contact",-15} {"Education",-20} {"Apps",-5}");
            Console.WriteLine(new string('-', 80));

            foreach (var app in applicants)
            {
                string name = $"{app.FirstName} {app.LastName}";
                string contact = app.ContactNo?.ToString() ?? "N/A";
                string education = app.Education?.ToString() ?? "N/A";
                int appCount = app.ApplicationCount ?? 0;
                
                Console.WriteLine($"{applicants.IndexOf(app) + 1,-3} {name,-20} {contact,-15} {education,-20} {appCount,-5}");
            }
        }

        private void DisplayFullProfile(dynamic profile)
        {
            Console.WriteLine($"\n{'='} APPLICANT PROFILE {'='}\n");
            Console.WriteLine($"Name             : {profile.FirstName} {profile.LastName}");
            Console.WriteLine($"Contact          : {profile.ContactNo}");
            Console.WriteLine($"Address          : {profile.Address}");
            Console.WriteLine($"Education        : {profile.Education}");
            Console.WriteLine($"Skills           : {profile.Skills}");

            Console.WriteLine($"\n{'='} APPLICATION HISTORY {'='}\n");

            var applications = (List<dynamic>)profile.Applications;
            if (applications.Count == 0)
            {
                Console.WriteLine("No applications found.");
            }
            else
            {
                Console.WriteLine($"Total Applications: {profile.ApplicationCount}\n");
                foreach (var app in applications)
                {
                    Console.WriteLine($"  Job: {app.JobTitle}");
                    Console.WriteLine($"  Status: {app.Status}");
                    Console.WriteLine($"  Applied: {app.DateApplied:yyyy-MM-dd}");
                    Console.WriteLine(new string('-', 40));
                }
            }
        }
    }
}
