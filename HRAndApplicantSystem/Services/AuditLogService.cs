using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using System;
using System.Collections.Generic;

namespace HRAndApplicantSystem.Services
{
    public class AuditLogService
    {
        private readonly IAuditRepository auditRepository;
        private readonly DatabaseHelper db;

        public AuditLogService(IAuditRepository auditRepo = null)
        {
            auditRepository = auditRepo ?? new AuditRepository(new DatabaseHelper());
            db = new DatabaseHelper();
        }

        public void ViewAuditLogs()
        {
            bool viewing = true;

            while (viewing)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     AUDIT LOG VIEWER                         ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                Console.WriteLine("Select number of records to view:");
                Console.WriteLine("1. Last 10 Actions");
                Console.WriteLine("2. Last 25 Actions");
                Console.WriteLine("3. Last 50 Actions");
                Console.WriteLine("4. Last 100 Actions");
                Console.WriteLine("5. Back\n");

                Console.Write("Choose an option: ");
                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                int limit = choice switch
                {
                    "1" => 10,
                    "2" => 25,
                    "3" => 50,
                    "4" => 100,
                    "5" => 0,
                    _ => -1
                };

                if (limit == 0)
                {
                    viewing = false;
                }
                else if (limit > 0)
                {
                    DisplayLogs(limit);
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");
                    System.Threading.Thread.Sleep(1500);
                }
            }
        }

        private void DisplayLogs(int limit)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     SYSTEM AUDIT TRAIL                       ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var logs = db.GetAuditTrail(limit);

            if (logs.Count == 0)
            {
                Console.WriteLine("No audit logs found.\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"#",-4} {"Date & Time",-25} {"User (Role)",-30} {"Action",-55}");
            Console.WriteLine(new string('=', 114));

            for (int i = 0; i < logs.Count; i++)
            {
                var log = logs[i];
                string dateTime = Convert.ToDateTime(log.ActionDate).ToString("yyyy-MM-dd HH:mm:ss");
                string username = log.Username?.ToString() ?? "Unknown";
                string userType = log.UserType?.ToString() ?? "Unknown";
                string userDisplay = $"{username} ({userType})";
                string action = log.Action?.ToString() ?? "Unknown";

                // Truncate action if too long
                if (action.Length > 54)
                {
                    action = action.Substring(0, 51) + "...";
                }

                Console.WriteLine($"{i + 1,-4} {dateTime,-25} {userDisplay,-30} {action,-55}");
            }

            Console.WriteLine(new string('=', 114));
            Console.WriteLine($"\nTotal Records: {logs.Count}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public void ExportAuditLog()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     EXPORT AUDIT LOG                         ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                var logs = db.GetAuditTrail(500);  // Export last 500 records

                if (logs.Count == 0)
                {
                    Console.WriteLine("No audit logs to export.");
                    System.Threading.Thread.Sleep(1500);
                    return;
                }

                string fileName = $"AuditLog_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write header
                    writer.WriteLine("AuditID,Date,Username,Role,Action");

                    // Write data
                    foreach (var log in logs)
                    {
                        string dateTime = Convert.ToDateTime(log.ActionDate).ToString("yyyy-MM-dd HH:mm:ss");
                        string username = log.Username?.ToString() ?? "Unknown";
                        string userType = log.UserType?.ToString() ?? "Unknown";
                        writer.WriteLine($"\"{log.AuditID}\",\"{dateTime}\",\"{username}\",\"{userType}\",\"{log.Action.Replace("\"", "\"\"") }\"");
                    }
                }

                Console.WriteLine($"✓ Audit log exported successfully!");
                Console.WriteLine($"File: {fileName}");
                Console.WriteLine($"Records: {logs.Count}");
                System.Threading.Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error exporting audit log: {ex.Message}");
                System.Threading.Thread.Sleep(2000);
            }
        }
    }
}
