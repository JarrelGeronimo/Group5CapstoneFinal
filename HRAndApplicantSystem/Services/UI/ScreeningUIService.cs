using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Services.Business;

namespace HRAndApplicantSystem.Services.UI
{
    /// <summary>
    /// UI rendering for screening operations.
    /// Handles all console output and user interaction.
    /// Uses ScreeningBusinessService for business logic.
    /// </summary>
    public class ScreeningUIService
    {
        private readonly ScreeningBusinessService businessService;

        public ScreeningUIService(ScreeningBusinessService businessService = null)
        {
            this.businessService = businessService ?? new ScreeningBusinessService();
        }

        /// <summary>
        /// Display screening interface for HR staff
        /// </summary>
        public void ScreenApplication(int applicationId, string hrUsername)
        {
            var appDetails = businessService.GetApplicationDetailsForScreening(applicationId);

            if (appDetails == null)
            {
                Console.WriteLine("Application not found.");
                return;
            }

            // Update status to under review
            businessService.UpdateToUnderReview(applicationId, hrUsername);

            RenderScreeningHeader();
            RenderApplicantInformation(appDetails);
            RenderPositionInformation(appDetails);
            RenderSubmittedDocuments(appDetails);
            RenderScreeningDecision(applicationId, appDetails, hrUsername);
        }

        private void RenderScreeningHeader()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     APPLICATION SCREENING                    ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");
        }

        private void RenderApplicantInformation(dynamic appDetails)
        {
            Console.WriteLine("=== APPLICANT INFORMATION ===\n");
            Console.WriteLine($"Name: {appDetails.FirstName} {appDetails.LastName}");
            Console.WriteLine($"Contact: {appDetails.ContactNo}");
            Console.WriteLine($"Address: {appDetails.Address}");
            Console.WriteLine($"Education: {appDetails.Education}");
            Console.WriteLine($"Skills: {appDetails.Skills}");
            Console.WriteLine($"Applied Date: {appDetails.DateApplied:MMMM dd, yyyy HH:mm}\n");
        }

        private void RenderPositionInformation(dynamic appDetails)
        {
            Console.WriteLine("=== POSITION INFORMATION ===\n");
            Console.WriteLine($"Job Title: {appDetails.JobTitle}");
            Console.WriteLine($"Description:\n{appDetails.JobDetail}\n");
        }

        private void RenderSubmittedDocuments(dynamic appDetails)
        {
            Console.WriteLine("=== SUBMITTED DOCUMENTS ===\n");
            var documents = businessService.GetApplicantDocuments(appDetails.ApplicantID, appDetails.JobID);

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
        }

        private void RenderScreeningDecision(int applicationId, dynamic appDetails, string hrUsername)
        {
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

            if (!businessService.IsValidScreeningDecision(choice))
            {
                Console.WriteLine("\n✗ Invalid choice.");
                System.Threading.Thread.Sleep(1500);
                return;
            }

            var (result, newStatus) = businessService.GetDecisionFromChoice(choice);

            Console.Write("\nAdd remarks/feedback: ");
            string remarks = Console.ReadLine()?.Trim() ?? "";

            // Update the screening result
            if (businessService.ScreenApplication(applicationId, result, remarks, hrUsername))
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
