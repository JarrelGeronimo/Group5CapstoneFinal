using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;

namespace HRAndApplicantSystem.Services
{
    public class ReportsService
    {
        private readonly IApplicationRepository applicationRepository;
        private readonly DatabaseHelper db;

        public ReportsService(IApplicationRepository appRepo = null)
        {
            applicationRepository = appRepo ?? new ApplicationRepository(new DatabaseHelper());
            db = new DatabaseHelper();
        }

        // ═══════════════════════════════════════════════════════════════════════
        // PUBLIC DATA-RETURNING METHODS (for external use/UI binding)
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Get application metrics data (for external UI use)
        /// </summary>
        public dynamic GetApplicationMetricsData()
        {
            return db.GetApplicationMetrics();
        }

        /// <summary>
        /// Get interview metrics data (for external UI use)
        /// </summary>
        public dynamic GetInterviewMetricsData()
        {
            return db.GetInterviewMetrics();
        }

        /// <summary>
        /// Get time-to-hire metrics data (for external UI use)
        /// </summary>
        public dynamic GetTimeToHireMetricsData()
        {
            return db.GetTimeToHireMetrics();
        }

        /// <summary>
        /// Get hiring decision metrics data (for external UI use)
        /// </summary>
        public dynamic GetHiringDecisionMetricsData()
        {
            return db.GetHiringDecisionMetrics();
        }

        /// <summary>
        /// Display Reports & Statistics Dashboard
        /// </summary>
        public void ShowReportsMenu()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     REPORTS & STATISTICS                     ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                Console.WriteLine("1. Application Metrics (Total & Status Breakdown)");
                Console.WriteLine("2. Interview Metrics (Pass/Fail Rates)");
                Console.WriteLine("3. Time-to-Hire Metrics");
                Console.WriteLine("4. Hiring Decision Metrics");
                Console.WriteLine("5. View All Reports Summary");
                Console.WriteLine("6. Back\n");

                Console.Write("Choose an option: ");
                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ViewApplicationMetrics();
                        break;
                    case "2":
                        ViewInterviewMetrics();
                        break;
                    case "3":
                        ViewTimeToHireMetrics();
                        break;
                    case "4":
                        ViewHiringDecisionMetrics();
                        break;
                    case "5":
                        ViewAllReportsSummary();
                        break;
                    case "6":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("\nInvalid option. Please try again.");
                        System.Threading.Thread.Sleep(1500);
                        break;
                }
            }
        }

        private void ViewApplicationMetrics()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     APPLICATION METRICS                      ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var metrics = db.GetApplicationMetrics();

            if (metrics != null)
            {
                Console.WriteLine($"Total Applications Received: {metrics.TotalApplications}\n");
                Console.WriteLine("Status Breakdown:");
                Console.WriteLine(new string('-', 40));

                if (metrics.StatusBreakdown != null && ((List<dynamic>)metrics.StatusBreakdown).Count > 0)
                {
                    foreach (var status in (List<dynamic>)metrics.StatusBreakdown)
                    {
                        string statusName = status.Status;
                        int count = status.Count;
                        double percentage = metrics.TotalApplications > 0 
                            ? (count * 100.0) / metrics.TotalApplications 
                            : 0;
                        
                        Console.WriteLine($"  {statusName,-20}: {count,4} ({percentage:F1}%)");
                    }
                }
                Console.WriteLine(new string('-', 40));
            }
            else
            {
                Console.WriteLine("Unable to retrieve metrics.");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void ViewInterviewMetrics()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     INTERVIEW METRICS                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var metrics = db.GetInterviewMetrics();

            if (metrics != null)
            {
                Console.WriteLine($"Total Interviews Evaluated: {metrics.TotalInterviews}\n");
                Console.WriteLine("Interview Results:");
                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"  Passed          : {metrics.PassCount,4} ({metrics.PassRate:F1}%)");
                Console.WriteLine($"  Failed          : {metrics.FailCount,4} ({metrics.FailRate:F1}%)");
                Console.WriteLine(new string('-', 50));

                if (metrics.ResultBreakdown != null && ((List<dynamic>)metrics.ResultBreakdown).Count > 0)
                {
                    Console.WriteLine("\nDetailed Results:");
                    foreach (var result in (List<dynamic>)metrics.ResultBreakdown)
                    {
                        string resultName = result.Result;
                        int count = result.Count;
                        Console.WriteLine($"  {resultName,-30}: {count}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Unable to retrieve metrics.");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void ViewTimeToHireMetrics()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     TIME-TO-HIRE METRICS                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var metrics = db.GetTimeToHireMetrics();

            if (metrics != null)
            {
                Console.WriteLine($"Total Applications Tracked: {metrics.TotalApplications}\n");
                Console.WriteLine("Time-to-Hire Summary (Days):");
                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"  Average         : {metrics.AverageDaysToHire:F2} days");
                Console.WriteLine($"  Median          : {metrics.MedianDaysToHire:F2} days");
                Console.WriteLine($"  Minimum         : {metrics.MinDaysToHire} days");
                Console.WriteLine($"  Maximum         : {metrics.MaxDaysToHire} days");
                Console.WriteLine(new string('-', 50));
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void ViewHiringDecisionMetrics()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     HIRING DECISION METRICS                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var metrics = db.GetHiringDecisionMetrics();

            if (metrics != null)
            {
                Console.WriteLine($"Total Hiring Decisions Made: {metrics.TotalDecisions}\n");
                Console.WriteLine("Decision Summary:");
                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"  Offers/Hired    : {metrics.OfferedCount,4} ({metrics.OfferRate:F1}%)");
                Console.WriteLine($"  Rejected        : {metrics.RejectedCount,4} ({metrics.RejectionRate:F1}%)");
                Console.WriteLine(new string('-', 50));
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void ViewAllReportsSummary()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     COMPLETE REPORTS SUMMARY                 ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var appMetrics = db.GetApplicationMetrics();
            var interviewMetrics = db.GetInterviewMetrics();
            var hireMetrics = db.GetTimeToHireMetrics();
            var decisionMetrics = db.GetHiringDecisionMetrics();

            if (appMetrics != null)
            {
                Console.WriteLine("▶ APPLICATION SUMMARY");
                Console.WriteLine($"  Total Applications: {appMetrics.TotalApplications}");
            }

            if (interviewMetrics != null)
            {
                Console.WriteLine("\n▶ INTERVIEW SUMMARY");
                Console.WriteLine($"  Total Interviews: {interviewMetrics.TotalInterviews}");
                Console.WriteLine($"  Pass Rate: {interviewMetrics.PassRate:F1}%");
            }

            if (hireMetrics != null)
            {
                Console.WriteLine("\n▶ TIME-TO-HIRE SUMMARY");
                Console.WriteLine($"  Average Time: {hireMetrics.AverageDaysToHire:F2} days");
            }

            if (decisionMetrics != null)
            {
                Console.WriteLine("\n▶ HIRING DECISION SUMMARY");
                Console.WriteLine($"  Total Decisions: {decisionMetrics.TotalDecisions}");
                Console.WriteLine($"  Offer Rate: {decisionMetrics.OfferRate:F1}%");
            }

            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }
    }
}
