using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Utilities
{
    /// <summary>
    /// Utility class for role-based access control and permission validation
    /// </summary>
    public static class RoleValidator
    {
        private static readonly DatabaseHelper db = new DatabaseHelper();

        /// <summary>
        /// Check if user has a specific role
        /// </summary>
        public static bool HasRole(string username, int requiredRole)
        {
            int userRole = db.GetUserRoleByUsername(username);
            return userRole == requiredRole;
        }

        /// <summary>
        /// Check if user has any of the specified roles
        /// </summary>
        public static bool HasAnyRole(string username, params int[] requiredRoles)
        {
            int userRole = db.GetUserRoleByUsername(username);
            return requiredRoles.Contains(userRole);
        }

        /// <summary>
        /// Check if user is an applicant
        /// </summary>
        public static bool IsApplicant(string username)
        {
            return HasRole(username, RoleConstants.APPLICANT);
        }

        /// <summary>
        /// Check if user is HR staff or higher
        /// </summary>
        public static bool IsHRStaffOrHigher(string username)
        {
            return HasAnyRole(username, RoleConstants.HR, RoleConstants.HR_MANAGER, RoleConstants.ADMIN);
        }

        /// <summary>
        /// Check if user is HR manager or admin
        /// </summary>
        public static bool IsHRManagerOrAdmin(string username)
        {
            return HasAnyRole(username, RoleConstants.HR_MANAGER, RoleConstants.ADMIN);
        }

        /// <summary>
        /// Check if user is admin
        /// </summary>
        public static bool IsAdmin(string username)
        {
            return HasRole(username, RoleConstants.ADMIN);
        }

        /// <summary>
        /// Validate permission for an action
        /// </summary>
        public static bool CanPerformAction(string username, string actionName)
        {
            int userRole = db.GetUserRoleByUsername(username);

            // Define permissions based on role and action
            return (userRole, actionName.ToLower()) switch
            {
                // Applicant permissions
                (RoleConstants.APPLICANT, "view_own_profile") => true,
                (RoleConstants.APPLICANT, "edit_own_profile") => true,
                (RoleConstants.APPLICANT, "browse_jobs") => true,
                (RoleConstants.APPLICANT, "apply_job") => true,
                (RoleConstants.APPLICANT, "view_own_applications") => true,
                (RoleConstants.APPLICANT, "submit_documents") => true,
                (RoleConstants.APPLICANT, "view_own_interviews") => true,

                // HR Staff permissions
                (RoleConstants.HR, "view_all_applications") => true,
                (RoleConstants.HR, "screen_application") => true,
                (RoleConstants.HR, "schedule_interview") => true,
                (RoleConstants.HR, "evaluate_interview") => true,
                (RoleConstants.HR, "view_applicants") => true,
                (RoleConstants.HR, "view_reports") => false, // HR can't view full reports

                // HR Manager permissions
                (RoleConstants.HR_MANAGER, "view_all_applications") => true,
                (RoleConstants.HR_MANAGER, "screen_application") => true,
                (RoleConstants.HR_MANAGER, "schedule_interview") => true,
                (RoleConstants.HR_MANAGER, "evaluate_interview") => true,
                (RoleConstants.HR_MANAGER, "make_hiring_decision") => true,
                (RoleConstants.HR_MANAGER, "view_applicants") => true,
                (RoleConstants.HR_MANAGER, "view_reports") => true,
                (RoleConstants.HR_MANAGER, "manage_users") => false,

                // Admin permissions
                (RoleConstants.ADMIN, "view_all_applications") => true,
                (RoleConstants.ADMIN, "screen_application") => true,
                (RoleConstants.ADMIN, "schedule_interview") => true,
                (RoleConstants.ADMIN, "evaluate_interview") => true,
                (RoleConstants.ADMIN, "make_hiring_decision") => true,
                (RoleConstants.ADMIN, "view_applicants") => true,
                (RoleConstants.ADMIN, "view_reports") => true,
                (RoleConstants.ADMIN, "manage_users") => true,
                (RoleConstants.ADMIN, "manage_requirements") => true,
                (RoleConstants.ADMIN, "view_audit_trail") => true,

                _ => false
            };
        }

        /// <summary>
        /// Display permission denied message
        /// </summary>
        public static void DisplayPermissionDenied(string username, string actionName)
        {
            int userRole = db.GetUserRoleByUsername(username);
            string roleName = RoleConstants.GetRoleName(userRole);
            
            Console.WriteLine("\n╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     ACCESS DENIED                            ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");
            Console.WriteLine($"Your role '{roleName}' does not have permission");
            Console.WriteLine($"to perform this action: '{actionName}'\n");
            Console.WriteLine("Contact your system administrator if you believe this is an error.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Check and display permission if user lacks access
        /// </summary>
        public static bool EnsurePermission(string username, string actionName)
        {
            if (!CanPerformAction(username, actionName))
            {
                DisplayPermissionDenied(username, actionName);
                return false;
            }
            return true;
        }
    }
}
