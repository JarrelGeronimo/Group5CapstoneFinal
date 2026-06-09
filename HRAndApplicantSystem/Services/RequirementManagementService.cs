using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Utilities;

namespace HRAndApplicantSystem.Services
{
    /// <summary>
    /// Manages job requirements for HR managers and admins.
    /// Allows viewing, adding, and removing requirement types.
    /// </summary>
    public class RequirementManagementService
    {
        private readonly IDocumentRepository documentRepository;
        private readonly DatabaseHelper db;

        public RequirementManagementService(IDocumentRepository docRepo = null)
        {
            documentRepository = docRepo ?? new DocumentRepository(new DatabaseHelper());
            db = new DatabaseHelper();
        }

        /// <summary>
        /// Shows the main requirement management menu
        /// </summary>
        public void ShowRequirementManagementMenu()
        {
            bool continueMenu = true;

            while (continueMenu)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     REQUIREMENT MANAGEMENT                   ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");

                Console.WriteLine("1. View All Requirement Types");
                Console.WriteLine("2. Add New Requirement Type");
                Console.WriteLine("3. Remove Requirement Type");
                Console.WriteLine("4. Back to Dashboard\n");

                Console.Write("Choose an option: ");
                string choice = Console.ReadLine()?.Trim() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        ViewAllRequirementTypes();
                        break;
                    case "2":
                        AddNewRequirementType();
                        break;
                    case "3":
                        RemoveRequirementType();
                        break;
                    case "4":
                        continueMenu = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ViewAllRequirementTypes()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     ALL REQUIREMENT TYPES                    ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var requirements = db.GetAllRequirementTypes();

            if (requirements.Count == 0)
            {
                Console.WriteLine("No requirement types found.\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"#",-3} {"Requirement Type",-40}");
            Console.WriteLine(new string('-', 45));

            for (int i = 0; i < requirements.Count; i++)
            {
                var req = requirements[i];
                Console.WriteLine($"{i + 1,-3} {req.RequirementName,-40}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void AddNewRequirementType()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     ADD NEW REQUIREMENT TYPE                 ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            string requirementName = InputValidator.GetValidatedInput("Enter requirement type name: ", "Requirement Type");

            if (string.IsNullOrWhiteSpace(requirementName))
            {
                Console.WriteLine("Requirement type name cannot be empty.");
                System.Threading.Thread.Sleep(1500);
                return;
            }

            if (db.AddRequirementType(requirementName))
            {
                Console.WriteLine($"\n✓ Successfully added requirement type: {requirementName}");
            }
            else
            {
                Console.WriteLine($"\n✗ Failed to add requirement type.");
            }

            System.Threading.Thread.Sleep(1500);
        }

        private void RemoveRequirementType()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     REMOVE REQUIREMENT TYPE                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var requirements = db.GetAllRequirementTypes();

            if (requirements.Count == 0)
            {
                Console.WriteLine("No requirement types available to remove.\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"#",-3} {"Requirement Type",-40}");
            Console.WriteLine(new string('-', 45));

            for (int i = 0; i < requirements.Count; i++)
            {
                var req = requirements[i];
                Console.WriteLine($"{i + 1,-3} {req.RequirementName,-40}");
            }

            Console.WriteLine($"\n{requirements.Count + 1}. Cancel");
            Console.Write("\nSelect requirement to remove: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice)
                || choice < 1 || choice > requirements.Count + 1)
            {
                return;
            }

            if (choice == requirements.Count + 1)
            {
                return; // Cancel
            }

            var selected = requirements[choice - 1];

            Console.Write($"\nAre you sure you want to remove '{selected.RequirementName}'? (yes/no): ");
            string confirm = (Console.ReadLine()?.Trim() ?? string.Empty).ToLower();

            if (confirm == "yes" || confirm == "y")
            {
                if (db.RemoveRequirementType(selected.RequirementTypeID))
                {
                    Console.WriteLine($"\n✓ Successfully removed requirement type: {selected.RequirementName}");
                }
                else
                {
                    Console.WriteLine($"\n✗ Failed to remove requirement type.");
                }
            }
            else
            {
                Console.WriteLine("\nCancelled.");
            }

            System.Threading.Thread.Sleep(1500);
        }
    }
}
