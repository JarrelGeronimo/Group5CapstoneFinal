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
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter a username.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter a password.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    passwordTextBox.Clear();
                    usernameTextBox.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during login: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter a username.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter a password.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool registerSuccess = _loginService.RegisterApplicant(username, password);

                if (registerSuccess)
                {
                    MessageBox.Show("Registration successful! You can now login with your credentials.", "Registration Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    usernameTextBox.Clear();
                    passwordTextBox.Clear();
                    usernameTextBox.Focus();
                }
                else
                {
                    MessageBox.Show("Registration failed. Username may already exist.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during registration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearLoginFields()
        {
            usernameTextBox.Clear();
            passwordTextBox.Clear();
        }
    }
}
