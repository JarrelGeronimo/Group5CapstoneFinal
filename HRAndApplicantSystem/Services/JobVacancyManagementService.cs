using System;
using System.Collections.Generic;
using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Services
{
    public class JobVacancyManagementService
    {
        private readonly DatabaseHelper db;

        public JobVacancyManagementService()
        {
            db = new DatabaseHelper();
        }

        /// <summary>
        /// Displays the job vacancy management menu for managers and admins
        /// </summary>
        public void ShowJobVacancyManagementMenu()
        {
            bool managing = true;

            while (managing)
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════════════╗");
                Console.WriteLine("║   JOB VACANCY MANAGEMENT                       ║");
                Console.WriteLine("╚════════════════════════════════════════════════╝\n");

                Console.WriteLine("1. View All Job Vacancies");
                Console.WriteLine("2. Create New Job Vacancy");
                Console.WriteLine("3. Edit Job Vacancy");
                Console.WriteLine("4. Close Job Vacancy (Set Status to Closed)");
                Console.WriteLine("5. Delete Job Vacancy");
                Console.WriteLine("6. Back to Dashboard\n");

                Console.Write("Choose an option: ");
                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ViewAllJobVacancies();
                        break;
                    case "2":
                        CreateNewJobVacancy();
                        break;
                    case "3":
                        EditJobVacancy();
                        break;
                    case "4":
                        CloseJobVacancy();
                        break;
                    case "5":
                        DeleteJobVacancy();
                        break;
                    case "6":
                        managing = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        System.Threading.Thread.Sleep(2000);
                        break;
                }
            }
        }

        /// <summary>
        /// Displays all job vacancies with their details
        /// </summary>
        private void ViewAllJobVacancies()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║   ALL JOB VACANCIES                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");

            var vacancies = db.GetAllJobVacancies();

            if (vacancies.Count == 0)
            {
                Console.WriteLine("No job vacancies found in the system.\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            // Display table header
            Console.WriteLine($"{"ID",-5} {"Job Title",-30} {"Status",-15}");
            Console.WriteLine(new string('-', 50));

            foreach (var job in vacancies)
            {
                Console.WriteLine($"{job.JobID,-5} {job.JobTitle,-30} {job.Status,-15}");
            }

            Console.WriteLine($"\nTotal Vacancies: {vacancies.Count}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Creates a new job vacancy
        /// </summary>
        private void CreateNewJobVacancy()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║   CREATE NEW JOB VACANCY                       ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");

            // Get job title
            Console.Write("Enter Job Title (e.g., Software Developer): ");
            string jobTitle = Console.ReadLine()?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(jobTitle))
            {
                Console.WriteLine("Error: Job title cannot be empty.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            if (jobTitle.Length > 100)
            {
                Console.WriteLine("Error: Job title cannot exceed 100 characters.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            // Get job details
            Console.Write("\nEnter Job Details/Description:\n(Press Enter twice when done)\n");
            string jobDetail = GetMultilineInput();

            if (string.IsNullOrEmpty(jobDetail))
            {
                Console.WriteLine("Error: Job details cannot be empty.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            // Create the job vacancy object
            var newJob = new JobVacancy
            {
                JobTitle = jobTitle,
                JobDetail = jobDetail,
                Status = "Open"
            };

            // Insert into database
            if (db.CreateJobVacancy(newJob))
            {
                Console.WriteLine("\n✓ Job vacancy created successfully!");
                Console.WriteLine($"  Job Title: {jobTitle}");
                Console.WriteLine($"  Status: Open");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to create job vacancy. Please try again.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Edits an existing job vacancy
        /// </summary>
        private void EditJobVacancy()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║   EDIT JOB VACANCY                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");

            // Show available jobs
            var vacancies = db.GetAllJobVacancies();

            if (vacancies.Count == 0)
            {
                Console.WriteLine("No job vacancies available to edit.\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            // Display list
            Console.WriteLine("Available Job Vacancies:\n");
            for (int i = 0; i < vacancies.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {vacancies[i].JobTitle} (ID: {vacancies[i].JobID}) - Status: {vacancies[i].Status}");
            }

            Console.WriteLine($"\n{vacancies.Count + 1}. Back\n");
            Console.Write("Select a job to edit: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice) || choice < 1 || choice > vacancies.Count + 1)
            {
                Console.WriteLine("Invalid selection.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            if (choice == vacancies.Count + 1)
                return;

            var selectedJob = vacancies[choice - 1];

            // Show current details
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║   EDIT JOB DETAILS                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");

            Console.WriteLine($"Current Job Title: {selectedJob.JobTitle}");
            Console.WriteLine($"Current Status: {selectedJob.Status}\n");

            // Get new job title
            Console.Write("Enter new Job Title (press Enter to keep current): ");
            string newTitle = Console.ReadLine()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(newTitle) && newTitle.Length > 100)
            {
                Console.WriteLine("Error: Job title cannot exceed 100 characters.");
                System.Threading.Thread.Sleep(2000);
                return;
            }
            if (string.IsNullOrEmpty(newTitle))
                newTitle = selectedJob.JobTitle;

            // Get new job details
            Console.Write("\nEnter new Job Details (press Enter to keep current):\n");
            Console.Write("(Press Enter twice when done)\n");
            string newDetail = GetMultilineInput();
            if (string.IsNullOrEmpty(newDetail))
                newDetail = selectedJob.JobDetail;

            // Get new status
            Console.Write("\nSelect new Status:\n");
            Console.WriteLine("1. Open");
            Console.WriteLine("2. Closed");
            Console.Write("Choose status (press Enter to keep current): ");
            string statusChoice = Console.ReadLine()?.Trim() ?? string.Empty;

            string newStatus = selectedJob.Status;
            if (statusChoice == "1")
                newStatus = "Open";
            else if (statusChoice == "2")
                newStatus = "Closed";

            // Update the job
            var updatedJob = new JobVacancy
            {
                JobID = selectedJob.JobID,
                JobTitle = newTitle,
                JobDetail = newDetail,
                Status = newStatus
            };

            if (db.UpdateJobVacancy(updatedJob))
            {
                Console.WriteLine("\n✓ Job vacancy updated successfully!");
                Console.WriteLine($"  Job Title: {newTitle}");
                Console.WriteLine($"  Status: {newStatus}");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to update job vacancy. Please try again.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Closes a job vacancy (sets status to Closed)
        /// </summary>
        private void CloseJobVacancy()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║   CLOSE JOB VACANCY                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");

            var vacancies = db.GetAllJobVacancies();
            var openVacancies = new List<JobVacancy>();

            // Filter only open vacancies
            foreach (var job in vacancies)
            {
                if (job.Status == "Open")
                    openVacancies.Add(job);
            }

            if (openVacancies.Count == 0)
            {
                Console.WriteLine("No open job vacancies available to close.\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            // Display list of open jobs
            Console.WriteLine("Open Job Vacancies:\n");
            for (int i = 0; i < openVacancies.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {openVacancies[i].JobTitle} (ID: {openVacancies[i].JobID})");
            }

            Console.WriteLine($"\n{openVacancies.Count + 1}. Back\n");
            Console.Write("Select a job to close: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice) || choice < 1 || choice > openVacancies.Count + 1)
            {
                Console.WriteLine("Invalid selection.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            if (choice == openVacancies.Count + 1)
                return;

            var selectedJob = openVacancies[choice - 1];

            // Confirm closure
            Console.Write($"\nAre you sure you want to close '{selectedJob.JobTitle}'? (yes/no): ");
            string confirm = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;

            if (confirm != "yes" && confirm != "y")
            {
                Console.WriteLine("Operation cancelled.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            // Update status to Closed
            var closedJob = new JobVacancy
            {
                JobID = selectedJob.JobID,
                JobTitle = selectedJob.JobTitle,
                JobDetail = selectedJob.JobDetail,
                Status = "Closed"
            };

            if (db.UpdateJobVacancy(closedJob))
            {
                Console.WriteLine($"\n✓ Job vacancy '{selectedJob.JobTitle}' has been closed successfully!");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to close job vacancy. Please try again.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Deletes a job vacancy
        /// </summary>
        private void DeleteJobVacancy()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║   DELETE JOB VACANCY                           ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");

            var vacancies = db.GetAllJobVacancies();

            if (vacancies.Count == 0)
            {
                Console.WriteLine("No job vacancies available to delete.\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            // Display list
            Console.WriteLine("Job Vacancies:\n");
            for (int i = 0; i < vacancies.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {vacancies[i].JobTitle} (ID: {vacancies[i].JobID}) - Status: {vacancies[i].Status}");
            }

            Console.WriteLine($"\n{vacancies.Count + 1}. Back\n");
            Console.Write("Select a job to delete: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice) || choice < 1 || choice > vacancies.Count + 1)
            {
                Console.WriteLine("Invalid selection.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            if (choice == vacancies.Count + 1)
                return;

            var selectedJob = vacancies[choice - 1];

            // Double confirm deletion (important!)
            Console.WriteLine($"\n⚠️  WARNING: This will permanently delete '{selectedJob.JobTitle}' and all related data.");
            Console.Write("Are you absolutely sure? (yes/no): ");
            string confirm = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;

            if (confirm != "yes" && confirm != "y")
            {
                Console.WriteLine("Deletion cancelled.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            // Delete from database
            if (db.DeleteJobVacancy(selectedJob.JobID))
            {
                Console.WriteLine($"\n✓ Job vacancy '{selectedJob.JobTitle}' has been deleted successfully!");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to delete job vacancy. Please try again.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Helper method to get multiline input from user
        /// </summary>
        private string GetMultilineInput()
        {
            string input = string.Empty;
            string line;
            int emptyLineCount = 0;

            while (true)
            {
                line = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrEmpty(line))
                {
                    emptyLineCount++;
                    if (emptyLineCount >= 1) // Stop on first empty line
                        break;
                }
                else
                {
                    emptyLineCount = 0;
                    if (input.Length > 0)
                        input += "\n";
                    input += line;
                }
            }

            return input.Trim();
        }
    }
}
