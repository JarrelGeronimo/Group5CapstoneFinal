using HRAndApplicantSystem.Database;

namespace HRAndApplicantSystem.Services
{
    public class InterviewService
    {
        private readonly DatabaseHelper db;

        public InterviewService()
        {
            db = new DatabaseHelper();
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
                Console.WriteLine("3. Back to Dashboard");
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

            var shortlisted = db.GetApplicationsByStatus("Shortlisted");

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
                Console.WriteLine($"  Status updated to: For Interview");
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

            var forInterview = db.GetApplicationsByStatus("For Interview");

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
            string newStatus = recChoice == "1" ? "For Final Review" : "Rejected";

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
    }
}
