using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;
using ApplicationModel = HRAndApplicantSystem.Models.Application;

namespace HRAndApplicantSystem.Services.Business
{
    /// <summary>
    /// Business logic for application management.
    /// Handles application status checks, data retrieval, and status messaging.
    /// No UI/Console rendering - returns data for presentation layer.
    /// </summary>
    public class ApplicationManagementBusinessService
    {
        private readonly IApplicationRepository applicationRepository;
        private readonly DatabaseHelper db;

        public ApplicationManagementBusinessService(IApplicationRepository appRepo = null)
        {
            applicationRepository = appRepo ?? new ApplicationRepository(new DatabaseHelper());
            db = new DatabaseHelper();
        }

        /// <summary>
        /// Get all applications for an applicant
        /// </summary>
        public List<dynamic> GetApplicantApplications(int applicantId)
        {
            return db.GetApplicantApplications(applicantId);
        }

        /// <summary>
        /// Get an application by ID with additional details
        /// </summary>
        public ApplicationModel GetApplicationDetails(int applicationId)
        {
            return db.GetApplicationByID(applicationId);
        }

        /// <summary>
        /// Get application status message for display
        /// </summary>
        public string GetStatusMessage(string status)
        {
            return status switch
            {
                ApplicationStatus.Draft => "This application is in Draft status.\nYou can manage documents and submit it whenever you're ready.",
                ApplicationStatus.Submitted => "Your application has been submitted and is pending initial review.\nWe'll notify you of any updates.",
                ApplicationStatus.UnderReview => "Your application is currently being actively reviewed by our HR team.\nPlease check back soon for updates.",
                ApplicationStatus.Shortlisted => "Great! You've been shortlisted for the position.\nWe'll contact you soon about next steps.",
                ApplicationStatus.InterviewScheduled => "Excellent! An interview has been scheduled for you.\nCheck your email for the date, time, and location.",
                ApplicationStatus.Accepted => "Congratulations! Your application has been accepted.\nPlease check your email for next steps.",
                ApplicationStatus.Rejected => "Unfortunately, we won't be moving forward with your application at this time.\nWe appreciate your interest and encourage you to apply for other positions.",
                _ => "Your application status: " + status
            };
        }

        /// <summary>
        /// Check if application is in draft status
        /// </summary>
        public bool IsDraft(string applicationStatus)
        {
            return applicationStatus == ApplicationStatus.Draft;
        }

        /// <summary>
        /// Check if application status is interview scheduled
        /// </summary>
        public bool IsInterviewScheduled(string applicationStatus)
        {
            return applicationStatus == ApplicationStatus.InterviewScheduled;
        }

        /// <summary>
        /// Check if application is editable (Draft or Submitted only)
        /// </summary>
        public bool IsEditable(string applicationStatus)
        {
            return applicationStatus == ApplicationStatus.Draft || applicationStatus == ApplicationStatus.Submitted;
        }

        /// <summary>
        /// Get interview schedule for an application
        /// </summary>
        public dynamic GetInterviewSchedule(int applicationId)
        {
            return db.GetInterviewSchedule(applicationId);
        }

        /// <summary>
        /// Get job vacancy details by ID
        /// </summary>
        public JobVacancy GetJobVacancy(int jobId)
        {
            return db.GetJobVacancyByID(jobId);
        }

        /// <summary>
        /// Get applicant details by ID
        /// </summary>
        public Applicant GetApplicant(int applicantId)
        {
            return db.GetApplicantByID(applicantId);
        }

        /// <summary>
        /// Delete an application
        /// </summary>
        public bool DeleteApplication(int applicationId)
        {
            return db.DeleteApplication(applicationId);
        }

        /// <summary>
        /// Get submitted applications count for applicant
        /// </summary>
        public int GetSubmittedApplicationCount(int applicantId)
        {
            var applications = db.GetApplicantApplications(applicantId);
            return applications.Count(a => 
                a.ApplicationStatus == ApplicationStatus.Submitted || 
                a.ApplicationStatus == ApplicationStatus.UnderReview);
        }
    }
}
