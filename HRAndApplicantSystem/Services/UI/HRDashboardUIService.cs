using HRAndApplicantSystem.Services.Business;

namespace HRAndApplicantSystem.Services.UI
{
    /// <summary>
    /// UI rendering for HR dashboard.
    /// Handles all console output and menu navigation.
    /// Uses HRDashboardBusinessService for business logic.
    /// </summary>
    public class HRDashboardUIService
    {
        private readonly HRDashboardBusinessService businessService;
        private readonly ScreeningUIService screeningUIService;
        private readonly InterviewUIService interviewUIService;

        public HRDashboardUIService(HRDashboardBusinessService businessService = null)
        {
            this.businessService = businessService ?? new HRDashboardBusinessService();
            this.screeningUIService = new ScreeningUIService();
            this.interviewUIService = new InterviewUIService();
        }

        /// <summary>
        /// Show the HR dashboard
        /// </summary>
        public void ShowDashboard(string hrUsername)
        {
            int roleId = businessService.GetUserRoleByUsername(hrUsername);
            bool isManagerOrAdmin = businessService.IsManagerOrAdmin(roleId);

            bool running = true;
            while (running)
            {
                DisplayDashboardHeader();
                DisplayRoleWelcome(businessService.GetRoleDisplayName(roleId));
                DisplayMenuOptions(isManagerOrAdmin);
                Console.Write("\nChoose an option: ");
                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                running = HandleMenuChoice(choice, isManagerOrAdmin, hrUsername);
            }
        }

        private void DisplayDashboardHeader()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     HR SCREENING DASHBOARD                   ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");
        }

        private void DisplayRoleWelcome(string role)
        {
            Console.WriteLine($"Welcome, {role}!\n");
        }

        private void DisplayMenuOptions(bool isManagerOrAdmin)
        {
            Console.WriteLine("1. Screen Pending Applications");
            Console.WriteLine("2. Interview Management");
            Console.WriteLine("3. View All Applications");
            Console.WriteLine("4. Filter Applications by Job");
            Console.WriteLine("5. Filter Applications by Status");
            Console.WriteLine("6. Application Status Transitions");

            if (isManagerOrAdmin)
            {
                Console.WriteLine("7. Job Vacancy Management");
                Console.WriteLine("8. Requirement Management");
                Console.WriteLine("9. Hiring Decision (Accept / Reject)");
                Console.WriteLine("10. View Audit Logs");
                Console.WriteLine("11. Account Settings");
                Console.WriteLine("12. Logout");
            }
            else
            {
                Console.WriteLine("7. View Audit Logs");
                Console.WriteLine("8. Account Settings");
                Console.WriteLine("9. Logout");
            }
        }

        private bool HandleMenuChoice(string choice, bool isManagerOrAdmin, string hrUsername)
        {
            if (isManagerOrAdmin)
            {
                return HandleManagerChoice(choice, hrUsername);
            }
            else
            {
                return HandleHRStaffChoice(choice, hrUsername);
            }
        }

        private bool HandleManagerChoice(string choice, string hrUsername)
        {
            switch (choice)
            {
                case "1":
                    ScreenPendingApplications(hrUsername);
                    break;
                case "2":
                    interviewUIService.ShowInterviewMenu(hrUsername);
                    break;
                case "3":
                    ViewAllApplications();
                    break;
                case "4":
                    FilterApplicationsByJob();
                    break;
                case "5":
                    FilterApplicationsByStatus();
                    break;
                case "6":
                    var statusTransitionService = new ApplicationStatusTransitionService();
                    statusTransitionService.ShowStatusTransitionMenu(hrUsername);
                    break;
                case "7":
                    var jobVacancyService = new JobVacancyManagementService();
                    jobVacancyService.ShowJobVacancyManagementMenu();
                    break;
                case "8":
                    var requirementService = new RequirementManagementService();
                    requirementService.ShowRequirementManagementMenu();
                    break;
                case "9":
                    var hiringService = new HiringDecisionService();
                    hiringService.ShowHiringDecisionMenu(hrUsername);
                    break;
                case "10":
                    ViewAuditLogs();
                    break;
                case "11":
                    var accountService = new AccountSettingsService();
                    accountService.ShowAccountSettings(hrUsername);
                    break;
                case "12":
                    Console.WriteLine("\nLogging out...");
                    return false;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    System.Threading.Thread.Sleep(1500);
                    break;
            }
            return true;
        }

