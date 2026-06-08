using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Utilities;

namespace HRAndApplicantSystem.Services
{
    public class ApplicantDashboardService
    {
        private readonly DashboardSummaryService dashboardSummaryService;
        private readonly ApplicantProfileService profileService;
        private readonly JobVacancyService jobVacancyService;
        private readonly ApplicationManagementService applicationManagementService;
        private readonly AccountSettingsService accountSettingsService;
        private readonly ApplicantApplicationHistoryService applicationHistoryService;

        public ApplicantDashboardService()
        {
            dashboardSummaryService = new DashboardSummaryService();
            profileService = new ApplicantProfileService();
            jobVacancyService = new JobVacancyService();
            applicationManagementService = new ApplicationManagementService();
            accountSettingsService = new AccountSettingsService();
            applicationHistoryService = new ApplicantApplicationHistoryService();
        }

        public void ShowDashboard(Applicant applicant, string username)
        {
            if (applicant == null)
            {
                Console.WriteLine("Error: Applicant information not found.");
                return;
            }

            bool dashboardRunning = true;
            while (dashboardRunning)
            {
                Console.Clear();
                Console.WriteLine($"╔══════════════════════════════════════════════╗");
                Console.WriteLine($"║ Welcome back, {applicant.FirstName} {applicant.LastName}!                  ║");
                Console.WriteLine($"╚══════════════════════════════════════════════╝\n");

                Console.WriteLine("=== Applicant Dashboard Main Menu ===\n");
                Console.WriteLine("1. View Application Dashboard");
                Console.WriteLine("2. Manage My Profile");
                Console.WriteLine("3. Browse Job Vacancies");
                Console.WriteLine("4. View My Applications");
                Console.WriteLine("5. My Application History");
                Console.WriteLine("6. Account Settings");
                Console.WriteLine("7. Logout");
                Console.Write("\nChoose an option: ");

                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        dashboardSummaryService.ShowDashboard(applicant, username);
                        break;
                    case "2":
                        profileService.ShowProfileMenu(applicant, username);
                        break;
                    case "3":
                        jobVacancyService.BrowseJobVacancies(applicant);
                        break;
                    case "4":
                        applicationManagementService.ManageApplications(applicant);
                        break;
                    case "5":
                        applicationHistoryService.ShowMyApplications(username);
                        break;
                    case "6":
                        accountSettingsService.ShowAccountSettings(username);
                        break;
                    case "7":
                        dashboardRunning = false;
                        Console.WriteLine("\nLogging out...");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        System.Threading.Thread.Sleep(2000);
                        break;
                }
            }
        }
    }
}
