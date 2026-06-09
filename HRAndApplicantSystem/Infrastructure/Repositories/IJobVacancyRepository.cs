using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Repository interface for job vacancy data access operations
    /// </summary>
    public interface IJobVacancyRepository
    {
        /// <summary>
        /// Gets all job vacancies
        /// </summary>
        List<JobVacancy> GetAllJobVacancies();

        /// <summary>
        /// Gets a specific job vacancy by ID
        /// </summary>
        JobVacancy GetJobVacancyByID(int jobID);

        /// <summary>
        /// Creates a new job vacancy
        /// </summary>
        bool CreateJobVacancy(JobVacancy job);

        /// <summary>
        /// Updates an existing job vacancy
        /// </summary>
        bool UpdateJobVacancy(JobVacancy job);

        /// <summary>
        /// Deletes a job vacancy
        /// </summary>
        bool DeleteJobVacancy(int jobID);

        /// <summary>
        /// Checks if an applicant has already applied for a job
        /// </summary>
        bool HasApplicantAppliedForJob(int applicantId, int jobID);
    }
}
