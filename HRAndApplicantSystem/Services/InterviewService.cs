using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Utilities;

namespace HRAndApplicantSystem.Services
{
    public class InterviewService
    {
        private readonly IInterviewRepository interviewRepository;
        private readonly IApplicationRepository applicationRepository;
        private readonly DatabaseHelper db;

        public InterviewService(IInterviewRepository interRepo = null, IApplicationRepository appRepo = null)
        {
            interviewRepository = interRepo ?? new InterviewRepository(new DatabaseHelper());
            applicationRepository = appRepo ?? new ApplicationRepository(new DatabaseHelper());
            db = new DatabaseHelper(); // Temporary for non-repository operations
        }

        // ── Show shortlisted applicants and pick one to schedule ──
        public void ShowInterviewMenu(string hrUsername)
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     INTERVIEW MANAGEMENT                     ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                Console.WriteLine("1. Schedule Interview (Shortlisted Applicants)");
                Console.WriteLine("2. Evaluate Interview (For Interview Applicants)");
                Console.WriteLine("3. Cancel Interview");
                Console.WriteLine("4. Reschedule Interview");
                Console.WriteLine("5. Back to Dashboard");
                Console.Write("\nChoose an option: ");

                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ScheduleInterview(hrUsername);
                        break;
                    case "2":
                        EvaluateInterview(hrUsername);
                        break;
                    case "3":
                        CancelInterview(hrUsername);
                        break;
                    case "4":
                        RescheduleInterview(hrUsername);
                        break;
                    case "5":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        System.Threading.Thread.Sleep(1500);
                        break;
                }
            }
        }

        // ── Schedule Interview ────────────────────────────────────
        private void ScheduleInterview(string hrUsername)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     SCHEDULE INTERVIEW                       ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var shortlisted = db.GetApplicationsByStatus(ApplicationStatus.Shortlisted);

            if (shortlisted.Count == 0)
            {
                Console.WriteLine("No shortlisted applicants available for interview.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"#",-3} {"Name",-22} {"Job Title",-25} {"Applied",-12}");
            Console.WriteLine(new string('-', 65));

            for (int i = 0; i < shortlisted.Count; i++)
            {
                var app = shortlisted[i];
                string name = $"{app.FirstName} {app.LastName}";
                string jobTitle = ((string)app.JobTitle).Length > 24
                    ? ((string)app.JobTitle).Substring(0, 24)
                    : (string)app.JobTitle;
                string date = ((DateTime)app.DateApplied).ToString("yyyy-MM-dd");
                Console.WriteLine($"{i + 1,-3} {name,-22} {jobTitle,-25} {date,-12}");
            }

            Console.WriteLine($"\n{shortlisted.Count + 1}. Back");
            Console.Write("\nSelect applicant to schedule: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice)
                || choice < 1 || choice > shortlisted.Count)
            {
                Console.WriteLine("Returning...");
                System.Threading.Thread.Sleep(1000);
                return;
            }

            var selected = shortlisted[choice - 1];

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     INTERVIEW DETAILS                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.WriteLine($"Applicant : {selected.FirstName} {selected.LastName}");
            Console.WriteLine($"Position  : {selected.JobTitle}\n");

            // ── Interview date ──
            DateTime interviewDate = DateTime.MinValue;
            while (interviewDate == DateTime.MinValue)
            {
                Console.Write("Interview Date (YYYY-MM-DD): ");
                string dateInput = Console.ReadLine()?.Trim() ?? "";
                if (DateTime.TryParse(dateInput, out DateTime parsed) && parsed.Date >= DateTime.Today)
                {
                    interviewDate = parsed;
                }
                else
                {
                    Console.WriteLine("✗ Invalid date or date is in the past. Please try again.");
                }
            }

            // ── Interview time ──
            TimeSpan interviewTime = TimeSpan.Zero;
            while (interviewTime == TimeSpan.Zero)
            {
                Console.Write("Interview Time (HH:MM, 24hr): ");
                string timeInput = Console.ReadLine()?.Trim() ?? "";
                if (TimeSpan.TryParse(timeInput, out TimeSpan parsed))
                {
                    interviewTime = parsed;
                }
                else
                {
                    Console.WriteLine("✗ Invalid time format. Please try again.");
                }
            }

            // ── Mode ──
            Console.WriteLine("\nInterview Mode:");
            Console.WriteLine("1. Face-to-Face");
            Console.WriteLine("2. Online (Video Call)");
            Console.Write("Select mode: ");
            string modeChoice = Console.ReadLine()?.Trim() ?? "1";
            string mode = modeChoice == "2" ? "Online" : "Face-to-Face";

            // ── Location/Link ──
            Console.Write(mode == "Online"
                ? "Meeting Link/Platform: "
                : "Location/Venue: ");
            string location = Console.ReadLine()?.Trim() ?? "";

            // ── Interviewer ──
            Console.Write("Interviewer Name: ");
            string interviewer = Console.ReadLine()?.Trim() ?? hrUsername;

            // ── Save ──
            DateTime fullDateTime = interviewDate.Date + interviewTime;

            bool success = db.ScheduleInterview(
                selected.ApplicationID,
                fullDateTime,
                interviewer,
                mode,
                location,
                hrUsername
            );

            if (success)
            {
                Console.WriteLine("\n✓ Interview scheduled successfully!");
                Console.WriteLine($"  Date     : {fullDateTime:MMMM dd, yyyy HH:mm}");
                Console.WriteLine($"  Mode     : {mode}");
                Console.WriteLine($"  Location : {location}");
                Console.WriteLine($"  Status updated to: Interview Scheduled");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to schedule interview. Please try again.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        // ── Evaluate Interview ────────────────────────────────────
        private void EvaluateInterview(string hrUsername)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     EVALUATE INTERVIEW                       ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var forInterview = db.GetApplicationsByStatus(ApplicationStatus.InterviewScheduled);

            if (forInterview.Count == 0)
            {
                Console.WriteLine("No applicants currently scheduled for interview.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"#",-3} {"Name",-22} {"Job Title",-25} {"Applied",-12}");
            Console.WriteLine(new string('-', 65));

            for (int i = 0; i < forInterview.Count; i++)
            {
                var app = forInterview[i];
                string name = $"{app.FirstName} {app.LastName}";
                string jobTitle = ((string)app.JobTitle).Length > 24
                    ? ((string)app.JobTitle).Substring(0, 24)
                    : (string)app.JobTitle;
                string date = ((DateTime)app.DateApplied).ToString("yyyy-MM-dd");
                Console.WriteLine($"{i + 1,-3} {name,-22} {jobTitle,-25} {date,-12}");
            }

            Console.WriteLine($"\n{forInterview.Count + 1}. Back");
            Console.Write("\nSelect applicant to evaluate: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice)
                || choice < 1 || choice > forInterview.Count)
            {
                Console.WriteLine("Returning...");
                System.Threading.Thread.Sleep(1000);
                return;
            }

            var selected = forInterview[choice - 1];

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     INTERVIEW EVALUATION                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.WriteLine($"Applicant : {selected.FirstName} {selected.LastName}");
            Console.WriteLine($"Position  : {selected.JobTitle}\n");

            // ── Score ──
            int score = -1;
            while (score < 0 || score > 100)
            {
                Console.Write("Interview Score (0-100): ");
                if (int.TryParse(Console.ReadLine()?.Trim(), out int parsed)
                    && parsed >= 0 && parsed <= 100)
                {
                    score = parsed;
                }
                else
                {
                    Console.WriteLine("✗ Please enter a number between 0 and 100.");
                }
            }

            // ── Result ──
            Console.WriteLine("\nInterview Result:");
            Console.WriteLine("1. Pass");
            Console.WriteLine("2. Fail");
            Console.Write("Select result: ");
            string resultChoice = Console.ReadLine()?.Trim() ?? "2";
            string result = resultChoice == "1" ? "Pass" : "Fail";

            // ── Remarks ──
            Console.Write("Remarks/Feedback: ");
            string remarks = Console.ReadLine()?.Trim() ?? "";

            // ── Recommendation ──
            Console.WriteLine("\nRecommendation:");
            Console.WriteLine("1. For Final Review");
            Console.WriteLine("2. Rejected");
            Console.Write("Select recommendation: ");
            string recChoice = Console.ReadLine()?.Trim() ?? "2";
            string newStatus = recChoice == "1" ? "For Final Review" : ApplicationStatus.Rejected;

            bool success = db.EvaluateInterview(
                selected.ApplicationID,
                score,
                result,
                remarks,
                newStatus,
                hrUsername
            );

            if (success)
            {
                Console.WriteLine($"\n✓ Interview evaluation saved!");
                Console.WriteLine($"  Score  : {score}/100");
                Console.WriteLine($"  Result : {result}");
                Console.WriteLine($"  Status updated to: {newStatus}");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to save evaluation. Please try again.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        // ── Cancel Interview ─────────────────────────────────
        private void CancelInterview(string hrUsername)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     CANCEL INTERVIEW                         ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var scheduled = db.GetApplicationsByStatus(ApplicationStatus.InterviewScheduled);

            if (scheduled.Count == 0)
            {
                Console.WriteLine("No scheduled interviews available.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"#",-3} {"Name",-22} {"Job Title",-25} {"Applied",-12}");
            Console.WriteLine(new string('-', 65));

            for (int i = 0; i < scheduled.Count; i++)
            {
                var app = scheduled[i];
                string name = $"{app.FirstName} {app.LastName}";
                string jobTitle = ((string)app.JobTitle).Length > 24
                    ? ((string)app.JobTitle).Substring(0, 24)
                    : (string)app.JobTitle;
                string date = ((DateTime)app.DateApplied).ToString("yyyy-MM-dd");
                Console.WriteLine($"{i + 1,-3} {name,-22} {jobTitle,-25} {date,-12}");
            }

            Console.WriteLine($"\n{scheduled.Count + 1}. Back");
            Console.Write("\nSelect interview to cancel: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice)
                || choice < 1 || choice > scheduled.Count)
            {
                return;
            }

            var selected = scheduled[choice - 1];
            var interview = db.GetScheduledInterview(selected.ApplicationID);

            if (interview == null)
            {
                Console.WriteLine("❌ Interview details not found.");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     CONFIRM CANCELLATION                    ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.WriteLine($"Applicant: {selected.FirstName} {selected.LastName}");
            Console.WriteLine($"Position: {selected.JobTitle}");
            Console.WriteLine($"Scheduled: {((DateTime)interview.InterviewDate):MMMM dd, yyyy} {((DateTime)interview.InterviewTime):HH:mm}");

            string reason = InputValidator.GetValidatedInput("\nReason for cancellation: ", "Reason");

            bool success = db.CancelInterview((int)interview.ScheduleID, reason, hrUsername);

            if (success)
            {
                Console.WriteLine($"\n✅ Interview cancelled successfully!");
            }
            else
            {
                Console.WriteLine("\n❌ Failed to cancel interview.");
            }

            Console.ReadKey();
        }

        // ── Reschedule Interview ────────────────────────────
        private void RescheduleInterview(string hrUsername)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     RESCHEDULE INTERVIEW                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var scheduled = db.GetApplicationsByStatus(ApplicationStatus.InterviewScheduled);

            if (scheduled.Count == 0)
            {
                Console.WriteLine("No scheduled interviews available.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"#",-3} {"Name",-22} {"Job Title",-25} {"Applied",-12}");
            Console.WriteLine(new string('-', 65));

            for (int i = 0; i < scheduled.Count; i++)
            {
                var app = scheduled[i];
                string name = $"{app.FirstName} {app.LastName}";
                string jobTitle = ((string)app.JobTitle).Length > 24
                    ? ((string)app.JobTitle).Substring(0, 24)
                    : (string)app.JobTitle;
                string date = ((DateTime)app.DateApplied).ToString("yyyy-MM-dd");
                Console.WriteLine($"{i + 1,-3} {name,-22} {jobTitle,-25} {date,-12}");
            }

            Console.WriteLine($"\n{scheduled.Count + 1}. Back");
            Console.Write("\nSelect interview to reschedule: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice)
                || choice < 1 || choice > scheduled.Count)
            {
                return;
            }

            var selected = scheduled[choice - 1];
            var interview = db.GetScheduledInterview(selected.ApplicationID);

            if (interview == null)
            {
                Console.WriteLine("❌ Interview details not found.");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     RESCHEDULE INTERVIEW                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.WriteLine($"Applicant: {selected.FirstName} {selected.LastName}");
            Console.WriteLine($"Position: {selected.JobTitle}");
            Console.WriteLine($"Current: {((DateTime)interview.InterviewDate):MMMM dd, yyyy} {((DateTime)interview.InterviewTime):HH:mm}\n");

            // Get new date
            DateTime newDateTime = DateTime.Now;
            while (true)
            {
                Console.Write("Enter new interview date (yyyy-MM-dd): ");
                string dateInput = Console.ReadLine()?.Trim() ?? string.Empty;
                if (DateTime.TryParse(dateInput, out DateTime parsedDate))
                {
                    newDateTime = parsedDate;
                    break;
                }
                Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
            }

            // Get new time
            while (true)
            {
                Console.Write("Enter new interview time (HH:mm): ");
                string timeInput = Console.ReadLine()?.Trim() ?? string.Empty;
                if (DateTime.TryParse(timeInput, out DateTime parsedTime))
                {
                    newDateTime = newDateTime.Date.Add(parsedTime.TimeOfDay);
                    break;
                }
                Console.WriteLine("Invalid time format. Please use HH:mm.");
            }

            string location = InputValidator.GetValidatedInput("Enter interview location: ", "Location");

            bool success = db.RescheduleInterview((int)interview.ScheduleID, newDateTime, location, hrUsername);

            if (success)
            {
                Console.WriteLine($"\n✅ Interview rescheduled successfully!");
                Console.WriteLine($"New date: {newDateTime:MMMM dd, yyyy HH:mm}");
                Console.WriteLine($"Location: {location}");
            }
            else
            {
                Console.WriteLine("\n❌ Failed to reschedule interview.");
            }

            Console.ReadKey();
        }
    }
}
