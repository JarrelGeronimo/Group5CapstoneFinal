using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Services.Business;

namespace HRAndApplicantSystem.Services.UI
{
    /// <summary>
    /// UI rendering for application management.
    /// Handles all console output and user interaction.
    /// Uses ApplicationManagementBusinessService for business logic.
    /// </summary>
    public class ApplicationManagementUIService
    {
        private readonly ApplicationManagementBusinessService businessService;

        public ApplicationManagementUIService(ApplicationManagementBusinessService businessService = null)
        {
            this.businessService = businessService ?? new ApplicationManagementBusinessService();
        }

        /// <summary>
        /// Show main applications list interface
        /// </summary>
        public void ManageApplications(Applicant applicant)
        {
            bool managing = true;
            while (managing)
            {
                var applications = businessService.GetApplicantApplications(applicant.ApplicantID);

                if (applications.Count == 0)
                {
                    ShowNoApplicationsMessage();
                    managing = false;
                    continue;
                }

                DisplayApplicationList(applications, applicant);
                managing = false;
            }
        }

        private void ShowNoApplicationsMessage()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     MY APPLICATIONS                          ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");
            Console.WriteLine("You haven't applied for any jobs yet.");
            Console.WriteLine("\nStart by browsing job vacancies to find positions that interest you.");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void DisplayApplicationList(List<dynamic> applications, Applicant applicant)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     YOUR APPLICATIONS                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.WriteLine($"{"#",-3} {"Job Title",-25} {"Status",-15} {"Date",-10}");
            Console.WriteLine(new string('-', 65));

            int counter = 1;
            var appList = applications.ToList();
            foreach (var app in appList)
            {
                string status = ((string)app.ApplicationStatus).Length > 14 
                    ? ((string)app.ApplicationStatus).Substring(0, 14) 
                    : (string)app.ApplicationStatus;
                string date = ((DateTime)app.DateApplied).ToString("yyyy-MM-dd");
                string jobTitle = ((string)app.JobTitle).Length > 24 
                    ? ((string)app.JobTitle).Substring(0, 24) 
                    : (string)app.JobTitle;
                Console.WriteLine($"{counter,-3} {jobTitle,-25} {status,-15} {date,-10}");
                counter++;
            }

            Console.WriteLine($"\n{counter}. Back");
            Console.Write("\nSelect an application to view details: ");

            if (int.TryParse(Console.ReadLine()?.Trim(), out int choice) && choice >= 1 && choice < counter)
            {
                var selectedApp = appList[choice - 1];
                
                var application = new Application
                {
                    ApplicationID = (int)selectedApp.ApplicationID,
                    ApplicantID = (int)selectedApp.ApplicantID,
                    JobID = (int)selectedApp.JobID,
                    ApplicationStatus = (string)selectedApp.ApplicationStatus,
                    DateApplied = (DateTime)selectedApp.DateApplied
                };

                ShowApplicationDetails(application, applicant);
            }
        }

        private void ShowApplicationDetails(Application application, Applicant applicant)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     APPLICATION DETAILS                      ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            Console.WriteLine($"Application ID:  {application.ApplicationID}");
            Console.WriteLine($"Job ID:          {application.JobID}");
            Console.WriteLine($"Current Status:  {application.ApplicationStatus}");
            Console.WriteLine($"Date Applied:    {application.DateApplied:MMMM dd, yyyy HH:mm}");

            Console.WriteLine("\n=== Application Status Summary ===\n");
            Console.WriteLine(businessService.GetStatusMessage(application.ApplicationStatus));

            DisplayActionMenu(application, applicant);
        }

