using HRAndApplicantSystem.Services.Business;

namespace HRAndApplicantSystem.Services.UI
{
    /// <summary>
    /// UI rendering for applicant dashboard summary.
    /// Handles all console output and user interaction.
    /// Uses DashboardSummaryBusinessService for data retrieval.
    /// </summary>
    public class DashboardSummaryUIService
    {
        private readonly DashboardSummaryBusinessService businessService;

        public DashboardSummaryUIService(DashboardSummaryBusinessService businessService = null)
        {
            this.businessService = businessService ?? new DashboardSummaryBusinessService();
        }

        /// <summary>
        /// Display the applicant dashboard summary
        /// </summary>
        public void ShowDashboard(int applicantId, string username)
        {
            // Get data from business service
            var dashboardData = businessService.GetDashboardData(applicantId, username);

            // Render dashboard
            RenderDashboardHeader();
            RenderApplicationStats(dashboardData);
            RenderRecentApplications(dashboardData);
            RenderNextSteps(dashboardData);

            Console.WriteLine("\nPress any key to return to dashboard...");
            Console.ReadKey();
        }

        private void RenderDashboardHeader()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     DASHBOARD SUMMARY                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");
        }

        private void RenderApplicationStats(DashboardSummaryData data)
        {
            Console.WriteLine("=== APPLICATION STATISTICS ===\n");
            Console.WriteLine($"Total Applications:     {data.TotalApplications}");
            Console.WriteLine($"Pending Review:         {data.PendingCount}");
            Console.WriteLine($"Accepted:               {data.AcceptedCount}");
            Console.WriteLine($"Interview Scheduled:    {data.Applications.Count(a => a.ApplicationStatus == Models.ApplicationStatus.InterviewScheduled)}");
            Console.WriteLine($"Rejected:               {data.Applications.Count(a => a.ApplicationStatus == Models.ApplicationStatus.Rejected)}\n");
        }

        private void RenderRecentApplications(DashboardSummaryData data)
        {
            if (data.RecentApplications.Count == 0)
            {
                Console.WriteLine("=== RECENT APPLICATIONS ===\nNo applications yet.\n");
                return;
            }

            Console.WriteLine("=== RECENT APPLICATIONS ===\n");
            Console.WriteLine($"{"Title",-30} {"Status",-18} {"Date Applied",-12}");
            Console.WriteLine(new string('-', 60));

            foreach (var app in data.RecentApplications)
            {
                string title = ((string)app.JobTitle).Length > 29 
                    ? ((string)app.JobTitle).Substring(0, 29) 
                    : (string)app.JobTitle;
                string status = ((string)app.ApplicationStatus).Length > 17 
                    ? ((string)app.ApplicationStatus).Substring(0, 17) 
                    : (string)app.ApplicationStatus;
                string date = ((DateTime)app.DateApplied).ToString("yyyy-MM-dd");

                Console.WriteLine($"{title,-30} {status,-18} {date,-12}");
            }
            Console.WriteLine();
        }

        private void RenderNextSteps(DashboardSummaryData data)
        {
            var nextSteps = businessService.GetNextSteps(data.Applications);

            Console.WriteLine("=== NEXT STEPS ===\n");
            if (nextSteps.Count == 0)
            {
                Console.WriteLine("✓ All set! Continue exploring job opportunities.\n");
                return;
            }

            for (int i = 0; i < nextSteps.Count; i++)
            {
                Console.WriteLine($"• {nextSteps[i]}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Get application count for display purposes
        /// </summary>
        public int GetApplicationCount(int applicantId)
        {
            var summary = businessService.GetDashboardData(applicantId, "");
            return summary.TotalApplications;
        }

        /// <summary>
        /// Get pending application count for display purposes
        /// </summary>
        public int GetPendingApplicationCount(int applicantId)
        {
            var summary = businessService.GetDashboardData(applicantId, "");
            return summary.PendingCount;
        }
    }
}
