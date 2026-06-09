namespace HRAndApplicantSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Repository interface for screening operations
    /// </summary>
    public interface IScreeningRepository
    {
        /// <summary>
        /// Records a screening decision for an application
        /// </summary>
        bool ScreenApplication(int applicationID, string result, string remarks, string screenerUsername);
    }
}
