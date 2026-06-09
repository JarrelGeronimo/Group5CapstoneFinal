using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Services.Business
{
    /// <summary>
    /// Business logic for the applicant dashboard summary.
    /// Handles data retrieval, calculations, and business rules.
    /// No UI/Console rendering - returns data for presentation layer.
    /// </summary>
    public class DashboardSummaryBusinessService
    {
        private readonly IApplicationRepository applicationRepository;
        private readonly DatabaseHelper db;

        public DashboardSummaryBusinessService(IApplicationRepository appRepo = null)
        {
            applicationRepository = appRepo ?? new ApplicationRepository(new DatabaseHelper());
            db = new DatabaseHelper();
        }

        /// <summary>
        /// Get summary data for applicant dashboard
        /// </summary>
        public DashboardSummaryData GetDashboardData(int applicantId, string username)
        {
            var applications = db.GetApplicantApplications(applicantId);
            
            var summary = new DashboardSummaryData
            {
                ApplicantId = applicantId,
                Username = username,
                TotalApplications = applications.Count,
                Applications = applications,
                PendingCount = GetPendingApplicationCount(applications),
                AcceptedCount = GetAcceptedApplicationCount(applications),
                RecentApplications = GetRecentApplications(applications, 3),
                HasPendingReview = HasPendingReview(applications),
                HasAccepted = HasAccepted(applications),
                HasInterviewScheduled = HasInterviewScheduled(applications)
            };

            return summary;
        }

        /// <summary>
        /// Get count of pending applications (Submitted or Under Review)
        /// </summary>
        public int GetPendingApplicationCount(List<dynamic> applications)
        {
            return applications.Count(a => 
                a.ApplicationStatus == ApplicationStatus.Submitted || 
                a.ApplicationStatus == ApplicationStatus.UnderReview);
        }

        /// <summary>
        /// Get count of accepted applications
        /// </summary>
        public int GetAcceptedApplicationCount(List<dynamic> applications)
        {
            return applications.Count(a => a.ApplicationStatus == ApplicationStatus.Accepted);
        }

        /// <summary>
        /// Get the most recent N applications
        /// </summary>
        public List<dynamic> GetRecentApplications(List<dynamic> applications, int count)
        {
            return applications
                .OrderByDescending(a => ((DateTime)a.DateApplied))
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Check if applicant has any pending review applications
        /// </summary>
        public bool HasPendingReview(List<dynamic> applications)
        {
            return applications.Any(a => 
                a.ApplicationStatus == ApplicationStatus.Submitted || 
                a.ApplicationStatus == ApplicationStatus.UnderReview);
        }

        /// <summary>
        /// Check if applicant has any accepted applications
        /// </summary>
        public bool HasAccepted(List<dynamic> applications)
        {
            return applications.Any(a => a.ApplicationStatus == ApplicationStatus.Accepted);
        }

        /// <summary>
        /// Check if applicant has any scheduled interviews
        /// </summary>
        public bool HasInterviewScheduled(List<dynamic> applications)
        {
            return applications.Any(a => a.ApplicationStatus == ApplicationStatus.InterviewScheduled);
        }

        /// <summary>
        /// Get next action recommendations based on application statuses
        /// </summary>
        public List<string> GetNextSteps(List<dynamic> applications)
        {
            var nextSteps = new List<string>();

            if (HasPendingReview(applications))
                nextSteps.Add("Review status updates for submitted applications");

            if (HasAccepted(applications))
                nextSteps.Add("Check email for accepted offer details");

            if (HasInterviewScheduled(applications))
                nextSteps.Add("Prepare for scheduled interviews");

            if (GetAcceptedApplicationCount(applications) == 0 && GetPendingApplicationCount(applications) == 0)
                nextSteps.Add("Browse and apply for more job vacancies");

            return nextSteps;
        }
    }

    /// <summary>
    /// Data transfer object for dashboard summary information
    /// </summary>
    public class DashboardSummaryData
    {
        public int ApplicantId { get; set; }
        public string Username { get; set; }
        public int TotalApplications { get; set; }
        public List<dynamic> Applications { get; set; }
        public int PendingCount { get; set; }
        public int AcceptedCount { get; set; }
        public List<dynamic> RecentApplications { get; set; }
        public bool HasPendingReview { get; set; }
        public bool HasAccepted { get; set; }
        public bool HasInterviewScheduled { get; set; }
    }
}
