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
        // go to HR system or applicant dashboard
    }
    else
    {
        Console.WriteLine("Invalid credentials.");
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