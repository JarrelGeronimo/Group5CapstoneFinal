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
                Console.WriteLine("4. View Job Requirements");
                Console.WriteLine("5. View Applicant Counts");
                Console.WriteLine("6. Delete Job Vacancy");
                Console.WriteLine("7. Back to Dashboard\n");

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
                        EditJobRequirements();
                        break;
                    case "5": 
                        ViewApplicantCounts();    
                        break;  
                    case "6": 
                        DeleteJobVacancy();       
                        break;
                    case "7": 
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

            // Show edit options for the selected job
            bool editing = true;
            while (editing)
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════════════╗");
                Console.WriteLine($"║   EDIT: {selectedJob.JobTitle,-36} ║");
                Console.WriteLine("╚════════════════════════════════════════════════╝\n");

                Console.WriteLine("What would you like to edit?\n");
                Console.WriteLine("1. Job Title");
                Console.WriteLine("2. Job Details");
                Console.WriteLine("3. Status");
                Console.WriteLine("4. Back\n");

                Console.Write("Choose an option: ");
                string fieldChoice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (fieldChoice)
                {
                    case "1":
                        EditJobTitle(selectedJob);
                        break;
                    case "2":
                        EditJobDetails(selectedJob);
                        break;
                    case "3":
                        EditJobStatus(selectedJob);
                        break;
                    case "4":
                        editing = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        System.Threading.Thread.Sleep(1500);
                        break;
                }
            }
        }

        private void EditJobTitle(JobVacancy job)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║   EDIT JOB TITLE                               ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");

            Console.WriteLine($"Current Title: {job.JobTitle}\n");
            Console.Write("Enter new Job Title: ");
            string newTitle = Console.ReadLine()?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(newTitle))
            {
                Console.WriteLine("Job title cannot be empty.");
                System.Threading.Thread.Sleep(1500);
                return;
            }

            if (newTitle.Length > 100)
            {
                Console.WriteLine("Job title cannot exceed 100 characters.");
                System.Threading.Thread.Sleep(1500);
                return;
            }

            job.JobTitle = newTitle;
            if (db.UpdateJobVacancy(job))
            {
                Console.WriteLine("\n✓ Job title updated successfully!");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to update job title.");
            }
            System.Threading.Thread.Sleep(1500);
        }

        private void EditJobDetails(JobVacancy job)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║   EDIT JOB DETAILS                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");

            Console.WriteLine("Current Details:\n" + job.JobDetail + "\n");
            Console.Write("Enter new Job Details:\n(Press Enter twice when done)\n");
            string newDetails = GetMultilineInput();

            if (string.IsNullOrEmpty(newDetails))
            {
                Console.WriteLine("Job details cannot be empty.");
                System.Threading.Thread.Sleep(1500);
                return;
            }

            job.JobDetail = newDetails;
            if (db.UpdateJobVacancy(job))
            {
                Console.WriteLine("\n✓ Job details updated successfully!");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to update job details.");
            }
            System.Threading.Thread.Sleep(1500);
        }

        private void EditJobStatus(JobVacancy job)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║   EDIT JOB STATUS                              ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");

            Console.WriteLine($"Current Status: {job.Status}\n");
            Console.WriteLine("1. Open");
            Console.WriteLine("2. Closed\n");
            Console.Write("Choose new status: ");
            string statusChoice = Console.ReadLine()?.Trim() ?? string.Empty;

            string newStatus = null;
            if (statusChoice == "1")
                newStatus = "Open";
            else if (statusChoice == "2")
                newStatus = "Closed";
            else
            {
                Console.WriteLine("Invalid choice.");
                System.Threading.Thread.Sleep(1500);
                return;
            }

            job.Status = newStatus;
            if (db.UpdateJobVacancy(job))
            {
                Console.WriteLine($"\n✓ Job status updated to '{newStatus}' successfully!");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to update job status.");
            }
            System.Threading.Thread.Sleep(1500);
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
        /// Edits the requirements for a specific job
        /// </summary>
        private void EditJobRequirements()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║   JOB REQUIREMENTS                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");

            // Show available jobs
            var vacancies = db.GetAllJobVacancies();

            if (vacancies.Count == 0)
            {
                Console.WriteLine("No job vacancies available.\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            // Display list of jobs
            Console.WriteLine("Available Job Vacancies:\n");
            for (int i = 0; i < vacancies.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {vacancies[i].JobTitle} (ID: {vacancies[i].JobID})");
            }

            Console.WriteLine($"\n{vacancies.Count + 1}. Back\n");
            Console.Write("Select a job to view requirements: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice) || choice < 1 || choice > vacancies.Count + 1)
            {
                Console.WriteLine("Invalid selection.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            if (choice == vacancies.Count + 1)
                return;

            var selectedJob = vacancies[choice - 1];

            // Show requirements for the selected job
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine($"║   REQUIREMENTS FOR: {selectedJob.JobTitle,-24} ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");

            var requirements = db.GetJobSpecificRequirements(selectedJob.JobID);

            if (requirements.Count == 0)
            {
                Console.WriteLine("No requirements found for this job.\n");
            }
            else
            {
                Console.WriteLine("Required Documents:\n");
                for (int i = 0; i < requirements.Count; i++)
                {
                    var req = requirements[i];
                    Console.WriteLine($"{i + 1}. {req.RequirementName}");
                }
                Console.WriteLine($"\nTotal: {requirements.Count} requirement(s)");
            }

            Console.WriteLine("\nNote: All jobs share the same requirement types from the system.\n");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Helper method to get multiline input from user
        /// </summary>
        
        // ════════════════════════════════════════════════════════════════
        //  FEATURE — VIEW APPLICANT COUNTS PER JOB
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Shows each job vacancy alongside a breakdown of how many applicants
        /// are at each stage of the pipeline.
        /// </summary>
        private void ViewApplicantCounts()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║   APPLICANT COUNTS BY JOB                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");

            var vacancies = db.GetAllJobVacancies();

            if (vacancies.Count == 0)
            {
                Console.WriteLine("No job vacancies found.\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            // Column header
            Console.WriteLine(
                $"{"Job Title",-28} {"Status",-10} {"Total",6} " +
                $"{"Submit",7} {"Review",7} {"Short",6} {"Intrvw",7} {"Accept",7} {"Reject",7}");
            Console.WriteLine(new string('─', 87));

            int grandTotal = 0;

            foreach (var job in vacancies)
            {
                var applications = db.GetApplicationsByJob(job.JobID);

                // Count per pipeline stage (exclude Drafts)
                int total      = 0;
                int submitted  = 0, underReview = 0, shortlisted = 0;
                int interview  = 0, accepted    = 0, rejected    = 0;

                foreach (var app in applications)
                {
                    string s = (string)app.Status;
                    if (s == ApplicationStatus.Draft) continue;     // skip drafts

                    total++;
                    switch (s)
                    {
                        case ApplicationStatus.Submitted:           submitted++;  break;
                        case ApplicationStatus.UnderReview:        underReview++;break;
                        case ApplicationStatus.Shortlisted:         shortlisted++;break;
                        case ApplicationStatus.InterviewScheduled: interview++;  break;
                        case ApplicationStatus.Accepted:            accepted++;   break;
                        case ApplicationStatus.Rejected:            rejected++;   break;
                    }
                }

                grandTotal += total;

                string titleDisplay  = job.JobTitle.Length > 27 ? job.JobTitle.Substring(0, 27) + "…" : job.JobTitle;
                string statusDisplay = job.Status.Length   > 9  ? job.Status.Substring(0, 9)   + "…" : job.Status;

                // Colour-code Archived rows so they stand out
                if (job.Status == "Archived")
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.WriteLine(
                    $"{titleDisplay,-28} {statusDisplay,-10} {total,6} " +
                    $"{submitted,7} {underReview,7} {shortlisted,6} {interview,7} {accepted,7} {rejected,7}");

                Console.ResetColor();
            }

            // Grand total footer
            Console.WriteLine(new string('─', 87));
            Console.WriteLine($"{"TOTAL APPLICATIONS",-40} {grandTotal,6}");
            Console.WriteLine();
            Console.WriteLine("Columns: Total | Submitted | Under Review | Shortlisted | Interview | Accepted | Rejected");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

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
