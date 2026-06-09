using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Services.Business
{
    /// <summary>
    /// Business logic for interview operations.
    /// Handles interview scheduling, evaluation, and data retrieval.
    /// No UI/Console rendering - returns data for presentation layer.
    /// </summary>
    public class InterviewBusinessService
    {
        private readonly IInterviewRepository interviewRepository;
        private readonly IApplicationRepository applicationRepository;
        private readonly DatabaseHelper db;

        public InterviewBusinessService(IInterviewRepository interRepo = null, IApplicationRepository appRepo = null)
        {
            interviewRepository = interRepo ?? new InterviewRepository(new DatabaseHelper());
            applicationRepository = appRepo ?? new ApplicationRepository(new DatabaseHelper());
            db = new DatabaseHelper();
        }

        /// <summary>
        /// Get shortlisted applications for interview
        /// </summary>
        public List<dynamic> GetShortlistedApplications()
        {
            return db.GetApplicationsByStatus(ApplicationStatus.Shortlisted);
        }

        /// <summary>
        /// Get applications scheduled for interview
        /// </summary>
        public List<dynamic> GetInterviewScheduledApplications()
        {
            return db.GetApplicationsByStatus(ApplicationStatus.InterviewScheduled);
        }

        /// <summary>
        /// Schedule interview for applicant
        /// </summary>
        public bool ScheduleInterview(int applicationId, DateTime interviewDateTime, 
            string interviewer, string mode, string location, string hrUsername)
        {
            return db.ScheduleInterview(applicationId, interviewDateTime, interviewer, mode, location, hrUsername);
        }

        /// <summary>
        /// Get interview schedule details
        /// </summary>
        public dynamic GetInterviewSchedule(int applicationId)
        {
            return db.GetInterviewSchedule(applicationId);
        }

        /// <summary>
        /// Update interview evaluation
        /// </summary>
        public bool EvaluateInterview(int applicationId, string interviewResult, int score, 
            string feedback, string hrUsername)
        {
            string newStatus = interviewResult == "Pass" ? ApplicationStatus.Accepted : ApplicationStatus.Rejected;
            return db.EvaluateInterview(applicationId, score, interviewResult, feedback, newStatus, hrUsername);
        }

        /// <summary>
        /// Cancel interview
        /// </summary>
        public bool CancelInterview(int applicationId, string reason, string hrUsername)
        {
            return db.CancelInterview(applicationId, reason, hrUsername);
        }

        /// <summary>
        /// Reschedule interview
        /// </summary>
        public bool RescheduleInterview(int applicationId, DateTime newDateTime, string hrUsername)
        {
            // Note: RescheduleInterview requires scheduleID. For now, using applicationID.
            // In production, would need to fetch the interview schedule ID first.
            return db.RescheduleInterview(applicationId, newDateTime, "", hrUsername);
        }

        /// <summary>
        /// Validate score input
        /// </summary>
        public bool IsValidScore(int score)
        {
            return score >= 0 && score <= 100;
        }

        /// <summary>
        /// Get interview result message
        /// </summary>
        public string GetInterviewResultMessage(string result)
        {
            return result switch
            {
                "Pass" => "Interview Passed - Candidate Qualified",
                "Fail" => "Interview Failed - Candidate Not Qualified",
                _ => result
            };
        }

        /// <summary>
        /// Get mode display value
        /// </summary>
        public string GetModeDisplayValue(string mode)
        {
            return mode == "Online" ? "Online (Video Call)" : "Face-to-Face";
        }
    }
}
