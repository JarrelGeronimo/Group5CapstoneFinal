using HRAndApplicantSystem.Database;

namespace HRAndApplicantSystem.Services
{
    public class HRDashboardService
    {
        private readonly DatabaseHelper db;
        private readonly ScreeningService screeningService;

        public HRDashboardService()
        {
            db = new DatabaseHelper();
            screeningService = new ScreeningService();
        }

        public void ShowDashboard(string hrUsername)
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     HR SCREENING DASHBOARD                   ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                Console.WriteLine("Welcome, HR Staff!\n");
                Console.WriteLine("1. Screen Pending Applications");
                Console.WriteLine("2. View All Applications");
                Console.WriteLine("3. Filter Applications by Job");
                Console.WriteLine("4. Filter Applications by Status");
                Console.WriteLine("5. Logout");
                Console.Write("\nChoose an option: ");

                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ScreenPendingApplications(hrUsername);
                        break;
                    case "2":
                        ViewAllApplications();
                        break;
                    case "3":
                        FilterApplicationsByJob();
                        break;
                    case "4":
                        FilterApplicationsByStatus();
                        break;
                    case "5":
                        running = false;
                        Console.WriteLine("\nLogging out...");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        System.Threading.Thread.Sleep(2000);
                        break;
                }
            }
        }

        private void ScreenPendingApplications(string hrUsername)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     PENDING APPLICATIONS FOR SCREENING       ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var pendingApps = db.GetPendingApplicationsForScreening();

            if (pendingApps.Count == 0)
            {
                Console.WriteLine("No pending applications for screening.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Found {pendingApps.Count} application(s) pending screening.\n");
            Console.WriteLine($"{"#",-3} {"Name",-20} {"Job Title",-25} {"Applied",-12}");
            Console.WriteLine(new string('-', 65));

            for (int i = 0; i < pendingApps.Count; i++)
            {
                var app = pendingApps[i];
                string name = $"{app.FirstName} {app.LastName}";
                string jobTitle = ((string)app.JobTitle).Length > 24 ? ((string)app.JobTitle).Substring(0, 24) : (string)app.JobTitle;
                string dateApplied = ((DateTime)app.DateApplied).ToString("yyyy-MM-dd");
                Console.WriteLine($"{i + 1,-3} {name,-20} {jobTitle,-25} {dateApplied,-12}");
            }

            Console.WriteLine($"\n{pendingApps.Count + 1}. Back");
            Console.Write("\nSelect application to screen: ");

            if (int.TryParse(Console.ReadLine()?.Trim(), out int choice) && choice >= 1 && choice <= pendingApps.Count)
            {
                var selectedApp = pendingApps[choice - 1];
                screeningService.ScreenApplication(selectedApp.ApplicationID, hrUsername);
            }
        }

        private void ViewAllApplications()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     ALL APPLICATIONS                         ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var allApps = db.GetApplicationsByStatus("Submitted");
            var shortlistedApps = db.GetApplicationsByStatus("Shortlisted");
            var rejectedApps = db.GetApplicationsByStatus("Rejected");

            Console.WriteLine($"Submitted: {allApps.Count}");
            Console.WriteLine($"Shortlisted: {shortlistedApps.Count}");
            Console.WriteLine($"Rejected: {rejectedApps.Count}\n");

            DisplayApplicationsList(allApps, "Submitted");
            DisplayApplicationsList(shortlistedApps, "Shortlisted");
            DisplayApplicationsList(rejectedApps, "Rejected");

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void FilterApplicationsByJob()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     FILTER BY JOB POSITION                   ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var allJobs = db.GetAllJobVacancies();

            if (allJobs.Count == 0)
            {
                Console.WriteLine("No job positions available.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Select a job position:\n");
            for (int i = 0; i < allJobs.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allJobs[i].JobTitle}");
            }
            Console.WriteLine($"{allJobs.Count + 1}. Back");

            Console.Write("\nChoose option: ");

            if (int.TryParse(Console.ReadLine()?.Trim(), out int choice) && choice >= 1 && choice <= allJobs.Count)
            {
                var selectedJob = allJobs[choice - 1];
                var applications = db.GetApplicationsByJob(selectedJob.JobID);

                Console.Clear();
                Console.WriteLine($"Applications for: {selectedJob.JobTitle}\n");
                DisplayApplicationsList(applications, "All Statuses");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void FilterApplicationsByStatus()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     FILTER BY STATUS                         ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.WriteLine("Select a status:\n");
            Console.WriteLine("1. Submitted");
            Console.WriteLine("2. Under Review");
            Console.WriteLine("3. Shortlisted");
            Console.WriteLine("4. Rejected");
            Console.WriteLine("5. Accepted");
            Console.WriteLine("6. Back");

            Console.Write("\nChoose option: ");

            string statusMap = Console.ReadLine()?.Trim() ?? string.Empty;
            string status = statusMap switch
            {
                "1" => "Submitted",
                "2" => "Under Review",
                "3" => "Shortlisted",
                "4" => "Rejected",
                "5" => "Accepted",
                _ => null
            };

            if (status != null)
            {
                var applications = db.GetApplicationsByStatus(status);

                Console.Clear();
                Console.WriteLine($"Applications with status: {status}\n");
                DisplayApplicationsList(applications, status);
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void DisplayApplicationsList(List<dynamic> applications, string statusLabel)
        {
            if (applications.Count == 0)
            {
                Console.WriteLine($"No applications found for {statusLabel}.\n");
                return;
            }

            Console.WriteLine($"\n{statusLabel}: {applications.Count}\n");
            Console.WriteLine($"{"Name",-20} {"Job Title",-25} {"Status",-15} {"Applied",-12}");
            Console.WriteLine(new string('-', 75));

            foreach (var app in applications)
            {
                string name = $"{app.FirstName} {app.LastName}";
                string jobTitle = ((string)app.JobTitle).Length > 24 ? ((string)app.JobTitle).Substring(0, 24) : (string)app.JobTitle;
                string status = ((string)app.Status).Length > 14 ? ((string)app.Status).Substring(0, 14) : (string)app.Status;
                string dateApplied = ((DateTime)app.DateApplied).ToString("yyyy-MM-dd");
                Console.WriteLine($"{name,-20} {jobTitle,-25} {status,-15} {dateApplied,-12}");
            }
        }
    }
}
