using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Login;
using ApplicantModel = HRAndApplicantSystem.Applicant.Applicant;
using ApplicationServiceNS = HRAndApplicantSystem.Applicant.ApplicationService;

LoginService loginService = new LoginService();
bool running = true;

while (running)
{
    Console.WriteLine("\n=== HR and Applicant System ===");
    Console.WriteLine("1. Login");
    Console.WriteLine("2. Register as Applicant");
    Console.WriteLine("3. Exit");
    Console.Write("Choose an option: ");

    string choice = Console.ReadLine();

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
    Console.Write("Username: ");
    string username = Console.ReadLine();

    Console.Write("Password: ");
    string password = Console.ReadLine();

    if (loginService.Login(username, password))
    {
        Console.WriteLine("Login successful!");

        // Get user role to determine which dashboard to show
        DatabaseHelper db = new DatabaseHelper();
        int roleId = db.GetUserRoleByUsername(username);

        if (roleId == 1)  // Applicant
        {
            Console.WriteLine("\nWelcome Applicant!");
            ShowApplicantDashboard(username);
        }
        else if (roleId == 2 || roleId == 3)  // HR or HRManager
        {
            Console.WriteLine("\nWelcome HR Staff!");
            // TODO: Show HR dashboard
        }
        else if (roleId == 4)  // Admin
        {
            Console.WriteLine("\nWelcome Admin!");
            // TODO: Show Admin dashboard
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
        Console.WriteLine($"\nWelcome back, {applicant.FirstName} {applicant.LastName}!");
        
        bool dashboardRunning = true;
        while (dashboardRunning)
        {
            Console.WriteLine("\n=== Applicant Dashboard ===");
            Console.WriteLine("1. View Profile");
            Console.WriteLine("2. Update Profile");
            Console.WriteLine("3. Browse Job Vacancies");
            Console.WriteLine("4. View My Applications");
            Console.WriteLine("5. Logout");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // Fetch fresh data from database
                    applicant = appService.GetApplicantInfo(username);
                    ViewApplicantProfile(applicant);
                    break;
                case "2":
                    UpdateApplicantProfile(username, appService);
                    // Refresh the applicant data after update
                    applicant = appService.GetApplicantInfo(username);
                    break;
                case "3":
                    appService.BrowseJobVacancies(applicant);
                    break;
                case "4":
                    appService.ViewMyApplications(applicant);
                    break;
                case "5":
                    dashboardRunning = false;
                    Console.WriteLine("Logging out...");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}

static void ViewApplicantProfile(ApplicantModel applicant)
{
    Console.WriteLine("\n=== Your Profile ===");
    Console.WriteLine($"Name: {applicant.FirstName} {applicant.LastName}");
    Console.WriteLine($"Contact: {applicant.ContactNo}");
    Console.WriteLine($"Address: {applicant.Address}");
    Console.WriteLine($"Education: {applicant.Education}");
    Console.WriteLine($"Skills: {applicant.Skills}");
}

static void UpdateApplicantProfile(string username, ApplicationServiceNS appService)
{
    // Get the existing applicant info first
    ApplicantModel existingApplicant = appService.GetApplicantInfo(username);
    if (existingApplicant != null)
    {
        appService.UpdateApplicantProfile(username, existingApplicant);
    }
    else
    {
        Console.WriteLine("No existing profile found. Please complete your profile first.");
    }
}

static void HandleRegistration(LoginService loginService)
{
    Console.Write("Enter username: ");
    string username = Console.ReadLine();

    Console.Write("Enter password: ");
    string password = Console.ReadLine();

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