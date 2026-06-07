using HRAndApplicantSystem.Database;

namespace HRAndApplicantSystem.Services
{
    public class HiringDecisionService
    {
        private readonly DatabaseHelper db;

        public HiringDecisionService()
        {
            db = new DatabaseHelper();
        }

        public void ShowHiringDecisionMenu(string managerUsername)
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     HIRING DECISION                          ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                Console.WriteLine("1. Review Applicants for Final Decision");
                Console.WriteLine("2. View Accepted Applicants");
                Console.WriteLine("3. View Rejected Applicants");
                Console.WriteLine("4. Back to Dashboard");
                Console.Write("\nChoose an option: ");

                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ReviewForFinalDecision(managerUsername);
                        break;
                    case "2":
                        ViewDecidedApplicants("Accepted");
                        break;
                    case "3":
                        ViewDecidedApplicants("Rejected");
                        break;
                    case "4":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        System.Threading.Thread.Sleep(1500);
                        break;
                }
            }
        }

        private void ReviewForFinalDecision(string managerUsername)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     APPLICANTS FOR FINAL REVIEW              ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var forFinalReview = db.GetApplicationsByStatus("For Final Review");

            if (forFinalReview.Count == 0)
            {
                Console.WriteLine("No applicants pending final decision.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"#",-3} {"Name",-22} {"Job Title",-25} {"Applied",-12}");
            Console.WriteLine(new string('-', 65));

            for (int i = 0; i < forFinalReview.Count; i++)
            {
                var app = forFinalReview[i];
                string name = $"{app.FirstName} {app.LastName}";
                string jobTitle = ((string)app.JobTitle).Length > 24
                    ? ((string)app.JobTitle).Substring(0, 24)
                    : (string)app.JobTitle;
                string date = ((DateTime)app.DateApplied).ToString("yyyy-MM-dd");
                Console.WriteLine($"{i + 1,-3} {name,-22} {jobTitle,-25} {date,-12}");
            }

            Console.WriteLine($"\n{forFinalReview.Count + 1}. Back");
            Console.Write("\nSelect applicant to decide: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice)
                || choice < 1 || choice > forFinalReview.Count)
            {
                Console.WriteLine("Returning...");
                System.Threading.Thread.Sleep(1000);
                return;
            }

            var selected = forFinalReview[choice - 1];

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     FINAL HIRING DECISION                    ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.WriteLine($"Applicant : {selected.FirstName} {selected.LastName}");
            Console.WriteLine($"Position  : {selected.JobTitle}");
            Console.WriteLine($"Applied   : {((DateTime)selected.DateApplied):MMMM dd, yyyy}\n");

            Console.WriteLine("Decision:");
            Console.WriteLine("1. Accept");
            Console.WriteLine("2. Reject");
            Console.WriteLine("3. On Hold");
            Console.WriteLine("4. Back");
            Console.Write("\nSelect decision: ");

            string decisionChoice = Console.ReadLine()?.Trim() ?? "4";

            if (decisionChoice == "4") return;

            string finalDecision = decisionChoice switch
            {
                "1" => "Accepted",
                "2" => "Rejected",
                "3" => "On Hold",
                _   => null
            };

            if (finalDecision == null)
            {
                Console.WriteLine("Invalid choice. Returning...");
                System.Threading.Thread.Sleep(1500);
                return;
            }

            Console.Write("Final Remarks: ");
            string remarks = Console.ReadLine()?.Trim() ?? "";

            bool success = db.MakeHiringDecision(
                selected.ApplicationID,
                finalDecision,
                remarks,
                managerUsername
            );

            if (success)
            {
                Console.WriteLine($"\n✓ Decision recorded: {finalDecision}");
                Console.WriteLine($"  Applicant: {selected.FirstName} {selected.LastName}");
                Console.WriteLine($"  Status updated to: {finalDecision}");
                Console.WriteLine($"  Decision by: {managerUsername}");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to record decision. Please try again.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ViewDecidedApplicants(string status)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine($"║     {status.ToUpper(),-41}║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var apps = db.GetApplicationsByStatus(status);

            if (apps.Count == 0)
            {
                Console.WriteLine($"No {status.ToLower()} applicants found.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"Name",-22} {"Job Title",-25} {"Applied",-12}");
            Console.WriteLine(new string('-', 62));

            foreach (var app in apps)
            {
                string name = $"{app.FirstName} {app.LastName}";
                string jobTitle = ((string)app.JobTitle).Length > 24
                    ? ((string)app.JobTitle).Substring(0, 24)
                    : (string)app.JobTitle;
                string date = ((DateTime)app.DateApplied).ToString("yyyy-MM-dd");
                Console.WriteLine($"{name,-22} {jobTitle,-25} {date,-12}");
            }

            Console.WriteLine($"\nTotal: {apps.Count}");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }
    }
}
