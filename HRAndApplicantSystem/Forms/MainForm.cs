using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Login;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Infrastructure.Repositories;
using WinFormsApp = System.Windows.Forms.Application;
using System.Linq;

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

            // Quick action panel - View/Edit Profile
            Button profileButton = new Button();
            profileButton.Text = "View/Edit My Profile";
            profileButton.Size = new System.Drawing.Size(300, 100);
            profileButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            profileButton.ForeColor = System.Drawing.Color.White;
            profileButton.Font = new Font("Arial", 12, FontStyle.Bold);
            profileButton.Cursor = Cursors.Hand;
            profileButton.Margin = new Padding(10);
            profileButton.FlatStyle = FlatStyle.Flat;
            profileButton.Click += (s, e) => ViewEditApplicantProfile();
            contentPanel.Controls.Add(profileButton);

            // Quick action panel - Change Credentials
            Button changeCredentialsButton = new Button();
            changeCredentialsButton.Text = "Change Username\nor Password";
            changeCredentialsButton.Size = new System.Drawing.Size(300, 100);
            changeCredentialsButton.BackColor = System.Drawing.Color.FromArgb(184, 134, 11);
            changeCredentialsButton.ForeColor = System.Drawing.Color.White;
            changeCredentialsButton.Font = new Font("Arial", 12, FontStyle.Bold);
            changeCredentialsButton.Cursor = Cursors.Hand;
            changeCredentialsButton.Margin = new Padding(10);
            changeCredentialsButton.FlatStyle = FlatStyle.Flat;
            changeCredentialsButton.Click += (s, e) => ChangeCredentials();
            contentPanel.Controls.Add(changeCredentialsButton);

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
            screeningButton.Click += (s, e) => OpenApplicationsFormFiltered("Shortlisted");
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
            evaluateButton.Click += (s, e) => OpenApplicationsFormFiltered("Interview Scheduled");
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

            // Job Vacancy Management (HR-specific feature)
            Button jobVacancyButton = new Button();
            jobVacancyButton.Text = "5. Manage Job\nVacancies";
            jobVacancyButton.Size = new System.Drawing.Size(300, 100);
            jobVacancyButton.BackColor = System.Drawing.Color.FromArgb(153, 102, 204);
            jobVacancyButton.ForeColor = System.Drawing.Color.White;
            jobVacancyButton.Font = new Font("Arial", 11, FontStyle.Bold);
            jobVacancyButton.Cursor = Cursors.Hand;
            jobVacancyButton.Margin = new Padding(10);
            jobVacancyButton.FlatStyle = FlatStyle.Flat;
            jobVacancyButton.Click += (s, e) => OpenJobVacancyManagementForm();
            contentPanel.Controls.Add(jobVacancyButton);

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
            reportsButton.Click += (s, e) => 
            {
                try
                {
                    ReportsForm form = new ReportsForm(_db, _currentUser.Username);
                    form.ShowDialog(this);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening reports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            
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
            try
            {
                // Get applicant ID from current username
                var applicant = _db.GetApplicantByUsername(_username);

                if (applicant == null)
                {
                    MessageBox.Show("Error: Could not retrieve applicant profile. Please contact support.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get all applications for this applicant
                var applications = _db.GetApplicantApplications(applicant.ApplicantID);

                // Create a simple dialog to display applications
                Form applicationsForm = new Form();
                applicationsForm.Text = $"My Applications - {applicant.FirstName} {applicant.LastName}";
                applicationsForm.Size = new System.Drawing.Size(900, 600);
                applicationsForm.StartPosition = FormStartPosition.CenterParent;

                // Create DataGridView
                DataGridView dgvApplications = new DataGridView();
                dgvApplications.Dock = DockStyle.Fill;
                dgvApplications.AllowUserToAddRows = false;
                dgvApplications.AllowUserToDeleteRows = false;
                dgvApplications.ReadOnly = true;
                dgvApplications.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvApplications.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Create a list of display objects
                var displayApplications = new List<dynamic>();
                foreach (var app in applications)
                {
                    displayApplications.Add(new
                    {
                        ApplicationID = app.ApplicationID,
                        JobTitle = app.JobTitle,
                        Status = app.ApplicationStatus,
                        DateApplied = Convert.ToDateTime(app.DateApplied).ToString("yyyy-MM-dd"),
                        JobCompany = "Company", // Default placeholder
                        Notes = app.ApplicationStatus == ApplicationStatus.Rejected ? "Not Selected" : ""
                    });
                }

                dgvApplications.DataSource = displayApplications;

                // Format columns
                if (dgvApplications.Columns["ApplicationID"] != null)
                    dgvApplications.Columns["ApplicationID"].HeaderText = "App ID";
                if (dgvApplications.Columns["JobTitle"] != null)
                    dgvApplications.Columns["JobTitle"].HeaderText = "Position";
                if (dgvApplications.Columns["Status"] != null)
                    dgvApplications.Columns["Status"].HeaderText = "Current Status";
                if (dgvApplications.Columns["DateApplied"] != null)
                    dgvApplications.Columns["DateApplied"].HeaderText = "Applied On";
                if (dgvApplications.Columns["JobCompany"] != null)
                    dgvApplications.Columns["JobCompany"].Visible = false;
                if (dgvApplications.Columns["Notes"] != null)
                    dgvApplications.Columns["Notes"].HeaderText = "Notes";

                // Add row double-click event to open details
                dgvApplications.CellDoubleClick += (s, e) =>
                {
                    if (e.RowIndex >= 0 && dgvApplications.Rows[e.RowIndex].DataBoundItem != null)
                    {
                        var selectedRow = (dynamic)dgvApplications.Rows[e.RowIndex].DataBoundItem;
                        int applicationID = selectedRow.ApplicationID;
                        ShowApplicationDetails(applicant, applicationID);
                    }
                };

                applicationsForm.Controls.Add(dgvApplications);

                // Add close button at the bottom
                Panel buttonPanel = new Panel();
                buttonPanel.Height = 50;
                buttonPanel.Dock = DockStyle.Bottom;

                Button closeButton = new Button();
                closeButton.Text = "Close";
                closeButton.Size = new System.Drawing.Size(100, 35);
                closeButton.Location = new System.Drawing.Point(790, 7);
                closeButton.Click += (s, e) => applicationsForm.Close();
                buttonPanel.Controls.Add(closeButton);

                applicationsForm.Controls.Add(buttonPanel);

                // Show the dialog
                applicationsForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading applications: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowApplicationDetails(Applicant applicant, int applicationID)
        {
            try
            {
                // Get detailed application information
                var appDetails = _db.GetApplicationDetailsForScreening(applicationID);

                if (appDetails == null)
                {
                    MessageBox.Show("Could not retrieve application details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool isDraft = appDetails.Status == ApplicationStatus.Draft;

                // Create details form
                Form detailsForm = new Form();
                detailsForm.Text = $"Application Details - {appDetails.JobTitle}";
                detailsForm.Size = new System.Drawing.Size(800, 650);
                detailsForm.StartPosition = FormStartPosition.CenterParent;
                detailsForm.AutoScroll = true;

                // Create main panel
                Panel mainPanel = new Panel();
                mainPanel.Dock = DockStyle.Fill;
                mainPanel.AutoScroll = true;
                mainPanel.Padding = new Padding(15);

                int yPos = 10;

                // Job Title
                Label jobTitleLabel = new Label() { Text = "Position:", Location = new System.Drawing.Point(15, yPos), Width = 200 };
                Label jobTitleValue = new Label() { Text = appDetails.JobTitle, Location = new System.Drawing.Point(250, yPos), Width = 500, Font = new Font("Arial", 11, FontStyle.Bold) };
                mainPanel.Controls.Add(jobTitleLabel);
                mainPanel.Controls.Add(jobTitleValue);
                yPos += 35;

                // Job Details
                Label jobDetailLabel = new Label() { Text = "Job Details:", Location = new System.Drawing.Point(15, yPos), Width = 200 };
                mainPanel.Controls.Add(jobDetailLabel);
                yPos += 25;

                TextBox jobDetailValue = new TextBox() { Text = appDetails.JobDetail, Location = new System.Drawing.Point(15, yPos), Width = 750, Height = 80, ReadOnly = true, Multiline = true, BackColor = System.Drawing.Color.WhiteSmoke };
                mainPanel.Controls.Add(jobDetailValue);
                yPos += 90;

                // Application Status
                Label statusLabel = new Label() { Text = "Application Status:", Location = new System.Drawing.Point(15, yPos), Width = 200 };
                Label statusValue = new Label() { Text = appDetails.Status, Location = new System.Drawing.Point(250, yPos), Width = 500 };
                if (appDetails.Status == ApplicationStatus.Draft)
                    statusValue.ForeColor = System.Drawing.Color.Orange;
                else if (appDetails.Status == ApplicationStatus.Accepted)
                    statusValue.ForeColor = System.Drawing.Color.Green;
                else if (appDetails.Status == ApplicationStatus.Rejected)
                    statusValue.ForeColor = System.Drawing.Color.Red;
                mainPanel.Controls.Add(statusLabel);
                mainPanel.Controls.Add(statusValue);
                yPos += 35;

                // Date Applied
                Label dateLabel = new Label() { Text = "Date Applied:", Location = new System.Drawing.Point(15, yPos), Width = 200 };
                Label dateValue = new Label() { Text = appDetails.DateApplied.ToString("yyyy-MM-dd"), Location = new System.Drawing.Point(250, yPos), Width = 500 };
                mainPanel.Controls.Add(dateLabel);
                mainPanel.Controls.Add(dateValue);
                yPos += 35;

                // Show interview schedule if application is in Interview Scheduled status
                if (appDetails.Status == ApplicationStatus.InterviewScheduled)
                {
                    // Interview Schedule Section
                    Panel interviewPanel = new Panel() { Location = new System.Drawing.Point(15, yPos), Width = 750, Height = 110, BackColor = System.Drawing.Color.FromArgb(240, 248, 255), BorderStyle = BorderStyle.FixedSingle };
                    
                    Label interviewTitleLabel = new Label() { Text = "📅 INTERVIEW SCHEDULED", Location = new System.Drawing.Point(10, 5), Width = 730, Font = new Font("Arial", 10, FontStyle.Bold), ForeColor = System.Drawing.Color.FromArgb(0, 102, 204) };
                    interviewPanel.Controls.Add(interviewTitleLabel);

                    try
                    {
                        var interview = _db.GetInterviewSchedule(applicationID);
                        if (interview != null)
                        {
                            // Combine date and time
                            DateTime interviewDateTime = interview.InterviewDate;
                            
                            // Handle InterviewTime which could be TimeSpan or DateTime from OleDb
                            TimeSpan interviewTime;
                            if (interview.InterviewTime is TimeSpan ts)
                            {
                                interviewTime = ts;
                            }
                            else if (interview.InterviewTime is DateTime dt)
                            {
                                interviewTime = dt.TimeOfDay;
                            }
                            else
                            {
                                // Fallback: try to parse as TimeSpan
                                interviewTime = TimeSpan.Zero;
                            }
                            
                            DateTime fullDateTime = interviewDateTime.Add(interviewTime);

                            Label dateTimeLabel = new Label() { Text = $"Date & Time: {fullDateTime:MMMM dd, yyyy 'at' HH:mm}", Location = new System.Drawing.Point(15, 25), Width = 720, Font = new Font("Arial", 9) };
                            interviewPanel.Controls.Add(dateTimeLabel);

                            Label interviewerLabel = new Label() { Text = $"Interviewer: {interview.Interviewer}", Location = new System.Drawing.Point(15, 45), Width = 720, Font = new Font("Arial", 9) };
                            interviewPanel.Controls.Add(interviewerLabel);

                            string modeText = "Meeting Link";
                            if (interview.Location?.ToString().Contains("http") == false && !interview.Location?.ToString().Contains("://"))
                                modeText = "Location";
                            
                            Label locationLabel = new Label() { Text = $"{modeText}: {interview.Location}", Location = new System.Drawing.Point(15, 65), Width = 720, Font = new Font("Arial", 9) };
                            interviewPanel.Controls.Add(locationLabel);
                        }
                        else
                        {
                            Label noScheduleLabel = new Label() { Text = "Interview details not yet available. Please check back soon.", Location = new System.Drawing.Point(15, 25), Width = 720, Font = new Font("Arial", 9, FontStyle.Italic), ForeColor = System.Drawing.Color.Gray };
                            interviewPanel.Controls.Add(noScheduleLabel);
                        }
                    }
                    catch (Exception ex)
                    {
                        Label errorLabel = new Label() { Text = $"Error loading interview details: {ex.Message}", Location = new System.Drawing.Point(15, 25), Width = 720, Font = new Font("Arial", 9), ForeColor = System.Drawing.Color.Red };
                        interviewPanel.Controls.Add(errorLabel);
                    }

                    mainPanel.Controls.Add(interviewPanel);
                    yPos += 120;
                }

                // Separator
                Panel separator = new Panel() { Location = new System.Drawing.Point(15, yPos), Width = 750, Height = 2, BackColor = System.Drawing.Color.LightGray };
                mainPanel.Controls.Add(separator);
                yPos += 15;

                // Required Documents Section
                Label requiredDocLabel = new Label() { Text = "Required Documents", Location = new System.Drawing.Point(15, yPos), Width = 300, Font = new Font("Arial", 11, FontStyle.Bold) };
                mainPanel.Controls.Add(requiredDocLabel);
                yPos += 30;

                try
                {
                    var requirements = _db.GetJobRequirements(appDetails.JobID);
                    var submittedDocs = _db.GetApplicantDocuments(applicant.ApplicantID, appDetails.JobID);
                    
                    if (requirements != null && requirements.Count > 0)
                    {
                        foreach (var req in requirements)
                        {
                            // Check if document has been submitted for this requirement
                            int reqTypeID = (int)req.RequirementTypeID;
                            bool isSubmitted = false;
                            
                            if (submittedDocs != null)
                            {
                                foreach (var doc in submittedDocs)
                                {
                                    if ((int)doc.RequirementTypeID == reqTypeID)
                                    {
                                        isSubmitted = true;
                                        break;
                                    }
                                }
                            }
                            
                            string statusText = isSubmitted ? "✓ Submitted" : "Not Submitted";
                            System.Drawing.Color statusColor = isSubmitted ? System.Drawing.Color.Green : System.Drawing.Color.Red;

                            // Requirement row panel
                            Panel reqRow = new Panel() { Location = new System.Drawing.Point(20, yPos), Width = 740, Height = 35, BorderStyle = BorderStyle.FixedSingle, BackColor = System.Drawing.Color.White };
                            
                            Label reqNameLabel = new Label() { Text = $"• {req.RequirementName}", Location = new System.Drawing.Point(5, 8), Width = 350, Font = new Font("Arial", 9) };
                            reqRow.Controls.Add(reqNameLabel);

                            Label reqStatusLabel = new Label() { Text = statusText, Location = new System.Drawing.Point(360, 8), Width = 120, Font = new Font("Arial", 9, FontStyle.Bold), ForeColor = statusColor };
                            reqRow.Controls.Add(reqStatusLabel);

                            // Submit/Unsubmit buttons only for Draft and Submitted statuses
                            if (isDraft || appDetails.Status == ApplicationStatus.Submitted)
                            {
                                Button submitReqBtn = new Button();
                                submitReqBtn.Text = isSubmitted ? "Unsubmit" : "Submit";
                                submitReqBtn.Size = new System.Drawing.Size(75, 25);
                                submitReqBtn.Location = new System.Drawing.Point(490, 5);
                                submitReqBtn.BackColor = isSubmitted ? System.Drawing.Color.FromArgb(169, 169, 169) : System.Drawing.Color.FromArgb(0, 102, 204);
                                submitReqBtn.ForeColor = System.Drawing.Color.White;
                                submitReqBtn.Font = new Font("Arial", 8, FontStyle.Bold);
                                submitReqBtn.Tag = reqTypeID;
                                
                                int capturedReqTypeID = reqTypeID;
                                submitReqBtn.Click += (s, e) =>
                                {
                                    try
                                    {
                                        bool docIsSubmitted = false;
                                        if (submittedDocs != null)
                                        {
                                            foreach (var doc in submittedDocs)
                                            {
                                                if ((int)doc.RequirementTypeID == capturedReqTypeID)
                                                {
                                                    docIsSubmitted = true;
                                                    break;
                                                }
                                            }
                                        }
                                        
                                        if (!docIsSubmitted)
                                        {
                                            // Submit document
                                            bool success = _db.SubmitApplicantDocument(applicant.ApplicantID, appDetails.JobID, capturedReqTypeID, "Submitted by applicant", "Submitted");
                                            if (success)
                                            {
                                                MessageBox.Show($"Document submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                detailsForm.Close();
                                            }
                                            else
                                            {
                                                MessageBox.Show("Failed to submit document.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                        else
                                        {
                                            // Delete document from database
                                            bool success = _db.DeleteApplicantDocument(applicant.ApplicantID, appDetails.JobID, capturedReqTypeID);
                                            if (success)
                                            {
                                                MessageBox.Show("Document unsubmitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                detailsForm.Close();
                                            }
                                            else
                                            {
                                                MessageBox.Show("Failed to unsubmit document.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                };
                                reqRow.Controls.Add(submitReqBtn);
                            }

                            mainPanel.Controls.Add(reqRow);
                            yPos += 40;
                        }
                    }
                    else
                    {
                        Label noReqLabel = new Label() { Text = "No specific documents required for this position.", Location = new System.Drawing.Point(20, yPos), Width = 740, Font = new Font("Arial", 9, FontStyle.Italic), ForeColor = System.Drawing.Color.Gray };
                        mainPanel.Controls.Add(noReqLabel);
                        yPos += 25;
                    }
                }
                catch (Exception ex)
                {
                    Label errorLabel = new Label() { Text = $"Error loading documents: {ex.Message}", Location = new System.Drawing.Point(20, yPos), Width = 740, Font = new Font("Arial", 9), ForeColor = System.Drawing.Color.Red };
                    mainPanel.Controls.Add(errorLabel);
                    yPos += 25;
                }

                // Separator after documents
                Panel separator2 = new Panel() { Location = new System.Drawing.Point(15, yPos), Width = 750, Height = 2, BackColor = System.Drawing.Color.LightGray };
                mainPanel.Controls.Add(separator2);
                yPos += 15;

                // Applicant Information
                Label applicantInfoLabel = new Label() { Text = "Applicant Information", Location = new System.Drawing.Point(15, yPos), Width = 300, Font = new Font("Arial", 11, FontStyle.Bold) };
                mainPanel.Controls.Add(applicantInfoLabel);
                yPos += 30;

                Label nameLabel = new Label() { Text = "Name:", Location = new System.Drawing.Point(15, yPos), Width = 200 };
                Label nameValue = new Label() { Text = $"{appDetails.FirstName} {appDetails.LastName}", Location = new System.Drawing.Point(250, yPos), Width = 500 };
                mainPanel.Controls.Add(nameLabel);
                mainPanel.Controls.Add(nameValue);
                yPos += 30;

                Label contactLabel = new Label() { Text = "Contact:", Location = new System.Drawing.Point(15, yPos), Width = 200 };
                Label contactValue = new Label() { Text = appDetails.ContactNo, Location = new System.Drawing.Point(250, yPos), Width = 500 };
                mainPanel.Controls.Add(contactLabel);
                mainPanel.Controls.Add(contactValue);
                yPos += 30;

                Label educationLabel = new Label() { Text = "Education:", Location = new System.Drawing.Point(15, yPos), Width = 200 };
                TextBox educationValue = new TextBox() { Text = appDetails.Education ?? "", Location = new System.Drawing.Point(15, yPos + 25), Width = 750, Height = 60, ReadOnly = true, Multiline = true, BackColor = System.Drawing.Color.WhiteSmoke };
                mainPanel.Controls.Add(educationLabel);
                mainPanel.Controls.Add(educationValue);
                yPos += 90;

                Label skillsLabel = new Label() { Text = "Skills:", Location = new System.Drawing.Point(15, yPos), Width = 200 };
                TextBox skillsValue = new TextBox() { Text = appDetails.Skills ?? "", Location = new System.Drawing.Point(15, yPos + 25), Width = 750, Height = 60, ReadOnly = true, Multiline = true, BackColor = System.Drawing.Color.WhiteSmoke };
                mainPanel.Controls.Add(skillsLabel);
                mainPanel.Controls.Add(skillsValue);
                yPos += 90;

                detailsForm.Controls.Add(mainPanel);

                // Create bottom button panel
                Panel bottomPanel = new Panel();
                bottomPanel.Height = 60;
                bottomPanel.Dock = DockStyle.Bottom;
                bottomPanel.BackColor = System.Drawing.Color.WhiteSmoke;
                bottomPanel.Padding = new Padding(10);

                // Draft-specific options
                if (isDraft)
                {
                    Button submitButton = new Button();
                    submitButton.Text = "Submit Application";
                    submitButton.Size = new System.Drawing.Size(150, 35);
                    submitButton.Location = new System.Drawing.Point(10, 10);
                    submitButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
                    submitButton.ForeColor = System.Drawing.Color.White;
                    submitButton.Font = new Font("Arial", 10, FontStyle.Bold);
                    submitButton.Click += (s, e) =>
                    {
                        if (MessageBox.Show("Are you sure you want to submit this application? You cannot edit it after submission.", "Confirm Submission", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            try
                            {
                                // Update status from Draft to Submitted
                                // Get applicant full name for the history record
                                var applicantInfo = _db.GetApplicantByUsername(_username);
                                string applicantFullName = applicantInfo != null ? $"{applicantInfo.FirstName} {applicantInfo.LastName}".Trim() : _username;
                                bool success = _db.UpdateApplicationStatus(applicationID, ApplicationStatus.Submitted, "Submitted by applicant", applicantFullName);
                                if (success)
                                {
                                    MessageBox.Show("Application submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    detailsForm.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Failed to submit application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error submitting application: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    };
                    bottomPanel.Controls.Add(submitButton);

                    Label draftLabel = new Label() { Text = "(Status: DRAFT - You can still make changes)", Location = new System.Drawing.Point(170, 15), ForeColor = System.Drawing.Color.Orange, Font = new Font("Arial", 9, FontStyle.Italic) };
                    bottomPanel.Controls.Add(draftLabel);
                }
                else if (appDetails.Status == ApplicationStatus.Submitted)
                {
                    Label submittedLabel = new Label() { Text = "(Status: SUBMITTED - Application received)", Location = new System.Drawing.Point(10, 15), ForeColor = System.Drawing.Color.Green, Font = new Font("Arial", 9, FontStyle.Italic) };
                    bottomPanel.Controls.Add(submittedLabel);
                }
                else
                {
                    Label readOnlyLabel = new Label() { Text = "(Status: " + appDetails.Status.ToUpper() + " - Read Only)", Location = new System.Drawing.Point(10, 15), ForeColor = System.Drawing.Color.Gray, Font = new Font("Arial", 10, FontStyle.Bold) };
                    bottomPanel.Controls.Add(readOnlyLabel);
                }

                Button historyButton = new Button();
                historyButton.Text = "View History";
                historyButton.Size = new System.Drawing.Size(120, 35);
                historyButton.Location = new System.Drawing.Point(560, 10);
                historyButton.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
                historyButton.ForeColor = System.Drawing.Color.White;
                historyButton.Font = new Font("Arial", 10, FontStyle.Bold);
                historyButton.Click += (s, e) => ShowApplicationStatusHistory(applicationID);
                bottomPanel.Controls.Add(historyButton);

                Button closeButton = new Button();
                closeButton.Text = "Close";
                closeButton.Size = new System.Drawing.Size(100, 35);
                closeButton.Location = new System.Drawing.Point(690, 10);
                closeButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
                closeButton.ForeColor = System.Drawing.Color.White;
                closeButton.Font = new Font("Arial", 10, FontStyle.Bold);
                closeButton.Click += (s, e) => detailsForm.Close();
                bottomPanel.Controls.Add(closeButton);

                detailsForm.Controls.Add(bottomPanel);

                detailsForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying application details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowApplicationStatusHistory(int applicationID)
        {
            try
            {
                var history = _db.GetApplicationStatusHistory(applicationID);
                
                if (history == null || history.Count == 0)
                {
                    MessageBox.Show("No status history available for this application.", "No History", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Create history form
                Form historyForm = new Form();
                historyForm.Text = $"Application Status History - Application #{applicationID}";
                historyForm.Size = new System.Drawing.Size(900, 500);
                historyForm.StartPosition = FormStartPosition.CenterParent;

                // Create ListView for history
                ListView historyListView = new ListView();
                historyListView.Dock = DockStyle.Fill;
                historyListView.View = View.Details;
                historyListView.FullRowSelect = true;
                historyListView.Padding = new Padding(10);

                // Add columns
                historyListView.Columns.Add("Date Changed", 150);
                historyListView.Columns.Add("Status", 150);
                historyListView.Columns.Add("Changed By", 150);
                historyListView.Columns.Add("Remarks", 350);

                // Add rows from history
                foreach (var record in history)
                {
                    string dateStr = record.DateChanged != null 
                        ? ((DateTime)record.DateChanged).ToString("yyyy-MM-dd HH:mm") 
                        : "N/A";
                    string status = record.Status?.ToString() ?? "Unknown";
                    string changedBy = record.ChangedBy?.ToString() ?? "System";
                    string remarks = record.Remarks?.ToString() ?? "";

                    ListViewItem item = new ListViewItem(dateStr);
                    item.SubItems.Add(status);
                    item.SubItems.Add(changedBy);
                    item.SubItems.Add(remarks);
                    historyListView.Items.Add(item);
                }

                historyForm.Controls.Add(historyListView);

                // Add close button
                Panel buttonPanel = new Panel();
                buttonPanel.Height = 50;
                buttonPanel.Dock = DockStyle.Bottom;
                buttonPanel.BackColor = System.Drawing.Color.WhiteSmoke;
                buttonPanel.Padding = new Padding(10);

                Button closeButton = new Button();
                closeButton.Text = "Close";
                closeButton.Size = new System.Drawing.Size(100, 35);
                closeButton.Location = new System.Drawing.Point(790, 10);
                closeButton.Click += (s, e) => historyForm.Close();
                buttonPanel.Controls.Add(closeButton);

                historyForm.Controls.Add(buttonPanel);

                historyForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading status history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void ViewEditApplicantProfile()
        {
            try
            {
                // Get applicant info
                var applicant = _db.GetApplicantByUsername(_username);
                
                if (applicant == null)
                {
                    MessageBox.Show("Error: Could not retrieve applicant profile.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create profile form
                Form profileForm = new Form();
                profileForm.Text = "My Profile";
                profileForm.Size = new System.Drawing.Size(650, 550);
                profileForm.StartPosition = FormStartPosition.CenterParent;
                profileForm.AutoScroll = true;

                // Create main panel
                Panel mainPanel = new Panel();
                mainPanel.Dock = DockStyle.Fill;
                mainPanel.AutoScroll = true;
                mainPanel.Padding = new Padding(15);

                // Store controls for switching between view and edit modes
                System.Collections.Generic.List<Control> viewControls = new System.Collections.Generic.List<Control>();
                System.Collections.Generic.List<TextBox> editTextBoxes = new System.Collections.Generic.List<TextBox>();

                bool isEditMode = false;

                Action<bool> SwitchMode = (editMode) =>
                {
                    isEditMode = editMode;
                    foreach (Control c in viewControls)
                        c.Visible = !editMode;
                    foreach (TextBox t in editTextBoxes)
                        t.Visible = editMode;
                };

                int yPos = 10;

                // Title
                Label titleLabel = new Label() { Text = "Personal Information", Location = new System.Drawing.Point(15, yPos), Width = 300, Font = new Font("Arial", 12, FontStyle.Bold) };
                mainPanel.Controls.Add(titleLabel);
                yPos += 35;

                // First Name
                Label firstNameLabel = new Label() { Text = "First Name:", Location = new System.Drawing.Point(15, yPos), Width = 150 };
                Label firstNameValue = new Label() { Text = applicant.FirstName ?? "", Location = new System.Drawing.Point(180, yPos), Width = 380 };
                TextBox firstNameEdit = new TextBox() { Text = applicant.FirstName ?? "", Location = new System.Drawing.Point(180, yPos), Width = 380, Visible = false };
                mainPanel.Controls.Add(firstNameLabel);
                mainPanel.Controls.Add(firstNameValue);
                mainPanel.Controls.Add(firstNameEdit);
                viewControls.Add(firstNameValue);
                editTextBoxes.Add(firstNameEdit);
                yPos += 30;

                // Last Name
                Label lastNameLabel = new Label() { Text = "Last Name:", Location = new System.Drawing.Point(15, yPos), Width = 150 };
                Label lastNameValue = new Label() { Text = applicant.LastName ?? "", Location = new System.Drawing.Point(180, yPos), Width = 380 };
                TextBox lastNameEdit = new TextBox() { Text = applicant.LastName ?? "", Location = new System.Drawing.Point(180, yPos), Width = 380, Visible = false };
                mainPanel.Controls.Add(lastNameLabel);
                mainPanel.Controls.Add(lastNameValue);
                mainPanel.Controls.Add(lastNameEdit);
                viewControls.Add(lastNameValue);
                editTextBoxes.Add(lastNameEdit);
                yPos += 30;

                // Contact
                Label contactLabel = new Label() { Text = "Contact Number:", Location = new System.Drawing.Point(15, yPos), Width = 150 };
                Label contactValue = new Label() { Text = applicant.ContactNo ?? "", Location = new System.Drawing.Point(180, yPos), Width = 380 };
                TextBox contactEdit = new TextBox() { Text = applicant.ContactNo ?? "", Location = new System.Drawing.Point(180, yPos), Width = 380, Visible = false };
                mainPanel.Controls.Add(contactLabel);
                mainPanel.Controls.Add(contactValue);
                mainPanel.Controls.Add(contactEdit);
                viewControls.Add(contactValue);
                editTextBoxes.Add(contactEdit);
                yPos += 30;

                // Address
                Label addressLabel = new Label() { Text = "Address:", Location = new System.Drawing.Point(15, yPos), Width = 150 };
                mainPanel.Controls.Add(addressLabel);
                yPos += 25;

                TextBox addressValue = new TextBox() { Text = applicant.Address ?? "", Location = new System.Drawing.Point(15, yPos), Width = 590, Height = 60, ReadOnly = true, Multiline = true, BackColor = System.Drawing.Color.WhiteSmoke };
                TextBox addressEdit = new TextBox() { Text = applicant.Address ?? "", Location = new System.Drawing.Point(15, yPos), Width = 590, Height = 60, Multiline = true, Visible = false };
                mainPanel.Controls.Add(addressValue);
                mainPanel.Controls.Add(addressEdit);
                viewControls.Add(addressValue);
                editTextBoxes.Add(addressEdit);
                yPos += 70;

                // Education
                Label educationLabel = new Label() { Text = "Education:", Location = new System.Drawing.Point(15, yPos), Width = 150 };
                mainPanel.Controls.Add(educationLabel);
                yPos += 25;

                TextBox educationValue = new TextBox() { Text = applicant.Education ?? "", Location = new System.Drawing.Point(15, yPos), Width = 590, Height = 60, ReadOnly = true, Multiline = true, BackColor = System.Drawing.Color.WhiteSmoke };
                TextBox educationEdit = new TextBox() { Text = applicant.Education ?? "", Location = new System.Drawing.Point(15, yPos), Width = 590, Height = 60, Multiline = true, Visible = false };
                mainPanel.Controls.Add(educationValue);
                mainPanel.Controls.Add(educationEdit);
                viewControls.Add(educationValue);
                editTextBoxes.Add(educationEdit);
                yPos += 70;

                // Skills
                Label skillsLabel = new Label() { Text = "Skills:", Location = new System.Drawing.Point(15, yPos), Width = 150 };
                mainPanel.Controls.Add(skillsLabel);
                yPos += 25;

                TextBox skillsValue = new TextBox() { Text = applicant.Skills ?? "", Location = new System.Drawing.Point(15, yPos), Width = 590, Height = 60, ReadOnly = true, Multiline = true, BackColor = System.Drawing.Color.WhiteSmoke };
                TextBox skillsEdit = new TextBox() { Text = applicant.Skills ?? "", Location = new System.Drawing.Point(15, yPos), Width = 590, Height = 60, Multiline = true, Visible = false };
                mainPanel.Controls.Add(skillsValue);
                mainPanel.Controls.Add(skillsEdit);
                viewControls.Add(skillsValue);
                editTextBoxes.Add(skillsEdit);

                profileForm.Controls.Add(mainPanel);

                // Create bottom button panel
                Panel bottomPanel = new Panel();
                bottomPanel.Height = 60;
                bottomPanel.Dock = DockStyle.Bottom;
                bottomPanel.BackColor = System.Drawing.Color.WhiteSmoke;
                bottomPanel.Padding = new Padding(10);

                Label infoLabel = new Label() { Text = "View your current profile information", Location = new System.Drawing.Point(10, 15), ForeColor = System.Drawing.Color.Gray, Font = new Font("Arial", 9, FontStyle.Italic), Name = "InfoLabel" };
                bottomPanel.Controls.Add(infoLabel);

                Button editButton = new Button();
                editButton.Size = new System.Drawing.Size(100, 35);
                editButton.Location = new System.Drawing.Point(380, 10);
                editButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
                editButton.ForeColor = System.Drawing.Color.White;
                editButton.Font = new Font("Arial", 10, FontStyle.Bold);
                bottomPanel.Controls.Add(editButton);

                Button saveButton = new Button();
                saveButton.Text = "Save";
                saveButton.Size = new System.Drawing.Size(100, 35);
                saveButton.Location = new System.Drawing.Point(380, 10);
                saveButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
                saveButton.ForeColor = System.Drawing.Color.White;
                saveButton.Font = new Font("Arial", 10, FontStyle.Bold);
                saveButton.Visible = false;
                bottomPanel.Controls.Add(saveButton);

                Button cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.Size = new System.Drawing.Size(100, 35);
                cancelButton.Location = new System.Drawing.Point(490, 10);
                cancelButton.BackColor = System.Drawing.Color.FromArgb(169, 169, 169);
                cancelButton.ForeColor = System.Drawing.Color.White;
                cancelButton.Font = new Font("Arial", 10, FontStyle.Bold);
                cancelButton.Visible = false;
                bottomPanel.Controls.Add(cancelButton);

                // Now set up the click handlers
                editButton.Text = "Edit";
                editButton.Click += (s, e) =>
                {
                    SwitchMode(true);
                    infoLabel.Text = "Edit your profile information";
                    editButton.Visible = false;
                    saveButton.Visible = true;
                    cancelButton.Visible = true;
                };

                saveButton.Click += (s, e) =>
                {
                    try
                    {
                        // Update applicant object with new values
                        applicant.FirstName = firstNameEdit.Text;
                        applicant.LastName = lastNameEdit.Text;
                        applicant.ContactNo = contactEdit.Text;
                        applicant.Address = addressEdit.Text;
                        applicant.Education = educationEdit.Text;
                        applicant.Skills = skillsEdit.Text;

                        // Save to database
                        bool success = _db.UpdateApplicantInfo(_username, applicant);
                        if (success)
                        {
                            MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            // Update display labels
                            firstNameValue.Text = applicant.FirstName ?? "";
                            lastNameValue.Text = applicant.LastName ?? "";
                            contactValue.Text = applicant.ContactNo ?? "";
                            addressValue.Text = applicant.Address ?? "";
                            educationValue.Text = applicant.Education ?? "";
                            skillsValue.Text = applicant.Skills ?? "";

                            // Switch back to view mode
                            SwitchMode(false);
                            infoLabel.Text = "View your current profile information";
                            editButton.Visible = true;
                            saveButton.Visible = false;
                            cancelButton.Visible = false;
                        }
                        else
                        {
                            MessageBox.Show("Failed to update profile. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving profile: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                cancelButton.Click += (s, e) =>
                {
                    SwitchMode(false);
                    infoLabel.Text = "View your current profile information";
                    editButton.Visible = true;
                    saveButton.Visible = false;
                    cancelButton.Visible = false;
                };

                Button closeButton = new Button();
                closeButton.Text = "Close";
                closeButton.Size = new System.Drawing.Size(100, 35);
                closeButton.Location = new System.Drawing.Point(540, 10);
                closeButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
                closeButton.ForeColor = System.Drawing.Color.White;
                closeButton.Font = new Font("Arial", 10, FontStyle.Bold);
                closeButton.Click += (s, e) => profileForm.Close();
                bottomPanel.Controls.Add(closeButton);

                profileForm.Controls.Add(bottomPanel);

                profileForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error viewing profile: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenApplicationsForm()
        {
            try
            {
                ApplicationManagementForm form = new ApplicationManagementForm(_db, _currentUser.RoleID, "All Statuses", _currentUser.Username);
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
                ApplicationManagementForm form = new ApplicationManagementForm(_db, _currentUser.RoleID, statusFilter, _currentUser.Username);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening applications: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenJobVacancyManagementForm()
        {
            try
            {
                JobVacancyManagementForm form = new JobVacancyManagementForm(_db, _username);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening job vacancy management: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeCredentials()
        {
            try
            {
                ChangeCredentialsDialog dialog = new ChangeCredentialsDialog(_username);
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    MessageBox.Show("Your credentials have been updated. Please log in again with your new credentials.", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LogoutUser();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing credentials: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
