using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Services.Business;

namespace HRAndApplicantSystem.Services.UI
{
    /// <summary>
    /// UI rendering for applicant profile management.
    /// Handles all console output and user interaction.
    /// Uses ApplicantProfileBusinessService for business logic.
    /// </summary>
    public class ApplicantProfileUIService
    {
        private readonly ApplicantProfileBusinessService businessService;

        public ApplicantProfileUIService(ApplicantProfileBusinessService businessService = null)
        {
            this.businessService = businessService ?? new ApplicantProfileBusinessService();
        }

        /// <summary>
        /// Display applicant profile management menu
        /// </summary>
        public void ManageProfile(int applicantId)
        {
            bool managing = true;
            while (managing)
            {
                var applicant = businessService.GetApplicantDetails(applicantId);
                if (applicant == null)
                {
                    Console.WriteLine("Error: Applicant not found.");
                    System.Threading.Thread.Sleep(1500);
                    managing = false;
                    continue;
                }

                DisplayProfileMenu(applicant);
                managing = false;
            }
        }

        private void DisplayProfileMenu(Applicant applicant)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     MY PROFILE                               ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            int completionPercentage = businessService.GetProfileCompletionPercentage(applicant);
            DisplayCompletionBar(completionPercentage);

            Console.WriteLine("\n=== PROFILE INFORMATION ===\n");
            DisplayProfileFields(applicant);

            Console.WriteLine("\n=== OPTIONS ===\n");
            DisplayEditOptions();
            Console.WriteLine("9. Back");

            Console.Write("\nSelect an option: ");
            string choice = Console.ReadLine()?.Trim() ?? "";

            HandleProfileMenuChoice(choice, applicant);
        }

        private void DisplayCompletionBar(int percentage)
        {
            Console.WriteLine($"Profile Completion: {percentage}%");
            int barLength = 20;
            int filled = (percentage * barLength) / 100;
            string bar = "█".PadRight(filled, '█').PadRight(barLength, '░');
            Console.WriteLine($"[{bar}]");
        }

        private void DisplayProfileFields(Applicant applicant)
        {
            Console.WriteLine($"1. First Name:      {GetDisplayValue(applicant.FirstName)}");
            Console.WriteLine($"2. Last Name:       {GetDisplayValue(applicant.LastName)}");
            Console.WriteLine($"3. Contact No:      {GetDisplayValue(applicant.ContactNo)}");
            Console.WriteLine($"4. Address:         {GetDisplayValue(applicant.Address)}");
            Console.WriteLine($"5. Education:       {GetDisplayValue(applicant.Education)}");
            Console.WriteLine($"6. Skills:          {GetDisplayValue(applicant.Skills)}");
        }

        private void DisplayEditOptions()
        {
            Console.WriteLine("1. Edit First Name");
            Console.WriteLine("2. Edit Last Name");
            Console.WriteLine("3. Edit Contact Number");
            Console.WriteLine("4. Edit Address");
            Console.WriteLine("5. Edit Education");
            Console.WriteLine("6. Edit Skills");
            Console.WriteLine("7. View Incomplete Fields");
        }

        private void HandleProfileMenuChoice(string choice, Applicant applicant)
        {
            switch (choice)
            {
                case "1":
                    EditFirstName(applicant);
                    break;
                case "2":
                    EditLastName(applicant);
                    break;
                case "3":
                    EditContactNo(applicant);
                    break;
                case "4":
                    EditAddress(applicant);
                    break;
                case "5":
                    EditEducation(applicant);
                    break;
                case "6":
                    EditSkills(applicant);
                    break;
                case "7":
                    ViewIncompleteFields(applicant);
                    break;
            }
        }

        private void EditFirstName(Applicant applicant)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     EDIT FIRST NAME                          ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");
            
            Console.WriteLine($"Current First Name: {GetDisplayValue(applicant.FirstName)}");
            Console.Write("\nEnter new first name: ");
            string newName = Console.ReadLine()?.Trim() ?? "";

