using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Services.Business
{
    /// <summary>
    /// Business logic for HR dashboard operations.
    /// Handles application filtering, retrieval, and status checks.
    /// No UI/Console rendering - returns data for presentation layer.
    /// </summary>
    public class HRDashboardBusinessService
    {
        private readonly IApplicationRepository applicationRepository;
        private readonly DatabaseHelper db;

        public HRDashboardBusinessService(IApplicationRepository appRepo = null)
        {
            applicationRepository = appRepo ?? new ApplicationRepository(new DatabaseHelper());
            db = new DatabaseHelper();
        }

        /// <summary>
        /// Get pending applications for screening
        /// </summary>
        public List<dynamic> GetPendingApplicationsForScreening()
        {
            return db.GetPendingApplicationsForScreening();
        }

        /// <summary>
        /// Get applications by status
        /// </summary>
        public List<dynamic> GetApplicationsByStatus(string status)
        {
            return db.GetApplicationsByStatus(status);
        }

        /// <summary>
        /// Get applications by job
        /// </summary>
        public List<dynamic> GetApplicationsByJob(int jobId)
        {
            return db.GetApplicationsByJob(jobId);
        }

        /// <summary>
        /// Get all job vacancies
        /// </summary>
        public List<dynamic> GetAllJobVacancies()
        {
            var jobs = db.GetAllJobVacancies();
            return jobs.Cast<dynamic>().ToList();
        }

        /// <summary>
        /// Get user role by username
        /// </summary>
        public int GetUserRoleByUsername(string username)
        {
            return db.GetUserRoleByUsername(username);
        }

        /// <summary>
        /// Get status summary counts
        /// </summary>
        public ApplicationStatusSummary GetApplicationStatusSummary()
        {
            var submitted = db.GetApplicationsByStatus(ApplicationStatus.Submitted);
            var underReview = db.GetApplicationsByStatus(ApplicationStatus.UnderReview);
            var shortlisted = db.GetApplicationsByStatus(ApplicationStatus.Shortlisted);
            var interviewScheduled = db.GetApplicationsByStatus(ApplicationStatus.InterviewScheduled);
            var accepted = db.GetApplicationsByStatus(ApplicationStatus.Accepted);
            var rejected = db.GetApplicationsByStatus(ApplicationStatus.Rejected);

            return new ApplicationStatusSummary
            {
                SubmittedCount = submitted.Count,
                UnderReviewCount = underReview.Count,
                ShortlistedCount = shortlisted.Count,
                InterviewScheduledCount = interviewScheduled.Count,
                AcceptedCount = accepted.Count,
                RejectedCount = rejected.Count,
                TotalApplications = submitted.Count + underReview.Count + shortlisted.Count + 
                                   interviewScheduled.Count + accepted.Count + rejected.Count
            };
        }

        /// <summary>
        /// Check if user is manager or admin
        /// </summary>
        public bool IsManagerOrAdmin(int roleId)
        {
            return roleId == RoleConstants.HR_MANAGER || roleId == RoleConstants.ADMIN;
        }

        /// <summary>
        /// Get role display name
        /// </summary>
        public string GetRoleDisplayName(int roleId)
        {
            return roleId == RoleConstants.HR_MANAGER ? "HR Manager"
                : roleId == RoleConstants.ADMIN ? "Admin"
                : "HR Staff";
        }
    }

    /// <summary>
    /// Data transfer object for application status summary
    /// </summary>
    public class ApplicationStatusSummary
    {
        public int SubmittedCount { get; set; }
        public int UnderReviewCount { get; set; }
        public int ShortlistedCount { get; set; }
        public int InterviewScheduledCount { get; set; }
        public int AcceptedCount { get; set; }
        public int RejectedCount { get; set; }
        public int TotalApplications { get; set; }
    }
}
