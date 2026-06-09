using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Services
{
    public class HRDashboardService
    {
        private readonly DatabaseHelper db;
        private readonly IApplicationRepository applicationRepository;
        private readonly ScreeningService screeningService;
        private readonly InterviewService interviewService;
        private readonly HiringDecisionService hiringDecisionService;
        private readonly JobVacancyManagementService jobVacancyService;
        private readonly AuditLogService auditLogService;
        private readonly AccountSettingsService accountSettingsService;
        private readonly ApplicationStatusTransitionService statusTransitionService;
        private readonly ReportsService reportsService;
        private readonly ApplicantSearchService applicantSearchService;
        private readonly RequirementManagementService requirementManagementService;

        public HRDashboardService(IApplicationRepository appRepo = null)
        {
            db = new DatabaseHelper();
            applicationRepository = appRepo ?? new ApplicationRepository(new DatabaseHelper());
            screeningService = new ScreeningService();
            interviewService = new InterviewService();
            hiringDecisionService = new HiringDecisionService();
            jobVacancyService = new JobVacancyManagementService();
            auditLogService = new AuditLogService();
            accountSettingsService = new AccountSettingsService();
            reportsService = new ReportsService(); 
            applicantSearchService = new ApplicantSearchService();
            statusTransitionService = new ApplicationStatusTransitionService();
            requirementManagementService = new RequirementManagementService();
        }

        public void ShowDashboard(string hrUsername)
        {
            int roleId = db.GetUserRoleByUsername(hrUsername);
            bool isManagerOrAdmin = roleId == RoleConstants.HR_MANAGER || roleId == RoleConstants.ADMIN;

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     HR SCREENING DASHBOARD                   ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                 string role = roleId == RoleConstants.HR_MANAGER ? "HR Manager"
                            : roleId == RoleConstants.ADMIN ? "Admin"
                            : "HR Staff";
                Console.WriteLine($"Welcome, {role}!\n");

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

                Console.Write("\nChoose an option: ");
                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                if (isManagerOrAdmin)
                {
                    switch (choice)
                    {
                        case "1": ScreenPendingApplications(hrUsername); break;
                        case "2": interviewService.ShowInterviewMenu(hrUsername); break;
                        case "3": ViewAllApplications(); break;
                        case "4": FilterApplicationsByJob(); break;
                        case "5": FilterApplicationsByStatus(); break;
                        case "6": statusTransitionService.ShowStatusTransitionMenu(hrUsername); break;
                        case "7": jobVacancyService.ShowJobVacancyManagementMenu(); break;
                        case "8": requirementManagementService.ShowRequirementManagementMenu(); break;
                        case "9": hiringDecisionService.ShowHiringDecisionMenu(hrUsername); break;
                        case "10": ViewAuditLogs(); break;
                        case "11": accountSettingsService.ShowAccountSettings(hrUsername); break;
                        case "12":
                            running = false;
                            Console.WriteLine("\nLogging out...");
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            System.Threading.Thread.Sleep(1500);
                            break;
                    }
                }
                    else
                {
                    switch (choice)
                    {
                        case "1": ScreenPendingApplications(hrUsername); break;
                        case "2": interviewService.ShowInterviewMenu(hrUsername); break;
                        case "3": ViewAllApplications(); break;
                        case "4": FilterApplicationsByJob(); break;
                        case "5": FilterApplicationsByStatus(); break;
                        case "6": statusTransitionService.ShowStatusTransitionMenu(hrUsername); break;
                        case "7": ViewAuditLogs(); break;
                        case "8": accountSettingsService.ShowAccountSettings(hrUsername); break;
                        case "9":
                            running = false;
                            Console.WriteLine("\nLogging out...");
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            System.Threading.Thread.Sleep(1500);
                            break;
                    }
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

            var submitted       = db.GetApplicationsByStatus(ApplicationStatus.Submitted);
            var underReview     = db.GetApplicationsByStatus(ApplicationStatus.UnderReview);
            var shortlisted     = db.GetApplicationsByStatus(ApplicationStatus.Shortlisted);
            var interviewScheduled = db.GetApplicationsByStatus(ApplicationStatus.InterviewScheduled);
            var accepted        = db.GetApplicationsByStatus(ApplicationStatus.Accepted);
            var rejected        = db.GetApplicationsByStatus(ApplicationStatus.Rejected);

            Console.WriteLine($"Submitted            : {submitted.Count}");
            Console.WriteLine($"Under Review         : {underReview.Count}");
            Console.WriteLine($"Shortlisted          : {shortlisted.Count}");
            Console.WriteLine($"Interview Scheduled  : {interviewScheduled.Count}");
            Console.WriteLine($"Accepted             : {accepted.Count}");
            Console.WriteLine($"Rejected             : {rejected.Count}\n");

            DisplayApplicationsList(submitted,       ApplicationStatus.Submitted);
            DisplayApplicationsList(underReview,     ApplicationStatus.UnderReview);
            DisplayApplicationsList(shortlisted,     ApplicationStatus.Shortlisted);
            DisplayApplicationsList(interviewScheduled, ApplicationStatus.InterviewScheduled);
            DisplayApplicationsList(accepted,        ApplicationStatus.Accepted);
            DisplayApplicationsList(rejected,        ApplicationStatus.Rejected);

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
                "1" => ApplicationStatus.Submitted,
                "2" => ApplicationStatus.UnderReview,
                "3" => ApplicationStatus.Shortlisted,
                "4" => ApplicationStatus.InterviewScheduled,
                "5" => ApplicationStatus.Accepted,
                "6" => ApplicationStatus.Rejected,
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

        private void ViewAuditLogs()
        {
            auditLogService.ViewAuditLogs();
        }
    }
}
