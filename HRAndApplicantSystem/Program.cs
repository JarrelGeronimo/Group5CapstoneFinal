using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Login;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Utilities;
using HRAndApplicantSystem.Services;
using ApplicantModel = HRAndApplicantSystem.Models.Applicant;
using ApplicationServiceNS = HRAndApplicantSystem.Services.ApplicationService;


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
    ApplicationServiceNS appService = new ApplicationServiceNS();
    DatabaseHelper db = new DatabaseHelper();

    // Check if applicant has already filled in their info
    ApplicantModel applicant = appService.GetApplicantInfo(username);

    if (applicant == null)
    {
        Console.WriteLine("\nIt looks like you haven't completed your profile yet.");
        Console.WriteLine("Please fill in your information to proceed.\n");
        appService.CollectApplicantInfo(username);
        // Retrieve the newly created applicant info
        applicant = appService.GetApplicantInfo(username);
    }

    if (applicant != null)
    {
        // Use the new ApplicantDashboardService
        ApplicantDashboardService dashboardService = new ApplicantDashboardService();
        dashboardService.ShowDashboard(applicant, username);
    }
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
        Console.WriteLine("Registration successful! You can now login.");
    }
    else
    {
        Console.WriteLine("Registration failed. Username may already exist.");
    }
}