        private void DisplayActionMenu(Application application, Applicant applicant)
        {
            Console.WriteLine("\n=== Actions ===\n");
            Console.WriteLine("1. View Application Summary");
            Console.WriteLine("2. Manage Documents");
            
            int nextOption = 3;
            if (businessService.IsInterviewScheduled(application.ApplicationStatus))
            {
                Console.WriteLine($"{nextOption}. View Interview Details");
                nextOption++;
            }
            if (businessService.IsDraft(application.ApplicationStatus))
            {
                Console.WriteLine($"{nextOption}. Resume Draft");
                nextOption++;
                Console.WriteLine($"{nextOption}. Delete Draft");
                nextOption++;
            }
            Console.WriteLine($"{nextOption}. Back");

            Console.Write("\nChoose an option: ");

            string actionChoice = Console.ReadLine()?.Trim() ?? string.Empty;
            HandleApplicationAction(actionChoice, application, applicant);

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void HandleApplicationAction(string choice, Application application, Applicant applicant)
        {
            if (businessService.IsDraft(application.ApplicationStatus))
            {
                switch (choice)
                {
                    case "1":
                        ShowApplicationSummary(application);
                        break;
                    case "2":
                        ManageApplicationDocuments(application);
                        break;
                    case "3":
                        ResumeDraft(application);
                        break;
                    case "4":
                        DeleteDraft(application);
                        break;
                }
            }
            else
            {
                switch (choice)
                {
                    case "1":
                        ShowApplicationSummary(application);
                        break;
                    case "2":
                        ManageApplicationDocuments(application);
                        break;
                    case "3":
                        if (businessService.IsInterviewScheduled(application.ApplicationStatus))
                            ShowInterviewDetails(application);
                        break;
                }
            }
        }

        private void ShowApplicationSummary(Application application)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     APPLICATION SUMMARY                      ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");
            Console.WriteLine($"Status: {application.ApplicationStatus}");
            Console.WriteLine($"Applied: {application.DateApplied:MMMM dd, yyyy HH:mm}");
            Console.WriteLine("\n" + new string('=', 45));
        }

        private void ShowInterviewDetails(Application application)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     INTERVIEW DETAILS                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            var interview = businessService.GetInterviewSchedule(application.ApplicationID);
            
            if (interview == null)
            {
                Console.WriteLine("No interview scheduled yet.\n");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Your interview has been scheduled!");
            Console.WriteLine("\n" + new string('=', 45));
            Console.WriteLine("\n📅 INTERVIEW SCHEDULE");
            
            if (interview.InterviewDate != null && interview.InterviewTime != null)
            {
                DateTime interviewDate = (DateTime)interview.InterviewDate;
                DateTime interviewTime = (DateTime)interview.InterviewTime;
                DateTime combined = interviewDate.Date.Add(interviewTime.TimeOfDay);
                Console.WriteLine($"Date & Time: {combined:MMMM dd, yyyy h:mm tt}");
            }
            else
            {
                Console.WriteLine("Date & Time: Not set");
            }

            Console.WriteLine($"Interviewer: {interview.Interviewer ?? "Not assigned"}");
            Console.WriteLine($"Location: {interview.Location ?? "Not specified"}");
            Console.WriteLine($"Status: {interview.Status}");

            Console.WriteLine("\n" + new string('=', 45));
            Console.WriteLine("\nYou will receive an email confirmation with:");
            Console.WriteLine("  • Interview date and time");
            Console.WriteLine("  • Location or video call link");
            Console.WriteLine("  • Interviewer information");
            Console.WriteLine("  • Required documents to bring");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ResumeDraft(Application application)
        {
            var jobVacancy = businessService.GetJobVacancy(application.JobID);
            if (jobVacancy == null)
            {
                Console.WriteLine("\n✗ Error: Job position not found.");
                System.Threading.Thread.Sleep(1500);
                return;
            }

            var applicant = businessService.GetApplicant(application.ApplicantID);
            if (applicant == null)
            {
                Console.WriteLine("\n✗ Error: Applicant information not found.");
                System.Threading.Thread.Sleep(1500);
                return;
            }

            var workflowService = new ApplicationWorkflowService();
            workflowService.ResumeDraftApplication(jobVacancy, applicant, application.ApplicationID);
        }

        private void DeleteDraft(Application application)
        {
            Console.WriteLine("\nAre you sure you want to delete this draft? (yes/no): ");
            string confirm = (Console.ReadLine()?.Trim() ?? "no").ToLower();
            
            if (confirm == "yes" || confirm == "y")
            {
                if (businessService.DeleteApplication(application.ApplicationID))
                {
                    Console.WriteLine("✓ Draft deleted successfully.");
                    System.Threading.Thread.Sleep(1500);
                }
                else
                {
                    Console.WriteLine("✗ Failed to delete draft.");
                    System.Threading.Thread.Sleep(1500);
                }
            }
        }

        private void ManageApplicationDocuments(Application application)
        {
            if (!businessService.IsEditable(application.ApplicationStatus))
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════╗");
                Console.WriteLine("║     DOCUMENT MANAGEMENT                      ║");
                Console.WriteLine("╚══════════════════════════════════════════════╝\n");
                Console.WriteLine("Your application is currently under review or has been decided.");
                Console.WriteLine("You cannot edit your document submissions once HR has started reviewing your application.\n");
                Console.WriteLine("Current Status: " + application.ApplicationStatus);
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            var docService = new DocumentSubmissionService();
            docService.ManageDocumentSubmissions(application.ApplicantID, application.JobID);
        }
    }
}
