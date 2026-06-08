using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Utilities;

namespace HRAndApplicantSystem.Services
{
    public class ApplicationStatusTransitionService
    {
        private DatabaseHelper db = new DatabaseHelper();

        public void ShowStatusTransitionMenu(string hrUsername)
        {
            bool managing = true;

            while (managing)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     APPLICATION STATUS TRANSITIONS           ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                Console.WriteLine("1. Shortlist Applicant");
                Console.WriteLine("2. Put On Hold");
                Console.WriteLine("3. Reject Application");
                Console.WriteLine("4. Back to Dashboard");
                Console.Write("\nChoose an option: ");

                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ShortlistApplicant(hrUsername);
                        break;
                    case "2":
                        PutOnHold(hrUsername);
                        break;
                    case "3":
                        RejectApplication(hrUsername);
                        break;
                    case "4":
                        managing = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ShortlistApplicant(string hrUsername)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     SHORTLIST APPLICANT                      ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var pendingApps = db.GetApplicationsByStatus("Submitted");
            if (pendingApps.Count == 0)
            {
                pendingApps = db.GetApplicationsByStatus("Under Review");
            }

            if (pendingApps.Count == 0)
            {
                Console.WriteLine("No applications available for shortlisting.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"#",-3} {"Name",-22} {"Job Title",-25} {"Status",-15}");
            Console.WriteLine(new string('-', 65));

            for (int i = 0; i < pendingApps.Count; i++)
            {
                var app = pendingApps[i];
                string name = $"{app.FirstName} {app.LastName}";
                string jobTitle = ((string)app.JobTitle).Length > 24
                    ? ((string)app.JobTitle).Substring(0, 24)
                    : (string)app.JobTitle;
                Console.WriteLine($"{i + 1,-3} {name,-22} {jobTitle,-25} {app.Status,-15}");
            }

            Console.WriteLine($"\n{pendingApps.Count + 1}. Back");
            Console.Write("\nSelect applicant: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice)
                || choice < 1 || choice > pendingApps.Count)
            {
                return;
            }

            var selected = pendingApps[choice - 1];
            Console.Clear();

            string remarks = InputValidator.GetValidatedInput("Enter remarks (optional): ", "Remarks", true);

            bool success = db.UpdateApplicationStatus(selected.ApplicationID, "Shortlisted", remarks, hrUsername);

            if (success)
            {
                Console.WriteLine($"\n✅ Applicant shortlisted successfully!");
                string userRole = db.GetRoleNameByUsername(hrUsername);
                db.LogAuditTrail(userRole, hrUsername, $"Shortlisted Applicant from {selected.JobTitle}");
            }
            else
            {
                Console.WriteLine("\n❌ Failed to shortlist applicant.");
            }

            Console.ReadKey();
        }

        private void PutOnHold(string hrUsername)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     PUT ON HOLD                              ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var pendingApps = db.GetAllApplications();

            if (pendingApps.Count == 0)
            {
                Console.WriteLine("No applications available.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"#",-3} {"Name",-22} {"Job Title",-25} {"Status",-15}");
            Console.WriteLine(new string('-', 65));

            for (int i = 0; i < pendingApps.Count; i++)
            {
                var app = pendingApps[i];
                string name = $"{app.FirstName} {app.LastName}";
                string jobTitle = ((string)app.JobTitle).Length > 24
                    ? ((string)app.JobTitle).Substring(0, 24)
                    : (string)app.JobTitle;
                Console.WriteLine($"{i + 1,-3} {name,-22} {jobTitle,-25} {app.Status,-15}");
            }

            Console.WriteLine($"\n{pendingApps.Count + 1}. Back");
            Console.Write("\nSelect applicant: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice)
                || choice < 1 || choice > pendingApps.Count)
            {
                return;
            }

            var selected = pendingApps[choice - 1];
            Console.Clear();

            string remarks = InputValidator.GetValidatedInput("Enter reason for hold (optional): ", "Reason", true);

            bool success = db.UpdateApplicationStatus(selected.ApplicationID, "On Hold", remarks, hrUsername);

            if (success)
            {
                Console.WriteLine($"\n✅ Application put on hold successfully!");
                string userRole = db.GetRoleNameByUsername(hrUsername);
                db.LogAuditTrail(userRole, hrUsername, $"Put on hold: {selected.JobTitle}");
            }
            else
            {
                Console.WriteLine("\n❌ Failed to put application on hold.");
            }

            Console.ReadKey();
        }

        private void RejectApplication(string hrUsername)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     REJECT APPLICATION                       ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var pendingApps = db.GetAllApplications()
                .Where(app => app.Status != "Accepted" && app.Status != "Rejected")
                .ToList();

            if (pendingApps.Count == 0)
            {
                Console.WriteLine("No applications available.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"#",-3} {"Name",-22} {"Job Title",-25} {"Status",-15}");
            Console.WriteLine(new string('-', 65));

            for (int i = 0; i < pendingApps.Count; i++)
            {
                var app = pendingApps[i];
                string name = $"{app.FirstName} {app.LastName}";
                string jobTitle = ((string)app.JobTitle).Length > 24
                    ? ((string)app.JobTitle).Substring(0, 24)
                    : (string)app.JobTitle;
                Console.WriteLine($"{i + 1,-3} {name,-22} {jobTitle,-25} {app.Status,-15}");
            }

            Console.WriteLine($"\n{pendingApps.Count + 1}. Back");
            Console.Write("\nSelect applicant: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice)
                || choice < 1 || choice > pendingApps.Count)
            {
                return;
            }

            var selected = pendingApps[choice - 1];
            Console.Clear();

            string reason = InputValidator.GetValidatedInput("Enter rejection reason: ", "Reason");

            bool success = db.UpdateApplicationStatus(selected.ApplicationID, "Rejected", reason, hrUsername);

            if (success)
            {
                Console.WriteLine($"\n✅ Application rejected successfully!");
                string userRole = db.GetRoleNameByUsername(hrUsername);
                db.LogAuditTrail(userRole, hrUsername, $"Rejected Application: {selected.JobTitle}");
            }
            else
            {
                Console.WriteLine("\n❌ Failed to reject application.");
            }

            Console.ReadKey();
        }
    }
}
