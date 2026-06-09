using System;
using System.Collections.Generic;
using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Services
{
    public class ApplicantApplicationHistoryService
    {
        private readonly IApplicationRepository applicationRepository;
        private readonly DatabaseHelper db;

        public ApplicantApplicationHistoryService(IApplicationRepository appRepo = null)
        {
            applicationRepository = appRepo ?? new ApplicationRepository(new DatabaseHelper());
            db = new DatabaseHelper();
        }

        /// <summary>
        /// Entry point: shows the applicant's list of applications,
        /// then lets them drill into the full status timeline for any one.
        /// </summary>
        public void ShowMyApplications(string username)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║   MY APPLICATION HISTORY                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            // Resolve applicant
            var applicant = db.GetApplicantByUsername(username);
            if (applicant == null)
            {
                Console.WriteLine("Error: Could not load your applicant profile.");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            var applications = db.GetApplicantApplications(applicant.ApplicantID);

            if (applications.Count == 0)
            {
                Console.WriteLine("You have not submitted any applications yet.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            // List applications
            Console.WriteLine($"{"#",-4} {"Job Title",-30} {"Status",-22} {"Date Applied",-13}");
            Console.WriteLine(new string('─', 72));

            for (int i = 0; i < applications.Count; i++)
            {
                var app = applications[i];
                string jobTitle   = TruncateString((string)app.JobTitle, 29);
                string status     = TruncateString((string)app.ApplicationStatus, 21);
                string dateApplied = ((DateTime)app.DateApplied).ToString("yyyy-MM-dd");

                Console.WriteLine($"{i + 1,-4} {jobTitle,-30} {status,-22} {dateApplied,-13}");
            }

            Console.WriteLine($"\n{applications.Count + 1}. Back");
            Console.Write("\nSelect an application to view its full timeline: ");

            if (int.TryParse(Console.ReadLine()?.Trim(), out int choice)
                && choice >= 1 && choice <= applications.Count)
            {
                var selected = applications[choice - 1];
                ShowApplicationTimeline(selected.ApplicationID, selected.JobTitle);
            }
            // any other input (or "back") just returns
        }

        /// <summary>
        /// Displays the full chronological status timeline for one application.
        /// </summary>
        private void ShowApplicationTimeline(int applicationID, string jobTitle)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║   APPLICATION TIMELINE                       ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.WriteLine($"  Job Position : {jobTitle}");
            Console.WriteLine($"  Application  : #{applicationID}");
            Console.WriteLine();

            var history = db.GetApplicationStatusHistory(applicationID);

            if (history.Count == 0)
            {
                Console.WriteLine("  No status history found for this application.\n");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            // ── Timeline rendering ────────────────────────────────────────
            for (int i = 0; i < history.Count; i++)
            {
                var entry      = history[i];
                bool isLast    = i == history.Count - 1;
                bool isFirst   = i == 0;

                string status     = (string)entry.Status;
                string remarks    = (string)entry.Remarks;
                string changedBy  = (string)entry.ChangedBy;
                string dateStr    = ((DateTime)entry.DateChanged).ToString("MMM dd, yyyy  HH:mm");

                // ── connector line above node (skip for first)
                if (!isFirst)
                    Console.WriteLine("  │");

                // ── node bullet
                string bullet = isLast ? "  ●" : "  ○";
                Console.ForegroundColor = isLast ? ConsoleColor.Green : ConsoleColor.DarkGray;
                Console.Write(bullet + " ");
                Console.ResetColor();

                // ── status label (colour-coded)
                Console.ForegroundColor = StatusColor(status);
                Console.Write($"[{status}]");
                Console.ResetColor();

                Console.WriteLine($"  {dateStr}");

                // ── details indented under the node
                string indent = isLast ? "     " : "  │  ";
                if (!string.IsNullOrWhiteSpace(remarks))
                    Console.WriteLine($"{indent}Remarks  : {remarks}");

                if (!string.IsNullOrWhiteSpace(changedBy))
                    Console.WriteLine($"{indent}By       : {changedBy}");
            }

            Console.WriteLine();
            Console.WriteLine("  (End of timeline)");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // ── helpers ──────────────────────────────────────────────────────

        private static string TruncateString(string s, int maxLen)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Length <= maxLen ? s : s.Substring(0, maxLen - 1) + "…";
        }

        private static ConsoleColor StatusColor(string status) => status switch
        {
            ApplicationStatus.Submitted          => ConsoleColor.Cyan,
            ApplicationStatus.UnderReview        => ConsoleColor.Yellow,
            ApplicationStatus.Shortlisted        => ConsoleColor.Blue,
            ApplicationStatus.InterviewScheduled => ConsoleColor.Magenta,
            ApplicationStatus.Accepted           => ConsoleColor.Green,
            ApplicationStatus.Rejected           => ConsoleColor.Red,
            _                                    => ConsoleColor.White
        };
    }
}