            if (businessService.UpdateFirstName(applicant.ApplicantID, newName, applicant.Username))
            {
                Console.WriteLine("\n✓ First name updated successfully.");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to update first name.");
            }

            System.Threading.Thread.Sleep(1500);
        }

        private void EditLastName(Applicant applicant)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     EDIT LAST NAME                           ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");
            
            Console.WriteLine($"Current Last Name: {GetDisplayValue(applicant.LastName)}");
            Console.Write("\nEnter new last name: ");
            string newName = Console.ReadLine()?.Trim() ?? "";

            if (businessService.UpdateLastName(applicant.ApplicantID, newName, applicant.Username))
            {
                Console.WriteLine("\n✓ Last name updated successfully.");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to update last name.");
            }

            System.Threading.Thread.Sleep(1500);
        }

        private void EditContactNo(Applicant applicant)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     EDIT CONTACT NUMBER                      ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");
            
            Console.WriteLine($"Current Contact: {GetDisplayValue(applicant.ContactNo)}");
            Console.Write("\nEnter new contact number: ");
            string newContact = Console.ReadLine()?.Trim() ?? "";

            if (!businessService.IsValidPhoneNumber(newContact))
            {
                Console.WriteLine("\n✗ Invalid contact number (minimum 10 digits).");
                System.Threading.Thread.Sleep(1500);
                return;
            }

            if (businessService.UpdateContactNo(applicant.ApplicantID, newContact, applicant.Username))
            {
                Console.WriteLine("\n✓ Contact number updated successfully.");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to update contact number.");
            }

            System.Threading.Thread.Sleep(1500);
        }

        private void EditAddress(Applicant applicant)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     EDIT ADDRESS                             ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");
            
            Console.WriteLine($"Current Address: {GetDisplayValue(applicant.Address)}");
            Console.Write("\nEnter new address: ");
            string newAddress = Console.ReadLine()?.Trim() ?? "";

            if (businessService.UpdateAddress(applicant.ApplicantID, newAddress, applicant.Username))
            {
                Console.WriteLine("\n✓ Address updated successfully.");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to update address.");
            }

            System.Threading.Thread.Sleep(1500);
        }

        private void EditEducation(Applicant applicant)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     EDIT EDUCATION                           ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");
            
            Console.WriteLine($"Current Education: {GetDisplayValue(applicant.Education)}");
            Console.Write("\nEnter education details: ");
            string newEducation = Console.ReadLine()?.Trim() ?? "";

            if (businessService.UpdateEducation(applicant.ApplicantID, newEducation, applicant.Username))
            {
                Console.WriteLine("\n✓ Education updated successfully.");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to update education.");
            }

            System.Threading.Thread.Sleep(1500);
        }

        private void EditSkills(Applicant applicant)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     EDIT SKILLS                              ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");
            
            Console.WriteLine($"Current Skills: {GetDisplayValue(applicant.Skills)}");
            Console.Write("\nEnter skills (comma-separated): ");
            string newSkills = Console.ReadLine()?.Trim() ?? "";

            if (businessService.UpdateSkills(applicant.ApplicantID, newSkills, applicant.Username))
            {
                Console.WriteLine("\n✓ Skills updated successfully.");
            }
            else
            {
                Console.WriteLine("\n✗ Failed to update skills.");
            }

            System.Threading.Thread.Sleep(1500);
        }

        private void ViewIncompleteFields(Applicant applicant)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     INCOMPLETE FIELDS                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var incomplete = businessService.GetIncompleteFields(applicant);

            if (incomplete.Count == 0)
            {
                Console.WriteLine("✓ Your profile is complete!");
            }
            else
            {
                Console.WriteLine("Please complete these fields:\n");
                for (int i = 0; i < incomplete.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {incomplete[i]}");
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private string GetDisplayValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "[Not provided]" : value;
        }
    }
}
