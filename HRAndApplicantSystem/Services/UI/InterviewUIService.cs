using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Services.Business;

namespace HRAndApplicantSystem.Services.UI
{
    /// <summary>
    /// UI rendering for interview operations.
    /// Handles all console output and user interaction.
    /// Uses InterviewBusinessService for business logic.
    /// </summary>
    public class InterviewUIService
    {
        private readonly InterviewBusinessService businessService;

        public InterviewUIService(InterviewBusinessService businessService = null)
        {
            this.businessService = businessService ?? new InterviewBusinessService();
        }

        /// <summary>
        /// Display interview management menu
        /// </summary>
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
                        ScheduleInterviewUI(hrUsername);
                        break;
                    case "2":
                        EvaluateInterviewUI(hrUsername);
                        break;
                    case "3":
                        CancelInterviewUI(hrUsername);
                        break;
                    case "4":
                        RescheduleInterviewUI(hrUsername);
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

        private void ScheduleInterviewUI(string hrUsername)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     SCHEDULE INTERVIEW                       ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var shortlisted = businessService.GetShortlistedApplications();

            if (shortlisted.Count == 0)
            {
                Console.WriteLine("No shortlisted applicants available for interview.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            DisplayShortlistedApplicants(shortlisted);

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

            // Get interview details
            DateTime interviewDate = GetValidInterviewDate();
            TimeSpan interviewTime = GetValidInterviewTime();
            string mode = GetInterviewMode();
            string location = GetInterviewLocation(mode);
            string interviewer = GetInterviewerName(hrUsername);

            // Schedule interview
            DateTime fullDateTime = interviewDate.Date + interviewTime;
            bool success = businessService.ScheduleInterview(
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
                Console.WriteLine($"  Mode     : {businessService.GetModeDisplayValue(mode)}");
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

        private void EvaluateInterviewUI(string hrUsername)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     EVALUATE INTERVIEW                       ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var forInterview = businessService.GetInterviewScheduledApplications();

            if (forInterview.Count == 0)
            {
                Console.WriteLine("No applicants currently scheduled for interview.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            DisplayInterviewApplicants(forInterview);

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

            // Get evaluation details
            string interviewResult = GetInterviewResult();
            int score = GetInterviewScore();
            string feedback = GetInterviewFeedback();

            // Update evaluation
            bool success = businessService.EvaluateInterview(
                selected.ApplicationID,
                interviewResult,
                score,
                feedback,
                hrUsername
            );

            if (success)
            {
                Console.WriteLine($"\n✓ Interview evaluation recorded!");
                Console.WriteLine($"  Result: {businessService.GetInterviewResultMessage(interviewResult)}");
                Console.WriteLine($"  Score : {score}/100");
                Console.WriteLine($"  Status updated accordingly");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to record evaluation. Please try again.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void CancelInterviewUI(string hrUsername)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     CANCEL INTERVIEW                         ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var scheduled = businessService.GetInterviewScheduledApplications();

            if (scheduled.Count == 0)
            {
                Console.WriteLine("No scheduled interviews to cancel.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            DisplayInterviewApplicants(scheduled);

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice)
                || choice < 1 || choice > scheduled.Count)
            {
                Console.WriteLine("Returning...");
                System.Threading.Thread.Sleep(1000);
                return;
            }

            var selected = scheduled[choice - 1];
            Console.Write("\nReason for cancellation: ");
            string reason = Console.ReadLine()?.Trim() ?? "No reason provided";

            bool success = businessService.CancelInterview(selected.ApplicationID, reason, hrUsername);

            if (success)
            {
                Console.WriteLine("\n✓ Interview cancelled successfully.");
                Console.WriteLine("Applicant will be notified of the cancellation.");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to cancel interview.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void RescheduleInterviewUI(string hrUsername)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     RESCHEDULE INTERVIEW                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var scheduled = businessService.GetInterviewScheduledApplications();

            if (scheduled.Count == 0)
            {
                Console.WriteLine("No scheduled interviews to reschedule.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            DisplayInterviewApplicants(scheduled);

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice)
                || choice < 1 || choice > scheduled.Count)
            {
                Console.WriteLine("Returning...");
                System.Threading.Thread.Sleep(1000);
                return;
            }

            var selected = scheduled[choice - 1];

            DateTime newDateTime = GetValidInterviewDate();
            TimeSpan newTime = GetValidInterviewTime();
            DateTime combined = newDateTime.Date + newTime;

            bool success = businessService.RescheduleInterview(selected.ApplicationID, combined, hrUsername);

            if (success)
            {
                Console.WriteLine("\n✓ Interview rescheduled successfully!");
                Console.WriteLine($"  New Date & Time: {combined:MMMM dd, yyyy HH:mm}");
                Console.WriteLine("Applicant will be notified of the new date.");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to reschedule interview.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void DisplayShortlistedApplicants(List<dynamic> shortlisted)
        {
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
        }

        private void DisplayInterviewApplicants(List<dynamic> applicants)
        {
            Console.WriteLine($"{"#",-3} {"Name",-22} {"Job Title",-25} {"Applied",-12}");
            Console.WriteLine(new string('-', 65));

            for (int i = 0; i < applicants.Count; i++)
            {
                var app = applicants[i];
                string name = $"{app.FirstName} {app.LastName}";
                string jobTitle = ((string)app.JobTitle).Length > 24
                    ? ((string)app.JobTitle).Substring(0, 24)
                    : (string)app.JobTitle;
                string date = ((DateTime)app.DateApplied).ToString("yyyy-MM-dd");
                Console.WriteLine($"{i + 1,-3} {name,-22} {jobTitle,-25} {date,-12}");
            }

            Console.WriteLine($"\n{applicants.Count + 1}. Back");
            Console.Write("\nSelect applicant: ");
        }

        private DateTime GetValidInterviewDate()
        {
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
            return interviewDate;
        }

        private TimeSpan GetValidInterviewTime()
        {
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
            return interviewTime;
        }

        private string GetInterviewMode()
        {
            Console.WriteLine("\nInterview Mode:");
            Console.WriteLine("1. Face-to-Face");
            Console.WriteLine("2. Online (Video Call)");
            Console.Write("Select mode: ");
            string modeChoice = Console.ReadLine()?.Trim() ?? "1";
            return modeChoice == "2" ? "Online" : "Face-to-Face";
        }

        private string GetInterviewLocation(string mode)
        {
            Console.Write(mode == "Online"
                ? "Meeting Link/Platform: "
                : "Location/Venue: ");
            return Console.ReadLine()?.Trim() ?? "";
        }

        private string GetInterviewerName(string hrUsername)
        {
            Console.Write("Interviewer Name: ");
            return Console.ReadLine()?.Trim() ?? hrUsername;
        }

        private string GetInterviewResult()
        {
            Console.WriteLine("\nInterview Result:");
            Console.WriteLine("1. Pass");
            Console.WriteLine("2. Fail");
            Console.Write("Select result: ");
            string choice = Console.ReadLine()?.Trim() ?? "1";
            return choice == "1" ? "Pass" : "Fail";
        }

        private int GetInterviewScore()
        {
            int score = -1;
            while (score < 0 || score > 100)
            {
                Console.Write("Interview Score (0-100): ");
                if (int.TryParse(Console.ReadLine()?.Trim(), out int input) && input >= 0 && input <= 100)
                {
                    score = input;
                }
                else
                {
                    Console.WriteLine("✗ Please enter a score between 0 and 100.");
                }
            }
            return score;
        }

        private string GetInterviewFeedback()
        {
            Console.Write("Feedback/Comments: ");
            return Console.ReadLine()?.Trim() ?? "";
        }
    }
}
