using HRAndApplicantSystem.Database;

public class LoginService
{
    DatabaseHelper db = new DatabaseHelper();

    public bool Login(string username, string password)
    {
        return db.ValidateLogin(username, password);
    }

    public bool RegisterApplicant(string username, string password)
    {
        return db.RegisterApplicant(username, password);
    }
}
