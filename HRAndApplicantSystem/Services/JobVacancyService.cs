using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Utilities;

namespace HRAndApplicantSystem.Services
{
    public class JobVacancyService
    {
        private readonly DatabaseHelper db;

        public JobVacancyService()
        {
            db = new DatabaseHelper();
        }

        public void BrowseJobVacancies(Applicant applicant)
        {
            if (applicant == null || applicant.ApplicantID <= 0)
            {
                Console.WriteLine("Error: Applicant information not found.");
                return;
            }

            bool browsing = true;
            while (browsing)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     BROWSE JOB VACANCIES                     ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                Console.WriteLine("1. View All Job Vacancies");
                Console.WriteLine("2. Search Jobs by Title");
                Console.WriteLine("3. Filter Jobs by Status");
                Console.WriteLine("4. Back to Dashboard");
                Console.Write("\nChoose an option: ");

                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ViewAllJobs(applicant);
                        break;
                    case "2":
                        SearchJobsByTitle(applicant);
                        break;
                    case "3":
                        FilterJobsByStatus(applicant);
                        break;
                    case "4":
                        browsing = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        System.Threading.Thread.Sleep(2000);
                        break;
                }
            }
        }

        private void ViewAllJobs(Applicant applicant)
        {
            DisplayJobs(db.GetAllJobVacancies(), applicant);
        }

        private void SearchJobsByTitle(Applicant applicant)
        {
            Console.Write("Enter job title to search (e.g., 'Developer', 'Manager'): ");
            string searchTerm = Console.ReadLine()?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(searchTerm))
            {
                Console.WriteLine("Search term cannot be empty.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            var allJobs = db.GetAllJobVacancies();
            var filteredJobs = allJobs
                .Where(j => j.JobTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filteredJobs.Count == 0)
            {
                Console.WriteLine($"\nNo jobs found matching '{searchTerm}'.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            DisplayJobs(filteredJobs, applicant);
        }

        private void FilterJobsByStatus(Applicant applicant)
        {
            Console.WriteLine("\nFilter by Status:");
            Console.WriteLine("1. Open");
            Console.WriteLine("2. Closed");
            Console.WriteLine("3. On Hold");
            Console.Write("Select status: ");

            string statusChoice = Console.ReadLine()?.Trim() ?? string.Empty;
            string status = statusChoice switch
            {
                "1" => "Open",
                "2" => "Closed",
                "3" => "On Hold",
                _ => null
            };

            if (status == null)
            {
                Console.WriteLine("Invalid status selection.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            var allJobs = db.GetAllJobVacancies();
            var filteredJobs = allJobs
                .Where(j => j.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filteredJobs.Count == 0)
            {
                Console.WriteLine($"\nNo jobs found with status '{status}'.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            DisplayJobs(filteredJobs, applicant);
        }

        private void DisplayJobs(List<JobVacancy> vacancies, Applicant applicant)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     AVAILABLE JOB POSITIONS                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            if (vacancies.Count == 0)
            {
                Console.WriteLine("No job vacancies available at this time.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            bool selecting = true;
            while (selecting)
            {
                Console.Clear();
                Console.WriteLine($"Found {vacancies.Count} job(s)\n");
                Console.WriteLine($"{"#",-3} {"Job Title",-30} {"Status",-15}");
                Console.WriteLine(new string('-', 50));

                for (int i = 0; i < vacancies.Count; i++)
                {
                    var job = vacancies[i];
                    Console.WriteLine($"{i + 1,-3} {job.JobTitle,-30} {job.Status,-15}");
                }

                Console.WriteLine($"\n{vacancies.Count + 1}. Back");
                Console.Write("\nSelect a job to view details or apply: ");

                if (int.TryParse(Console.ReadLine()?.Trim(), out int choice) && choice >= 1 && choice <= vacancies.Count)
                {
                    var selectedJob = vacancies[choice - 1];
                    ShowJobDetailsAndApply(selectedJob, applicant);
                }
                else if (choice == vacancies.Count + 1)
                {
                    selecting = false;
                }
                else
                {
                    Console.WriteLine("Invalid selection. Please try again.");
                    System.Threading.Thread.Sleep(2000);
                }
            }
        }

        private void ShowJobDetailsAndApply(JobVacancy job, Applicant applicant)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     JOB DETAILS                              ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.WriteLine($"Job Title: {job.JobTitle}");
            Console.WriteLine($"Job ID: {job.JobID}");
            Console.WriteLine($"Status: {job.Status}");
            Console.WriteLine($"\nDescription:\n{job.JobDetail}");

            Console.WriteLine("\n" + new string('=', 45));
            Console.Write("\nWould you like to apply for this position? (yes/no): ");
            string confirm = (Console.ReadLine()?.Trim() ?? string.Empty).ToLower();

            if (confirm == "yes" || confirm == "y")
            {
                ApplyForJob(applicant.ApplicantID, job, applicant);
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ApplyForJob(int applicantId, JobVacancy job, Applicant applicant)
        {
            if (db.SubmitJobApplication(applicantId, job.JobID))
            {
                Console.WriteLine($"\n✓ Successfully applied for {job.JobTitle}!");
                Console.WriteLine("Your application has been submitted to HR for review.\n");

                // Show job requirements and allow document submission
                Console.WriteLine("This job has document requirements.\n");
                Console.Write("Would you like to submit required documents now? (yes/no): ");
                string choice = (Console.ReadLine()?.Trim() ?? "no").ToLower();

                if (choice == "yes" || choice == "y")
                {
                    DocumentSubmissionService docService = new DocumentSubmissionService();
                    docService.ManageDocumentSubmissions(applicantId, job.JobID);
                    Console.WriteLine("\nYour documents have been saved. You can update them anytime from your dashboard.");
                }
                else
                {
                    Console.WriteLine("You can submit documents later from 'View My Applications'.");
                }
            }
            else
            {
                Console.WriteLine($"\n✗ Failed to submit application.");
                Console.WriteLine("You may have already applied for this job.");
            }
        }
    }
}
