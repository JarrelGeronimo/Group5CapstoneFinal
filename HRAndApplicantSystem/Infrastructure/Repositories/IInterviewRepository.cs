namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Repository interface for interview scheduling operations
    /// </summary>
    public interface IInterviewRepository
    {
        /// <summary>
        /// Schedules an interview for an application
        /// </summary>
        bool ScheduleInterview(int applicationID, DateTime interviewDate, DateTime interviewTime, string interviewer, string location);

        /// <summary>
        /// Gets interview schedule for an application
        /// </summary>
        dynamic GetInterviewSchedule(int applicationID);

        /// <summary>
        /// Updates interview status
        /// </summary>
        bool UpdateInterviewStatus(int applicationID, string status);
    }
}
