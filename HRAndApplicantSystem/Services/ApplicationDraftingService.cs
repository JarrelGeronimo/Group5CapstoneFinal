using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Services
{
    /// <summary>
    /// Handles the application drafting workflow - allows applicants to prepare and review
    /// their application before submitting to HR for review
    /// </summary>
    public class ApplicationDraftingService
    {
        private readonly IApplicationRepository applicationRepository;
        private readonly DatabaseHelper db;

        public ApplicationDraftingService(IApplicationRepository appRepo = null)
        {
            applicationRepository = appRepo ?? new ApplicationRepository(new DatabaseHelper());
            db = new DatabaseHelper();
        }

        public class ApplicationDraft
        {
            public JobVacancy Job { get; set; }
            public Applicant Applicant { get; set; }
            public List<dynamic> UploadedDocuments { get; set; } = new List<dynamic>();
            public DateTime DraftCreatedAt { get; set; } = DateTime.Now;
        }

        /// <summary>
        /// Starts the application drafting workflow for a specific job
        /// Returns true if application was submitted, false if user cancelled
        /// </summary>
        public bool DraftAndSubmitApplication(JobVacancy job, Applicant applicant)
        {
            // First, create the draft application in the database
            int applicationID = db.CreateDraftApplication(applicant.ApplicantID, job.JobID);
            if (applicationID <= 0)
            {
                Console.WriteLine("Failed to create draft application.");
                System.Threading.Thread.Sleep(1500);
                return false;
            }

            var draft = new ApplicationDraft
            {
                Job = job,
                Applicant = applicant,
                DraftCreatedAt = DateTime.Now
            };

            bool draftComplete = false;
            while (!draftComplete)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     DRAFT APPLICATION                        ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                Console.WriteLine("=== JOB DETAILS ===");
                Console.WriteLine($"Position: {draft.Job.JobTitle}");
                Console.WriteLine($"Status: {draft.Job.Status}");
                Console.WriteLine($"Description:\n{draft.Job.JobDetail}\n");

                Console.WriteLine("=== APPLICATION DETAILS ===");
                Console.WriteLine($"Applicant: {draft.Applicant.FirstName} {draft.Applicant.LastName}");
                Console.WriteLine($"Contact: {draft.Applicant.ContactNo}");
                Console.WriteLine($"Address: {draft.Applicant.Address}");
                Console.WriteLine($"Education: {draft.Applicant.Education}");
                Console.WriteLine($"Skills: {draft.Applicant.Skills}\n");

                // Show document submission status
                Console.WriteLine("=== REQUIRED DOCUMENTS ===");
                var documents = db.GetApplicantDocuments(draft.Applicant.ApplicantID, draft.Job.JobID);
                
                if (documents.Count == 0)
                {
                    Console.WriteLine("No documents submitted yet.");
                    Console.WriteLine("Documents are optional, but recommended.\n");
                }
                else
                {
                    foreach (var doc in documents)
                    {
                        string status = doc.DocumentStatus;
                        string icon = status == ApplicationStatus.Submitted ? "✓" : "○";
                        Console.WriteLine($"  {icon} {doc.RequirementName}: {status}");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("=== DRAFT OPTIONS ===");
                Console.WriteLine("1. Manage Documents (Upload/Update)");
                Console.WriteLine("2. Review & Confirm");
                Console.WriteLine("3. Save Draft & Exit (You can resume this later)\n");

                Console.Write("Choose an option: ");
                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ManageApplicationDocuments(draft);
                        break;
                    case "2":
                        if (ConfirmAndSubmit(draft, applicationID))
                        {
                            return true;
                        }
                        draftComplete = true;
                        break;
                    case "3":
                        Console.WriteLine("\n✓ Draft saved successfully!");
                        Console.WriteLine("You can resume this draft anytime from 'View My Applications'.");
                        System.Threading.Thread.Sleep(2000);
                        return false;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        System.Threading.Thread.Sleep(1500);
                        break;
                }
            }

            return false;
        }

        private void ManageApplicationDocuments(ApplicationDraft draft)
        {
            DocumentSubmissionService docService = new DocumentSubmissionService();
            docService.ManageDocumentSubmissions(draft.Applicant.ApplicantID, draft.Job.JobID);
        }

        private bool ConfirmAndSubmit(ApplicationDraft draft, int applicationID)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     CONFIRM SUBMISSION                       ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.WriteLine("=== SUBMISSION SUMMARY ===");
            Console.WriteLine($"Position: {draft.Job.JobTitle}");
            Console.WriteLine($"Applicant: {draft.Applicant.FirstName} {draft.Applicant.LastName}");

            var requirements = db.GetJobSpecificRequirements(draft.Job.JobID);
            Console.WriteLine($"\nRequired Documents: {requirements.Count}");
            
            if (requirements.Count > 0)
            {
                var documents = db.GetApplicantDocuments(draft.Applicant.ApplicantID, draft.Job.JobID);
                foreach (var req in requirements)
                {
                    var doc = documents.FirstOrDefault(d => d.RequirementTypeID == req.RequirementTypeID);
                    string status = doc != null ? doc.DocumentStatus : "Not Submitted";
                    string icon = status == ApplicationStatus.Submitted ? "✓" : "✗";
                    Console.WriteLine($"  {icon} {req.RequirementName}: {status}");
                }
            }

            // Check if all requirements are met
            if (!db.CheckAllJobRequirementsSubmitted(draft.Applicant.ApplicantID, draft.Job.JobID))
            {
                Console.WriteLine("\n" + new string('=', 45));
                Console.WriteLine("\n✗ ERROR: All required documents must be submitted before submission.");
                Console.WriteLine("Please upload all required documents and mark them as 'Submitted'.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return false;
            }

            Console.WriteLine("\n" + new string('=', 45));
            Console.WriteLine("\n✓ All required documents have been submitted.");
            Console.WriteLine("Once you submit, your application will be sent to HR for review.");
            Console.WriteLine("You can update your documents anytime from your dashboard.\n");

            Console.Write("Do you want to submit this application? (yes/no): ");
            string confirm = (Console.ReadLine()?.Trim() ?? string.Empty).ToLower();

            if (confirm == "yes" || confirm == "y")
            {
                // Update draft to Submitted status
                if (db.UpdateApplicationStatus(applicationID, ApplicationStatus.Submitted, "Draft submitted by applicant", "Applicant"))
                {
                    // Log audit trail for applicant submission
                    db.LogAuditTrail("Applicant", draft.Applicant.Username, $"Submitted Application #{applicationID} for Position: {draft.Job.JobTitle}");
                    
                    Console.WriteLine($"\n✓ Successfully submitted application for {draft.Job.JobTitle}!");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return true;
                }
                else
                {
                    Console.WriteLine($"\n✗ Failed to submit application.\n");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Resumes an existing draft application, allowing applicant to continue editing
        /// </summary>
        public void ResumeDraftApplication(JobVacancy job, Applicant applicant, int applicationID)
        {
            var draft = new ApplicationDraft
            {
                Job = job,
                Applicant = applicant,
                DraftCreatedAt = DateTime.Now
            };

            bool draftComplete = false;
            while (!draftComplete)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     RESUME DRAFT APPLICATION                 ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                Console.WriteLine("=== JOB DETAILS ===");
                Console.WriteLine($"Position: {draft.Job.JobTitle}");
                Console.WriteLine($"Status: {draft.Job.Status}");
                Console.WriteLine($"Description:\n{draft.Job.JobDetail}\n");

                Console.WriteLine("=== APPLICATION DETAILS ===");
                Console.WriteLine($"Applicant: {draft.Applicant.FirstName} {draft.Applicant.LastName}");
                Console.WriteLine($"Contact: {draft.Applicant.ContactNo}");
                Console.WriteLine($"Address: {draft.Applicant.Address}");
                Console.WriteLine($"Education: {draft.Applicant.Education}");
                Console.WriteLine($"Skills: {draft.Applicant.Skills}\n");

                // Show document submission status
                Console.WriteLine("=== REQUIRED DOCUMENTS ===");
                var documents = db.GetApplicantDocuments(draft.Applicant.ApplicantID, draft.Job.JobID);
                
                if (documents.Count == 0)
                {
                    Console.WriteLine("No documents submitted yet.");
                    Console.WriteLine("Documents are optional, but recommended.\n");
                }
                else
                {
                    foreach (var doc in documents)
                    {
                        string status = doc.DocumentStatus;
                        string icon = status == ApplicationStatus.Submitted ? "✓" : "○";
                        Console.WriteLine($"  {icon} {doc.RequirementName}: {status}");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("=== DRAFT OPTIONS ===");
                Console.WriteLine("1. Manage Documents (Upload/Update)");
                Console.WriteLine("2. Review & Confirm");
                Console.WriteLine("3. Save Draft & Exit\n");

                Console.Write("Choose an option: ");
                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ManageApplicationDocuments(draft);
                        break;
                    case "2":
                        if (ConfirmAndSubmit(draft, applicationID))
                        {
                            return;
                        }
                        draftComplete = true;
                        break;
                    case "3":
                        Console.WriteLine("\n✓ Draft saved successfully!");
                        Console.WriteLine("You can resume this draft anytime from 'View My Applications'.");
                        System.Threading.Thread.Sleep(2000);
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        System.Threading.Thread.Sleep(1500);
                        break;
                }
            }
        }
    }
}
