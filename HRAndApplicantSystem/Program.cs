using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Login;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Utilities;
using HRAndApplicantSystem.Services;

LoginService loginService = new LoginService();
bool running = true;

while (running)
{
    Console.WriteLine("\n=== HR and Applicant System ===");
    Console.WriteLine("1. Login");
    Console.WriteLine("2. Register an Account");
    Console.WriteLine("3. Exit");
    Console.Write("Choose an option: ");

    string choice = Console.ReadLine()?.Trim() ?? string.Empty;

    switch (choice)
    {
        case "1":
            HandleLogin(loginService);
            break;
        case "2":
            HandleRegistration(loginService);
            break;
        case "3":
            running = false;
            Console.WriteLine("Goodbye!");
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }
}

static void HandleLogin(LoginService loginService)
{
    string username = InputValidator.GetValidatedUsername();
    string password = InputValidator.GetValidatedPassword();

    if (loginService.Login(username, password))
    {
        Console.WriteLine("Login successful!");

        // Get user role to determine which dashboard to show
        DatabaseHelper db = new DatabaseHelper();
        int roleId = db.GetUserRoleByUsername(username);

        if (roleId == (int)UserRole.Applicant)
        {
            Console.WriteLine("\nWelcome Applicant!");
            ShowApplicantDashboard(username);
        }
        else if (roleId == (int)UserRole.HR)
        {
            Console.WriteLine("\nWelcome HR Staff!");
            ShowHRDashboard(username);
        }
        else if (roleId == (int)UserRole.HRManager)
        {
            Console.WriteLine("\nWelcome HR Manager!");
            ShowHRDashboard(username);
        }
        else if (roleId == (int)UserRole.Admin)
        {
            Console.WriteLine("\nWelcome Admin!");
            ShowHRDashboard(username);
        }
    }
    else
    {
        Console.WriteLine("Invalid credentials.");
    }
}

static void ShowApplicantDashboard(string username)
{
    DatabaseHelper db = new DatabaseHelper();

    // Retrieve applicant info from database
    Applicant applicant = db.GetApplicantByUsername(username);

    if (applicant == null)
    {
        Console.WriteLine("\nError: Could not retrieve applicant information.");
        Console.WriteLine("Please contact support if this issue persists.");
        return;
    }

    // Use the new ApplicantDashboardService
    ApplicantDashboardService dashboardService = new ApplicantDashboardService();
    dashboardService.ShowDashboard(applicant, username);
}

static void ShowHRDashboard(string hrUsername)
{
    HRAndApplicantSystem.Services.HRDashboardService hrDashboard = new HRAndApplicantSystem.Services.HRDashboardService();
    hrDashboard.ShowDashboard(hrUsername);
}

static void HandleRegistration(LoginService loginService)
{
    string username = InputValidator.GetValidatedUsername();
    string password = InputValidator.GetValidatedPassword();

    bool success = loginService.RegisterApplicant(username, password);

    if (success)
    {
        Console.WriteLine("Registration successful! You can now log in.");
    }
    else
    {
        Console.WriteLine("Registration failed. Please try again.");
    }
}
