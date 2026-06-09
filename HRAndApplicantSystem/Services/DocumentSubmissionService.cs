using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Services
{
    public class DocumentSubmissionService
    {
        private readonly DatabaseHelper db;

        public DocumentSubmissionService()
        {
            db = new DatabaseHelper();
        }

        public void ManageDocumentSubmissions(int applicantID, int jobID)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     SUBMIT REQUIRED DOCUMENTS                ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            // Get job requirements (all RequirementTypes)
            var requirements = db.GetJobRequirements(jobID);

            if (requirements.Count == 0)
            {
                Console.WriteLine("This job position has no document requirements.\n");
                Console.WriteLine("You can proceed with your application.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"This position requires the following documents:\n");
            Console.WriteLine($"{"#",-3} {"Document Type",-35} {"Status",-15}");
            Console.WriteLine(new string('-', 55));

            // Display requirements and check submission status
            var existingDocs = db.GetApplicantDocuments(applicantID, jobID);
            for (int i = 0; i < requirements.Count; i++)
            {
                var req = requirements[i];
                var existing = existingDocs.FirstOrDefault(d => d.RequirementTypeID == req.RequirementTypeID);
                string status = existing != null ? (string)existing.DocumentStatus : "Not Submitted";
                Console.WriteLine($"{i + 1,-3} {req.RequirementName,-35} {status,-15}");
            }

            Console.WriteLine("\n=== Instructions ===\n");
            Console.WriteLine("You can draft and submit documents before HR starts reviewing your application.");
            Console.WriteLine("Documents can be updated while your application is in 'Submitted' status.\n");

            bool managing = true;
            while (managing)
            {
                Console.WriteLine("\n1. Submit/Update Document");
                Console.WriteLine("2. View My Submissions");
                Console.WriteLine("3. Continue");
                Console.Write("\nChoose an option: ");

                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        SubmitDocument(applicantID, jobID, requirements);
                        break;
                    case "2":
                        ViewDocumentSubmissions(applicantID, jobID);
                        break;
                    case "3":
                        managing = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        System.Threading.Thread.Sleep(1500);
                        break;
                }
            }
        }

        private void SubmitDocument(int applicantID, int jobID, List<dynamic> requirements)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     SUBMIT DOCUMENT                          ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            // Show available requirements
            Console.WriteLine("Select a document type to submit:\n");
            for (int i = 0; i < requirements.Count; i++)
            {
                var req = requirements[i];
                Console.WriteLine($"{i + 1}. {req.RequirementName}");
            }
            Console.WriteLine($"{requirements.Count + 1}. Back");

            Console.Write("\nChoose option: ");

            if (int.TryParse(Console.ReadLine()?.Trim(), out int choice) && choice >= 1 && choice <= requirements.Count)
            {
                var selectedReq = requirements[choice - 1];
                
                Console.Write("\nEnter document description/remarks (or press Enter to skip): ");
                string remarks = Console.ReadLine()?.Trim() ?? "";

                // Save as Submitted (applicant can provide remarks/description)
                if (db.SubmitApplicantDocument(applicantID, jobID, selectedReq.RequirementTypeID, remarks, ApplicationStatus.Submitted))
                {
                    Console.WriteLine($"\n✓ Document submitted: {selectedReq.RequirementName}");
                    Console.WriteLine("You can update this document later if needed.");
                }
                else
                {
                    Console.WriteLine($"\n✗ Failed to save document.");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        private void ViewDocumentSubmissions(int applicantID, int jobID)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     MY DOCUMENT SUBMISSIONS                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var documents = db.GetApplicantDocuments(applicantID, jobID);

            if (documents.Count == 0)
            {
                Console.WriteLine("You haven't submitted any documents yet.\n");
            }
            else
            {
                Console.WriteLine($"{"Document Type",-35} {"Status",-15} {"Remarks",-30}");
                Console.WriteLine(new string('-', 80));

                foreach (var doc in documents)
                {
                    string remarks = ((string)doc.Remarks).Length > 29 ? ((string)doc.Remarks).Substring(0, 29) : (string)doc.Remarks;
                    Console.WriteLine($"{(string)doc.RequirementName,-35} {(string)doc.DocumentStatus,-15} {remarks,-30}");
                }
            }

            Console.WriteLine("\n\nPress any key to return...");
            Console.ReadKey();
        }
    }
}
