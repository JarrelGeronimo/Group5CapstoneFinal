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
                // For applicants, check if profile is complete
                if (_currentUser.RoleID == (int)UserRole.Applicant)
                {
                    if (NeedsProfileCompletion())
                    {
                        ShowApplicantProfileForm();
                        return; // Don't load dashboard until profile is complete
                    }
                }

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

        private bool NeedsProfileCompletion()
        {
            try
            {
                Applicant applicant = _db.GetApplicantByUsername(_username);
                
                // If no profile exists, needs completion
                if (applicant == null)
                {
                    return true;
                }

                // If profile exists but key fields are empty, needs completion
                if (string.IsNullOrWhiteSpace(applicant.FirstName) ||
                    string.IsNullOrWhiteSpace(applicant.LastName) ||
                    string.IsNullOrWhiteSpace(applicant.ContactNo) ||
                    string.IsNullOrWhiteSpace(applicant.Address))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NeedsProfileCompletion error: {ex.Message}");
                return false;
            }
        }

        private void ShowApplicantProfileForm()
        {
            try
            {
                ApplicantProfileForm profileForm = new ApplicantProfileForm(_db, _username);
                if (profileForm.ShowDialog(this) == DialogResult.OK)
                {
                    // Profile completed successfully, now load dashboard
                    LoadUserInfo();
                    InitializeMenus();
                    LoadDashboardForRole();
                }
                else
                {
                    // Profile not completed, close the application
                    MessageBox.Show("You must complete your profile to access the system.", "Profile Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profile form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
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

            // Workflow Stage 1: Review Pending Applications
            Button reviewAppsButton = new Button();
            reviewAppsButton.Text = "1. Review Pending\nApplications";
            reviewAppsButton.Size = new System.Drawing.Size(300, 100);
            reviewAppsButton.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
            reviewAppsButton.ForeColor = System.Drawing.Color.White;
            reviewAppsButton.Font = new Font("Arial", 11, FontStyle.Bold);
            reviewAppsButton.Cursor = Cursors.Hand;
            reviewAppsButton.Margin = new Padding(10);
            reviewAppsButton.FlatStyle = FlatStyle.Flat;
            reviewAppsButton.Click += (s, e) => OpenApplicationsFormFiltered("Pending");
            contentPanel.Controls.Add(reviewAppsButton);

            // Workflow Stage 2a: Schedule Interviews (Screening Status)
            Button screeningButton = new Button();
            screeningButton.Text = "2. Schedule\nInterviews";
            screeningButton.Size = new System.Drawing.Size(300, 100);
            screeningButton.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
            screeningButton.ForeColor = System.Drawing.Color.White;
            screeningButton.Font = new Font("Arial", 11, FontStyle.Bold);
            screeningButton.Cursor = Cursors.Hand;
            screeningButton.Margin = new Padding(10);
            screeningButton.FlatStyle = FlatStyle.Flat;
            screeningButton.Click += (s, e) => OpenApplicationsFormFiltered("Screening");
            contentPanel.Controls.Add(screeningButton);

            // Workflow Stage 2b: Evaluate Interviews (Interview Status)
            Button evaluateButton = new Button();
            evaluateButton.Text = "3. Evaluate\nInterviews";
            evaluateButton.Size = new System.Drawing.Size(300, 100);
            evaluateButton.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
            evaluateButton.ForeColor = System.Drawing.Color.White;
            evaluateButton.Font = new Font("Arial", 11, FontStyle.Bold);
            evaluateButton.Cursor = Cursors.Hand;
            evaluateButton.Margin = new Padding(10);
            evaluateButton.FlatStyle = FlatStyle.Flat;
            evaluateButton.Click += (s, e) => OpenApplicationsFormFiltered("Interview");
            contentPanel.Controls.Add(evaluateButton);

            // Workflow Stage 3: Make Hiring Decisions (Manager/Admin Only)
            Button hiringButton = new Button();
            hiringButton.Text = "4. Make Hiring\nDecisions";
            hiringButton.Size = new System.Drawing.Size(300, 100);
            hiringButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            hiringButton.ForeColor = System.Drawing.Color.White;
            hiringButton.Font = new Font("Arial", 11, FontStyle.Bold);
            hiringButton.Cursor = Cursors.Hand;
            hiringButton.Margin = new Padding(10);
            hiringButton.FlatStyle = FlatStyle.Flat;
            
            // Enable only for Manager (3) or Admin (4)
            if (_currentUser.RoleID == 3 || _currentUser.RoleID == 4)
            {
                hiringButton.Click += (s, e) => OpenApplicationsFormFiltered("For Final Review");
            }
            else
            {
                hiringButton.BackColor = System.Drawing.Color.FromArgb(169, 169, 169);
                hiringButton.Enabled = false;
                hiringButton.Click += (s, e) => MessageBox.Show("Only HR Manager or Admin can make hiring decisions.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            contentPanel.Controls.Add(hiringButton);

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
            try
            {
                // Get applicant ID from current username
                var applicant = _db.GetApplicantByUsername(_username);
                
                if (applicant == null)
                {
                    MessageBox.Show("Error: Could not retrieve applicant profile. Please contact support.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Open Job Vacancies Form with applicant ID
                JobVacanciesForm form = new JobVacanciesForm(_db, applicant.ApplicantID);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening job vacancies: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenApplicationsForm()
        {
            try
            {
                ApplicationManagementForm form = new ApplicationManagementForm(_db, _currentUser.RoleID);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening applications: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenApplicationsFormFiltered(string statusFilter)
        {
            try
            {
                ApplicationManagementForm form = new ApplicationManagementForm(_db, _currentUser.RoleID, statusFilter);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening applications: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
