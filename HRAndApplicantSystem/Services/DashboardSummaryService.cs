using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Services
{
    public class DashboardSummaryService
    {
        private readonly DatabaseHelper db;

        public DashboardSummaryService()
        {
            db = new DatabaseHelper();
        }

        public void ShowDashboard(Applicant applicant, string username)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     APPLICATION DASHBOARD SUMMARY            ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            // Get applicant's applications
            var applications = db.GetApplicantApplications(applicant.ApplicantID);

            if (applications.Count == 0)
            {
                Console.WriteLine("No applications submitted yet.\n");
                Console.WriteLine("Start by browsing job vacancies and applying to positions that interest you.");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            // Display application summary
            Console.WriteLine($"Total Applications: {applications.Count}\n");
            Console.WriteLine("=== Your Application Status ===\n");

            // Count applications by status
            var statusCounts = new Dictionary<string, int>();
            foreach (var app in applications)
            {
                if (statusCounts.ContainsKey(app.Status))
                    statusCounts[app.Status]++;
                else
                    statusCounts[app.Status] = 1;
            }

            foreach (var status in statusCounts.OrderBy(s => s.Key))
            {
                Console.WriteLine($"  • {status.Key}: {status.Value}");
            }

            // Display recent applications
            Console.WriteLine("\n=== Recent Applications ===\n");
            var recentApps = applications.OrderByDescending(a => a.DateApplied).Take(5).ToList();

            foreach (var app in recentApps)
            {
                Console.WriteLine($"Job: {app.JobTitle}");
                Console.WriteLine($"Status: {app.Status}");
                Console.WriteLine($"Applied: {app.DateApplied:MMMM dd, yyyy}");
                Console.WriteLine(new string('-', 40));
            }

            // Show upcoming actions/missing documents placeholder
            Console.WriteLine("\n=== Next Steps ===\n");
            bool hasPendingReview = applications.Any(a => a.Status == "Submitted" || a.Status == "Under Review");
            bool hasAccepted = applications.Any(a => a.Status == "Accepted");
            bool hasInterviewScheduled = applications.Any(a => a.Status == "Interview Scheduled");
            bool hasDrafts = false; // No draft status in schema

            // Draft applications not applicable - applications go directly to Submitted status

            if (hasPendingReview)
            {
                Console.WriteLine("  ⏳ You have applications pending HR review");
                Console.WriteLine("     Check back soon for updates!");
            }

            if (hasInterviewScheduled)
            {
                Console.WriteLine("  📅 You have scheduled interview(s)");
                Console.WriteLine("    Check your email for interview details and date/time");
            }

            if (hasAccepted)
            {
                Console.WriteLine("  ✓ Congratulations! You have been accepted for an interview");
                Console.WriteLine("    Check your email for next steps");
            }

            if (!hasDrafts && !hasPendingReview && !hasInterviewScheduled && !hasAccepted)
            {
                Console.WriteLine("  📧 Stay tuned for updates on your applications");
                Console.WriteLine("  💡 Continue exploring and applying to more job vacancies");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        public int GetApplicationCount(int applicantId)
        {
            var applications = db.GetApplicantApplications(applicantId);
            return applications.Count;
        }

        public int GetPendingApplicationCount(int applicantId)
        {
            var applications = db.GetApplicantApplications(applicantId);
            return applications.Count(a => a.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase));
        }
    }
}
