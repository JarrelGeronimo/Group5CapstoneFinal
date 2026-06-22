using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Forms;
using WinFormsApp = System.Windows.Forms.Application;

namespace HRAndApplicantSystem.Login
{
    /// <summary>
    /// Login Form - Entry point of the application
    /// 
    /// ARCHITECTURE: UI Layer Only
    /// - Handles user input (username/password)
    /// - Delegates validation to LoginService
    /// - Navigates to MainForm on success
    /// - No business logic here
    /// </summary>
    public partial class LoginForm : Form
    {
        private readonly LoginService _loginService;
        private readonly DatabaseHelper _db;

        public LoginForm()
        {
            InitializeComponent();
            _loginService = new LoginService();
            _db = new DatabaseHelper();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            ClearStatus();

            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text;

            // Validation
            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Please enter a username.");
                usernameTextBox.Focus();
                return;
            }

            if (username.Length < 3)
            {
                ShowError("Username must be at least 3 characters.");
                usernameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter a password.");
                passwordTextBox.Focus();
                return;
            }

            try
            {
                bool loginSuccess = _loginService.Login(username, password);

                if (loginSuccess)
                {
                    // Get user details from database
                    int roleId = _db.GetUserRoleByUsername(username);
                    User user = new User 
                    { 
                        Username = username, 
                        RoleID = roleId 
                    };

                    // Navigate to MainForm (Dashboard)
                    MainForm mainForm = new MainForm(user, username);
                    this.Hide();
                    mainForm.ShowDialog();
                    // When MainForm closes (logout), show LoginForm again
                    this.Show();
                    ClearLoginFields();
                    usernameTextBox.Focus();
                }
                else
                {
                    ShowError("Invalid username or password. Please try again.");
                    passwordTextBox.Clear();
                    usernameTextBox.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Login error: {ex.Message}");
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            ClearStatus();

            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Please enter a username.");
                usernameTextBox.Focus();
                return;
            }

            if (username.Length < 3)
            {
                ShowError("Username must be at least 3 characters.");
                usernameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter a password.");
                passwordTextBox.Focus();
                return;
            }

            if (password.Length < 6)
            {
                ShowError("Password must be at least 6 characters long.");
                passwordTextBox.Focus();
                return;
            }

            try
            {
                bool registerSuccess = _loginService.RegisterApplicant(username, password, username);

                if (registerSuccess)
                {
                    ShowSuccess("Registration successful! You can now login.");
                    usernameTextBox.Clear();
                    passwordTextBox.Clear();
                    usernameTextBox.Focus();
                }
                else
                {
                    ShowError("Registration failed. Username may already exist.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Registration error: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            statusLabel.ForeColor = System.Drawing.Color.FromArgb(220, 20, 60);
            statusLabel.Text = message;
            statusLabel.Visible = true;
        }

        private void ShowSuccess(string message)
        {
            statusLabel.ForeColor = System.Drawing.Color.FromArgb(34, 139, 34);
            statusLabel.Text = message;
            statusLabel.Visible = true;
        }

        private void ClearStatus()
        {
            statusLabel.Visible = false;
            statusLabel.Text = "";
        }

        private void ClearLoginFields()
        {
            usernameTextBox.Clear();
            passwordTextBox.Clear();
            ClearStatus();
        }
    }
}
