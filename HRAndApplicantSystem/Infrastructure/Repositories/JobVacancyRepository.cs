using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Job vacancy repository implementation - Adaptor pattern wrapping DatabaseHelper
    /// </summary>
    public class JobVacancyRepository : IJobVacancyRepository
    {
        private readonly DatabaseHelper db;

        public JobVacancyRepository(DatabaseHelper databaseHelper = null)
        {
            db = databaseHelper ?? new DatabaseHelper();
        }

        public List<JobVacancy> GetAllJobVacancies()
        {
            return db.GetAllJobVacancies();
        }

        public JobVacancy GetJobVacancyByID(int jobID)
        {
            return db.GetJobVacancyByID(jobID);
        }

        public bool CreateJobVacancy(JobVacancy job)
        {
            return db.CreateJobVacancy(job);
        }

        public bool UpdateJobVacancy(JobVacancy job)
        {
            return db.UpdateJobVacancy(job);
        }

        public bool DeleteJobVacancy(int jobID)
        {
            return db.DeleteJobVacancy(jobID);
        }

        public bool HasApplicantAppliedForJob(int applicantId, int jobID)
        {
            return db.HasApplicantAppliedForJob(applicantId, jobID);
        }
    }
}
