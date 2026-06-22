using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Services
{
    public class DashboardSummaryService
    {
        private readonly IApplicationRepository applicationRepository;
        private readonly DatabaseHelper db;

        public DashboardSummaryService(IApplicationRepository appRepo = null)
        {
            applicationRepository = appRepo ?? new ApplicationRepository(new DatabaseHelper());
            db = new DatabaseHelper();
        }

        public void ShowDashboard(Applicant applicant, string username)
        {
            bool dashboardRunning = true;

            while (dashboardRunning)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     APPLICATION DASHBOARD SUMMARY            ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                // Get applicant's applications
                var applications = applicationRepository.GetApplicantApplications(applicant.ApplicantID);

                if (applications.Count == 0)
                {
                    Console.WriteLine("No applications submitted yet.\n");
                    Console.WriteLine("Start by browsing job vacancies and applying to positions that interest you.");
                    Console.WriteLine("\n1. Back to Menu");
                    Console.Write("\nChoose option: ");
                    string choice = Console.ReadLine()?.Trim() ?? "1";
                    if (choice == "1") break;
                    continue;
                }

                // Display application summary
                Console.WriteLine($"Total Applications: {applications.Count}\n");
                Console.WriteLine("=== Your Application Status ===\n");

                // Count applications by status
                var statusCounts = new Dictionary<string, int>();
                foreach (var app in applications)
                {
                    if (statusCounts.ContainsKey(app.ApplicationStatus))
                        statusCounts[app.ApplicationStatus]++;
                    else
                        statusCounts[app.ApplicationStatus] = 1;
                }

                foreach (var status in statusCounts.OrderBy(s => s.Key))
                {
                    Console.WriteLine($"  • {status.Key}: {status.Value}");
                }

                // Display recent applications
                Console.WriteLine("\n=== Recent Applications ===\n");
                var recentApps = applications.OrderByDescending(a => a.DateApplied).Take(3).ToList();

                foreach (var app in recentApps)
                {
                    Console.WriteLine($"Job: {app.JobTitle}");
                    Console.WriteLine($"Status: {app.ApplicationStatus}");
                    Console.WriteLine($"Applied: {app.DateApplied:MMMM dd, yyyy}");
                    Console.WriteLine(new string('-', 40));
                }

                // Show upcoming actions
                Console.WriteLine("\n=== Quick Actions ===\n");
                bool hasPendingReview = applications.Any(a => a.ApplicationStatus == ApplicationStatus.Submitted || a.ApplicationStatus == ApplicationStatus.UnderReview);
                bool hasAccepted = applications.Any(a => a.ApplicationStatus == ApplicationStatus.Accepted);
                bool hasInterviewScheduled = applications.Any(a => a.ApplicationStatus == ApplicationStatus.InterviewScheduled);

                Console.WriteLine("1. View Interview Schedule");
                Console.WriteLine("2. View Application Status Timeline");
                Console.WriteLine("3. Refresh Dashboard");
                Console.WriteLine("4. Back to Menu");

                if (hasPendingReview)
                    Console.WriteLine("  ⏳ You have pending applications");
                if (hasInterviewScheduled)
                    Console.WriteLine("  📅 You have upcoming interviews");
                if (hasAccepted)
                    Console.WriteLine("  ✓ You have accepted applications!");

                Console.Write("\nChoose option: ");
                string menuChoice = Console.ReadLine()?.Trim() ?? "4";

                switch (menuChoice)
                {
                    case "1":
                        ShowInterviewSchedule(applicant.ApplicantID);
                        break;
                    case "2":
                        ShowApplicationTimeline(applications);
                        break;
                    case "3":
                        // Refresh - loop continues
                        break;
                    case "4":
                        dashboardRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        System.Threading.Thread.Sleep(1500);
                        break;
                }
            }
        }

        private void ShowInterviewSchedule(int applicantID)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     MY INTERVIEW SCHEDULE                    ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var interviews = db.GetApplicantInterviews(applicantID);

            if (interviews.Count == 0)
            {
                Console.WriteLine("No interviews scheduled yet.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"Job Title",-30} {"Date",-15} {"Time",-10} {"Location",-30}");
            Console.WriteLine(new string('-', 85));

            foreach (var interview in interviews)
            {
                DateTime interviewDate = interview.InterviewDate;
                DateTime interviewTime = interview.InterviewTime;
                string time = interviewTime.ToString("HH:mm");
                string location = ((string)interview.Location).Length > 29 ? ((string)interview.Location).Substring(0, 29) : (string)interview.Location;
                
                Console.WriteLine($"{(string)interview.JobTitle,-30} {interviewDate:yyyy-MM-dd,-15} {time,-10} {location,-30}");
            }

            Console.WriteLine(new string('-', 85));
            Console.WriteLine($"\nTotal Interviews: {interviews.Count}");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void ShowApplicationTimeline(List<dynamic> applications)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     APPLICATION STATUS TIMELINE              ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            if (applications.Count == 0)
            {
                Console.WriteLine("No applications to display timeline.");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            Console.WriteLine($"{"#",-3} {"Job Title",-30} {"Status",-20}");
            Console.WriteLine(new string('-', 55));

            for (int i = 0; i < applications.Count; i++)
            {
                var app = applications[i];
                string jobTitle = ((string)app.JobTitle).Length > 29 ? ((string)app.JobTitle).Substring(0, 29) : (string)app.JobTitle;
                Console.WriteLine($"{i + 1,-3} {jobTitle,-30} {(string)app.ApplicationStatus,-20}");
            }

            Console.WriteLine(new string('-', 55));
            Console.WriteLine($"\n{applications.Count + 1}. Back");
            Console.Write("\nSelect application to view timeline: ");

            if (int.TryParse(Console.ReadLine()?.Trim(), out int choice) && choice >= 1 && choice <= applications.Count)
            {
                var selected = applications[choice - 1];
                db.DisplayApplicationTimeline((int)selected.ApplicationID);
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        public int GetApplicationCount(int applicantId)
        {
            var applications = applicationRepository.GetApplicantApplications(applicantId);
            return applications.Count;
        }

        public int GetPendingApplicationCount(int applicantId)
        {
            var applications = applicationRepository.GetApplicantApplications(applicantId);
            return applications.Count(a => a.ApplicationStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase));
        }
    }
}
