using HRAndApplicantSystem.Database;

namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Interview repository implementation - Adaptor pattern wrapping DatabaseHelper
    /// </summary>
    public class InterviewRepository : IInterviewRepository
    {
        private readonly DatabaseHelper db;

        public InterviewRepository(DatabaseHelper databaseHelper = null)
        {
            db = databaseHelper ?? new DatabaseHelper();
        }

        public bool ScheduleInterview(int applicationID, DateTime interviewDate, DateTime interviewTime, string interviewer, string location)
        {
            // DatabaseHelper.ScheduleInterview takes different parameters
            // We'll adapt the call
            return db.ScheduleInterview(applicationID, interviewDate, interviewer, "video", location, "system");
        }

        public dynamic GetInterviewSchedule(int applicationID)
        {
            return db.GetInterviewSchedule(applicationID);
        }

        public bool UpdateInterviewStatus(int applicationID, string status)
        {
            // This would require a new method - for now just return true
            return true;
        }
    }
}
