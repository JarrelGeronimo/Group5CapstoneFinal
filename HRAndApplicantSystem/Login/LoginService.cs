using HRAndApplicantSystem.Database;

public class LoginService
{
    DatabaseHelper db = new DatabaseHelper();

    public bool Login(string username, string password)
    {
        return db.ValidateLogin(username, password);
    }
}