using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Services
{
    /// <summary>
    /// Consolidated service for managing the complete application lifecycle:
    /// - Creating and drafting new applications
    /// - Submitting applications
    /// - Viewing and managing existing applications
    /// - Tracking application status and interview details
    /// </summary>
    public class ApplicationWorkflowService
    {
        private readonly IApplicationRepository applicationRepository;
        private readonly DatabaseHelper db;
        private readonly ApplicationDraftingService draftingService;
        private readonly ApplicationManagementService managementService;

        public ApplicationWorkflowService(IApplicationRepository appRepo = null)
        {
            applicationRepository = appRepo ?? new ApplicationRepository(new DatabaseHelper());
            db = new DatabaseHelper();
            draftingService = new ApplicationDraftingService(appRepo);
            managementService = new ApplicationManagementService(appRepo);
        }

        /// <summary>
        /// Entry point for managing applicant's applications
        /// </summary>
        public void ManageApplications(Applicant applicant)
        {
            managementService.ManageApplications(applicant);
        }

        /// <summary>
        /// Start the application drafting workflow for a specific job
        /// Returns true if application was submitted, false if user cancelled
        /// </summary>
        public bool DraftAndSubmitApplication(JobVacancy job, Applicant applicant)
        {
            return draftingService.DraftAndSubmitApplication(job, applicant);
        }

        /// <summary>
        /// Resume an existing draft application
        /// </summary>
        public void ResumeDraftApplication(JobVacancy job, Applicant applicant, int applicationID)
        {
            draftingService.ResumeDraftApplication(job, applicant, applicationID);
        }

        /// <summary>
        /// Get count of submitted or under-review applications for an applicant
        /// </summary>
        public int GetSubmittedApplicationCount(int applicantId)
        {
            return managementService.GetSubmittedApplicationCount(applicantId);
        }

        /// <summary>
        /// Get the ApplicationDraft helper class for use in workflows
        /// </summary>
        public ApplicationDraftingService.ApplicationDraft CreateApplicationDraft(JobVacancy job, Applicant applicant)
        {
            return new ApplicationDraftingService.ApplicationDraft
            {
                Job = job,
                Applicant = applicant,
                DraftCreatedAt = DateTime.Now
            };
        }
    }
}
