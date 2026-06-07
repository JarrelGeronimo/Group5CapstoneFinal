namespace HRAndApplicantSystem.Models
{
    /// <summary>
    /// User roles in the HR and Applicant System
    /// </summary>
    public enum UserRole
    {
        Applicant = 1,
        HR = 2,
        HRManager = 3,
        Admin = 4
    }

    /// <summary>
    /// Role constants and utilities
    /// </summary>
    public static class RoleConstants
    {
        public const int APPLICANT = 1;
        public const int HR = 2;
        public const int HR_MANAGER = 3;
        public const int ADMIN = 4;

        /// <summary>
        /// Get the display name for a role ID
        /// </summary>
        public static string GetRoleName(int roleId)
        {
            return roleId switch
            {
                APPLICANT => "Applicant",
                HR => "HR Staff",
                HR_MANAGER => "HR Manager",
                ADMIN => "Administrator",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Check if a role ID is valid
        /// </summary>
        public static bool IsValidRole(int roleId)
        {
            return roleId == APPLICANT || roleId == HR || roleId == HR_MANAGER || roleId == ADMIN;
        }
    }
}
