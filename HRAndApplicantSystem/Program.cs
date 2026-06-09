using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Login;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Utilities;
using HRAndApplicantSystem.Services;
using WinFormsApp = System.Windows.Forms.Application;

// Enable visual styles for Windows Forms
WinFormsApp.EnableVisualStyles();
WinFormsApp.SetCompatibleTextRenderingDefault(false);

// Create and run the login form
LoginForm loginForm = new LoginForm();
WinFormsApp.Run(loginForm);
