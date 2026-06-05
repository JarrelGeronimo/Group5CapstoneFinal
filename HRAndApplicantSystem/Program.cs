LoginService loginService = new LoginService();

Console.Write("Username: ");
string username = Console.ReadLine();

Console.Write("Password: ");
string password = Console.ReadLine();

if (loginService.Login(username, password))
{
    Console.WriteLine("Login successful!");
    // go to HR system
}
else
{
    Console.WriteLine("Invalid credentials.");
}