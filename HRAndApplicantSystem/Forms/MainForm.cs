using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Login;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Infrastructure.Repositories;
using WinFormsApp = System.Windows.Forms.Application;

namespace HRAndApplicantSystem.Forms
{
    /// <summary>
    /// Main Dashboard Form - Entry point after successful login
    /// 
    /// ARCHITECTURE: UI Layer Only
    /// - Displays user dashboard based on role
    /// - Delegates to role-specific services via menu buttons
    /// - Does NOT contain business logic
    /// 
    /// Navigation Flow:
    /// LoginForm -> MainForm (based on user role)
    /// MainForm -> Specific forms (ApplicantListForm, JobVacancyForm, etc.)
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly User _currentUser;
        private readonly string _username;
        private readonly DatabaseHelper _db;

        public MainForm(User user, string username)
        {
            InitializeComponent();
            _currentUser = user;
            _username = username;
            _db = new DatabaseHelper();
            
            this.Text = "HR and Applicant System - Main Dashboard";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Load user welcome message
                LoadUserInfo();
                
                // Initialize menus
                InitializeMenus();
                
                // Load dashboard based on role
                LoadDashboardForRole();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUserInfo()
        {
            // Display welcome message
            string welcomeText = $"Welcome, {_username}! | Role: {GetRoleName(_currentUser.RoleID)}";
            welcomeLabel.Text = welcomeText;
            this.Text = $"HR and Applicant System - {GetRoleName(_currentUser.RoleID)} Dashboard";
        }

        private void LoadDashboardForRole()
        {
            // Clear content panel only (keep menu)
            contentPanel.Controls.Clear();

            // Load role-specific content
            switch (_currentUser.RoleID)
            {
                case (int)UserRole.Applicant:
                    AddApplicantDashboardContent();
                    break;
                case (int)UserRole.HR:
                case (int)UserRole.HRManager:
                case (int)UserRole.Admin:
                    AddHRDashboardContent();
                    break;
            }
        }

        private void InitializeMenus()
        {
            // Clear existing menu items
            menuStrip.Items.Clear();
            // Menu system to be implemented in Phase 2
            // For now, all navigation is through dashboard buttons
        }

        private void AddApplicantDashboardContent()
        {
            contentPanel.Controls.Clear();

            // Title
            Label titleLabel = new Label();
            titleLabel.Text = "Applicant Dashboard";
            titleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            titleLabel.Width = 500;
            titleLabel.Height = 40;
            titleLabel.Margin = new Padding(10, 10, 10, 20);
            contentPanel.Controls.Add(titleLabel);

            // Quick action panel - Applications
            Button viewAppsButton = new Button();
            viewAppsButton.Text = "View My Applications";
            viewAppsButton.Size = new System.Drawing.Size(300, 100);
            viewAppsButton.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
            viewAppsButton.ForeColor = System.Drawing.Color.White;
            viewAppsButton.Font = new Font("Arial", 12, FontStyle.Bold);
            viewAppsButton.Cursor = Cursors.Hand;
            viewAppsButton.Margin = new Padding(10);
            viewAppsButton.FlatStyle = FlatStyle.Flat;
            viewAppsButton.Click += (s, e) => OpenApplicantApplicationsForm();
            contentPanel.Controls.Add(viewAppsButton);

            // Quick action panel - Browse Jobs
            Button browseJobsButton = new Button();
            browseJobsButton.Text = "Browse Job Vacancies";
            browseJobsButton.Size = new System.Drawing.Size(300, 100);
            browseJobsButton.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
            browseJobsButton.ForeColor = System.Drawing.Color.White;
            browseJobsButton.Font = new Font("Arial", 12, FontStyle.Bold);
            browseJobsButton.Cursor = Cursors.Hand;
            browseJobsButton.Margin = new Padding(10);
            browseJobsButton.FlatStyle = FlatStyle.Flat;
            browseJobsButton.Click += (s, e) => OpenJobVacanciesForm();
            contentPanel.Controls.Add(browseJobsButton);

            // Logout button
            Button logoutButton = new Button();
            logoutButton.Text = "Logout";
            logoutButton.Size = new System.Drawing.Size(300, 100);
            logoutButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
            logoutButton.ForeColor = System.Drawing.Color.White;
            logoutButton.Font = new Font("Arial", 12, FontStyle.Bold);
            logoutButton.Cursor = Cursors.Hand;
            logoutButton.Margin = new Padding(10, 20, 10, 10);
            logoutButton.FlatStyle = FlatStyle.Flat;
            logoutButton.Click += (s, e) => LogoutUser();
            contentPanel.Controls.Add(logoutButton);

            contentPanel.PerformLayout();
        }

        private void AddHRDashboardContent()
        {
            contentPanel.Controls.Clear();

            // Title
            Label titleLabel = new Label();
            titleLabel.Text = "HR Management Dashboard";
            titleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            titleLabel.Width = 500;
            titleLabel.Height = 40;
            titleLabel.Margin = new Padding(10, 10, 10, 20);
            contentPanel.Controls.Add(titleLabel);

            // Quick action panel - View Applicants
            Button viewApplicantsButton = new Button();
            viewApplicantsButton.Text = "View Applicants";
            viewApplicantsButton.Size = new System.Drawing.Size(300, 100);
            viewApplicantsButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            viewApplicantsButton.ForeColor = System.Drawing.Color.White;
            viewApplicantsButton.Font = new Font("Arial", 12, FontStyle.Bold);
            viewApplicantsButton.Cursor = Cursors.Hand;
            viewApplicantsButton.Margin = new Padding(10);
            viewApplicantsButton.FlatStyle = FlatStyle.Flat;
            viewApplicantsButton.Click += (s, e) => OpenApplicantListForm();
            contentPanel.Controls.Add(viewApplicantsButton);

            // Quick action panel - Manage Applications
            Button manageAppsButton = new Button();
            manageAppsButton.Text = "Manage Applications";
            manageAppsButton.Size = new System.Drawing.Size(300, 100);
            manageAppsButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            manageAppsButton.ForeColor = System.Drawing.Color.White;
            manageAppsButton.Font = new Font("Arial", 12, FontStyle.Bold);
            manageAppsButton.Cursor = Cursors.Hand;
            manageAppsButton.Margin = new Padding(10);
            manageAppsButton.FlatStyle = FlatStyle.Flat;
            manageAppsButton.Click += (s, e) => OpenApplicationsForm();
            contentPanel.Controls.Add(manageAppsButton);

            // Quick action panel - Reports
            Button reportsButton = new Button();
            reportsButton.Text = "View Reports";
            reportsButton.Size = new System.Drawing.Size(300, 100);
            reportsButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            reportsButton.ForeColor = System.Drawing.Color.White;
            reportsButton.Font = new Font("Arial", 12, FontStyle.Bold);
            reportsButton.Cursor = Cursors.Hand;
            reportsButton.Margin = new Padding(10);
            reportsButton.FlatStyle = FlatStyle.Flat;
            reportsButton.Click += (s, e) => MessageBox.Show("Reports feature coming soon", "Feature", MessageBoxButtons.OK, MessageBoxIcon.Information);
            contentPanel.Controls.Add(reportsButton);

            // Logout button
            Button logoutButton = new Button();
            logoutButton.Text = "Logout";
            logoutButton.Size = new System.Drawing.Size(300, 100);
            logoutButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
            logoutButton.ForeColor = System.Drawing.Color.White;
            logoutButton.Font = new Font("Arial", 12, FontStyle.Bold);
            logoutButton.Cursor = Cursors.Hand;
            logoutButton.Margin = new Padding(10, 20, 10, 10);
            logoutButton.FlatStyle = FlatStyle.Flat;
            logoutButton.Click += (s, e) => LogoutUser();
            contentPanel.Controls.Add(logoutButton);

            contentPanel.PerformLayout();
        }

        private void OpenApplicantListForm()
        {
            try
            {
                ApplicantListForm form = new ApplicantListForm(_db);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening applicant list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenApplicantApplicationsForm()
        {
            MessageBox.Show("Applicant applications view coming soon", "Feature", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenJobVacanciesForm()
        {
            MessageBox.Show("Job vacancies view coming soon", "Feature", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenApplicationsForm()
        {
            MessageBox.Show("Applications management coming soon", "Feature", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LogoutUser()
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();  // Close MainForm - LoginForm will reappear underneath
            }
        }

        private string GetRoleName(int roleId)
        {
            return roleId switch
            {
                (int)UserRole.Applicant => "Applicant",
                (int)UserRole.HR => "HR Staff",
                (int)UserRole.HRManager => "HR Manager",
                (int)UserRole.Admin => "Administrator",
                _ => "Unknown"
            };
        }

        private static class UserRole
        {
            public const int Applicant = 1;
            public const int HR = 2;
            public const int HRManager = 3;
            public const int Admin = 4;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Create MenuStrip - MUST be first and docked to top
            menuStrip = new MenuStrip();
            menuStrip.Name = "menuStrip";
            menuStrip.Dock = DockStyle.Top;
            this.Controls.Add(menuStrip);

            // Create welcome panel - docked below menu
            welcomePanel = new Panel();
            welcomePanel.BackColor = System.Drawing.Color.FromArgb(220, 240, 255);
            welcomePanel.Dock = DockStyle.Top;
            welcomePanel.Height = 80;
            welcomePanel.Padding = new Padding(20);
            welcomePanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(welcomePanel);

            // Welcome label
            welcomeLabel = new Label();
            welcomeLabel.Text = "Welcome";
            welcomeLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            welcomeLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            welcomeLabel.Dock = DockStyle.Fill;
            welcomeLabel.TextAlign = ContentAlignment.MiddleLeft;
            welcomeLabel.Name = "welcomeLabel";
            welcomePanel.Controls.Add(welcomeLabel);

            // Create main content panel - fills remaining space
            contentPanel = new FlowLayoutPanel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.AutoScroll = true;
            contentPanel.Padding = new Padding(20);
            contentPanel.FlowDirection = FlowDirection.TopDown;
            contentPanel.WrapContents = false;
            contentPanel.Name = "contentPanel";
            this.Controls.Add(contentPanel);

            // Set form properties
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.MainMenuStrip = menuStrip;
            this.Name = "MainForm";
            this.Font = new Font("Segoe UI", 10);
            this.Load += new EventHandler(MainForm_Load);
            
            this.ResumeLayout(true);
            this.PerformLayout();
        }

        private MenuStrip menuStrip;
        private Panel welcomePanel;
        private Label welcomeLabel;
        private FlowLayoutPanel contentPanel;
    }
}