        private bool HandleHRStaffChoice(string choice, string hrUsername)
        {
            switch (choice)
            {
                case "1":
                    ScreenPendingApplications(hrUsername);
                    break;
                case "2":
                    interviewUIService.ShowInterviewMenu(hrUsername);
                    break;
                case "3":
                    ViewAllApplications();
                    break;
                case "4":
                    FilterApplicationsByJob();
                    break;
                case "5":
                    FilterApplicationsByStatus();
                    break;
                case "6":
                    var statusTransitionService = new ApplicationStatusTransitionService();
                    statusTransitionService.ShowStatusTransitionMenu(hrUsername);
                    break;
                case "7":
                    ViewAuditLogs();
                    break;
                case "8":
                    var accountService = new AccountSettingsService();
                    accountService.ShowAccountSettings(hrUsername);
                    break;
                case "9":
                    Console.WriteLine("\nLogging out...");
                    return false;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    System.Threading.Thread.Sleep(1500);
                    break;
            }
            return true;
        }

        private void ScreenPendingApplications(string hrUsername)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     PENDING APPLICATIONS FOR SCREENING       ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var pendingApps = businessService.GetPendingApplicationsForScreening();

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
                screeningUIService.ScreenApplication(selectedApp.ApplicationID, hrUsername);
            }
        }

        private void ViewAllApplications()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     ALL APPLICATIONS                         ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var summary = businessService.GetApplicationStatusSummary();

            Console.WriteLine($"Submitted            : {summary.SubmittedCount}");
            Console.WriteLine($"Under Review         : {summary.UnderReviewCount}");
            Console.WriteLine($"Shortlisted          : {summary.ShortlistedCount}");
            Console.WriteLine($"Interview Scheduled  : {summary.InterviewScheduledCount}");
            Console.WriteLine($"Accepted             : {summary.AcceptedCount}");
            Console.WriteLine($"Rejected             : {summary.RejectedCount}\n");

            DisplayApplicationsList(businessService.GetApplicationsByStatus(Models.ApplicationStatus.Submitted), Models.ApplicationStatus.Submitted);
            DisplayApplicationsList(businessService.GetApplicationsByStatus(Models.ApplicationStatus.UnderReview), Models.ApplicationStatus.UnderReview);
            DisplayApplicationsList(businessService.GetApplicationsByStatus(Models.ApplicationStatus.Shortlisted), Models.ApplicationStatus.Shortlisted);
            DisplayApplicationsList(businessService.GetApplicationsByStatus(Models.ApplicationStatus.InterviewScheduled), Models.ApplicationStatus.InterviewScheduled);
            DisplayApplicationsList(businessService.GetApplicationsByStatus(Models.ApplicationStatus.Accepted), Models.ApplicationStatus.Accepted);
            DisplayApplicationsList(businessService.GetApplicationsByStatus(Models.ApplicationStatus.Rejected), Models.ApplicationStatus.Rejected);

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void FilterApplicationsByJob()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     FILTER BY JOB POSITION                   ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var allJobs = businessService.GetAllJobVacancies();

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
                var applications = businessService.GetApplicationsByJob(selectedJob.JobID);

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

            Console.WriteLine("1. Submitted");
            Console.WriteLine("2. Under Review");
            Console.WriteLine("3. Shortlisted");
            Console.WriteLine("4. Interview Scheduled");
            Console.WriteLine("5. Accepted");
            Console.WriteLine("6. Rejected");
            Console.WriteLine("7. Back");

            Console.Write("\nChoose option: ");

            string statusMap = Console.ReadLine()?.Trim() ?? string.Empty;
            string status = statusMap switch
            {
                "1" => Models.ApplicationStatus.Submitted,
                "2" => Models.ApplicationStatus.UnderReview,
                "3" => Models.ApplicationStatus.Shortlisted,
                "4" => Models.ApplicationStatus.InterviewScheduled,
                "5" => Models.ApplicationStatus.Accepted,
                "6" => Models.ApplicationStatus.Rejected,
                _ => null
            };

            if (status != null)
            {
                var applications = businessService.GetApplicationsByStatus(status);

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
                string appStatus = ((string)app.ApplicationStatus).Length > 14 ? ((string)app.ApplicationStatus).Substring(0, 14) : (string)app.ApplicationStatus;
                string date = ((DateTime)app.DateApplied).ToString("yyyy-MM-dd");
                Console.WriteLine($"{name,-20} {jobTitle,-25} {appStatus,-15} {date,-12}");
            }
        }

        private void ViewAuditLogs()
        {
            var auditService = new AuditLogService();
            auditService.ViewAuditLogs();
        }
    }
}
