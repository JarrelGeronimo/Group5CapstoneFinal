using HRAndApplicantSystem.Database;

namespace HRAndApplicantSystem.Services
{
    public class ScreeningService
    {
        private readonly DatabaseHelper db;

        public ScreeningService()
        {
            db = new DatabaseHelper();
        }

        public void ScreenApplication(int applicationID, string hrUsername)
        {
            var appDetails = db.GetApplicationDetailsForScreening(applicationID);

            if (appDetails == null)
            {
                Console.WriteLine("Application not found.");
                return;
            }

            // First, update status to "Under Review"
            db.UpdateApplicationStatus(applicationID, "Under Review", "HR screening started", hrUsername);

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     APPLICATION SCREENING                    ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            // Display applicant information
            Console.WriteLine("=== APPLICANT INFORMATION ===\n");
            Console.WriteLine($"Name: {appDetails.FirstName} {appDetails.LastName}");
            Console.WriteLine($"Contact: {appDetails.ContactNo}");
            Console.WriteLine($"Address: {appDetails.Address}");
            Console.WriteLine($"Education: {appDetails.Education}");
            Console.WriteLine($"Skills: {appDetails.Skills}");
            Console.WriteLine($"Applied Date: {appDetails.DateApplied:MMMM dd, yyyy HH:mm}\n");

            // Display job information
            Console.WriteLine("=== POSITION INFORMATION ===\n");
            Console.WriteLine($"Job Title: {appDetails.JobTitle}");
            Console.WriteLine($"Description:\n{appDetails.JobDetail}\n");

            // Display submitted documents
            Console.WriteLine("=== SUBMITTED DOCUMENTS ===\n");
            var documents = db.GetApplicantDocuments(appDetails.ApplicantID, appDetails.JobID);

            if (documents.Count == 0)
            {
                Console.WriteLine("No documents submitted.");
            }
            else
            {
                foreach (var doc in documents)
                {
                    Console.WriteLine($"• {doc.RequirementName}: {doc.DocumentStatus}");
                    if (!string.IsNullOrEmpty(doc.Remarks))
                    {
                        Console.WriteLine($"  Remarks: {doc.Remarks}");
                    }
                }
            }

            Console.WriteLine("\n" + new string('=', 45));

            // Screening decision
            Console.WriteLine("\n=== SCREENING DECISION ===\n");
            Console.WriteLine("1. Shortlist (Qualified)");
            Console.WriteLine("2. Reject (Not Qualified)");
            Console.WriteLine("3. Return to Dashboard (Keep Under Review)");
            Console.Write("\nSelect decision: ");

            string choice = Console.ReadLine()?.Trim() ?? string.Empty;

            if (choice == "3")
            {
                Console.WriteLine("\n✓ Application kept under review.");
                Console.WriteLine("Press any key to return to dashboard...");
                Console.ReadKey();
                return;
            }

            string result = choice == "1" ? "Qualified" : "Not Qualified";
            string newStatus = choice == "1" ? "Shortlisted" : "Rejected";

            Console.Write("\nAdd remarks/feedback: ");
            string remarks = Console.ReadLine()?.Trim() ?? "";

            // Update the screening result
            if (db.ScreenApplication(applicationID, result, remarks, hrUsername))
            {
                Console.WriteLine($"\n✓ Application marked as {newStatus}");
                Console.WriteLine("Status history has been recorded.");
                Console.WriteLine("\nApplicant will be notified of the status update.");
            }
            else
            {
                Console.WriteLine($"\n✗ Failed to complete screening.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
