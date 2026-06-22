using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;

namespace HRAndApplicantSystem.Login
{
    public class LoginService
    {
        private readonly IUserRepository userRepository;

        public LoginService(IUserRepository userRepo = null)
        {
            userRepository = userRepo ?? new UserRepository(new DatabaseHelper());
        }

        public bool Login(string username, string password)
        {
            return userRepository.ValidateLogin(username, password);
        }

        public bool RegisterApplicant(string username, string password, string email)
        {
            return userRepository.RegisterApplicant(username, password, email);
        }
    }
}
