using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
        // ================================================================
        // DATA-RETURNING METHODS (for UI binding and external use)
        // ================================================================

        /// <summary>
        /// Validates if the user role has permission to access audit logs
        /// Only Admin (4) and HR Manager (3) can access audit logs
        /// </summary>
        public bool ValidateAuditLogAccess(int userRoleID)
        {
            return userRoleID == RoleConstants.ADMIN || userRoleID == RoleConstants.HR_MANAGER;
        }

        /// <summary>
        /// Gets all audit logs with optional row limit
        /// Converts to typed AuditTrail list for easy UI binding
        /// </summary>
        public List<AuditTrail> GetAllAuditLogs(int limit = 500)
        {
            var logs = db.GetAllAuditLogs(limit);
            return ConvertToAuditTrailList(logs);
        }

        /// <summary>
        /// Gets audit logs for a specific user by UserID
        /// </summary>
        public List<AuditTrail> GetAuditLogsByUser(int userID, int limit = 100)
        {
            var logs = db.GetAuditLogsByUserID(userID, limit);
            return ConvertToAuditTrailList(logs);
        }

        /// <summary>
        /// Gets audit logs filtered by user role/type
        /// Examples: "Applicant", "HR Staff", "HR Manager", "Administrator"
        /// </summary>
        public List<AuditTrail> GetAuditLogsByRole(string userType, int limit = 100)
        {
            if (string.IsNullOrWhiteSpace(userType))
                return new List<AuditTrail>();

            var logs = db.GetAuditLogsByUserType(userType, limit);
            return ConvertToAuditTrailList(logs);
        }

        /// <summary>
        /// Gets audit logs for a specific date
        /// </summary>
        public List<AuditTrail> GetAuditLogsByDate(DateTime date, int limit = 500)
        {
            var logs = db.GetAuditLogsByDateRange(date.Date, date.Date, limit);
            return ConvertToAuditTrailList(logs);
        }

        /// <summary>
        /// Gets the most recent audit logs
        /// </summary>
        public List<AuditTrail> GetRecentAuditLogs(int limit = 50)
        {
            var logs = db.GetAuditTrail(limit);
            return ConvertToAuditTrailList(logs);
        }

        /// <summary>
        /// Gets audit logs for a date range
        /// </summary>
        public List<AuditTrail> GetAuditLogsByDateRange(DateTime startDate, DateTime endDate, int limit = 500)
        {
            if (startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            var logs = db.GetAuditLogsByDateRange(startDate, endDate, limit);
            return ConvertToAuditTrailList(logs);
        }

        /// <summary>
        /// Gets audit logs filtered by action keyword
        /// </summary>
        public List<AuditTrail> GetAuditLogsByAction(string actionKeyword, int limit = 100)
        {
            if (string.IsNullOrWhiteSpace(actionKeyword))
                return new List<AuditTrail>();

            var allLogs = db.GetAuditTrail(limit * 5); // Get more logs to filter from
            var filtered = allLogs.Where(log => 
                log.Action != null && 
                log.Action.ToString().Contains(actionKeyword, StringComparison.OrdinalIgnoreCase))
                .Take(limit)
                .ToList();

            return ConvertToAuditTrailList(filtered);
        }

        /// <summary>
        /// Gets summary statistics about audit logs
        /// </summary>
        public dynamic GetAuditStatistics()
        {
            try
            {
                var allLogs = db.GetAuditTrail(5000);
                if (allLogs.Count == 0)
                    return null;

                var roleGroups = allLogs
                    .GroupBy(log => log.UserType)
                    .Select(g => new { UserType = g.Key, Count = g.Count() })
                    .ToList();

                var totalActions = allLogs.Count;
                var uniqueUsers = allLogs.Select(log => log.UserID).Distinct().Count();
                var dateRange = new
                {
                    Earliest = allLogs.Min(log => Convert.ToDateTime(log.ActionDate)),
                    Latest = allLogs.Max(log => Convert.ToDateTime(log.ActionDate))
                };

                return new
                {
                    TotalActions = totalActions,
                    UniqueUsers = uniqueUsers,
                    ActionsByRole = roleGroups,
                    DateRange = dateRange
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting audit statistics: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Helper method to convert dynamic audit logs to typed AuditTrail objects
        /// </summary>
        private List<AuditTrail> ConvertToAuditTrailList(List<dynamic> dynamicLogs)
        {
            var auditTrails = new List<AuditTrail>();

            try
            {
                if (dynamicLogs == null || dynamicLogs.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("ConvertToAuditTrailList: No logs to convert");
                    return auditTrails;
                }

                System.Diagnostics.Debug.WriteLine($"ConvertToAuditTrailList: Converting {dynamicLogs.Count} logs");

                foreach (var log in dynamicLogs)
                {
                    try
                    {
                        int auditID = log.AuditID;
                        string userType = log.UserType?.ToString() ?? "Unknown";
                        int userID = log.UserID ?? 0;
                        string username = log.Username?.ToString() ?? "Unknown";
                        string action = log.Action?.ToString() ?? "Unknown";
                        DateTime actionDate = Convert.ToDateTime(log.ActionDate);

                        auditTrails.Add(new AuditTrail(auditID, userType, userID, username, action, actionDate));
                    }
                    catch (Exception itemEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error converting individual audit log: {itemEx.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"ConvertToAuditTrailList: Successfully converted {auditTrails.Count} logs");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error converting audit logs: {ex.Message}");
            }

            return auditTrails;
        }    }
}
