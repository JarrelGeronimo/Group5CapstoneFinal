using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using ApplicationModel = HRAndApplicantSystem.Models.Application;
using HRAndApplicantSystem.Services;

namespace HRAndApplicantSystem.Forms
{
    /// <summary>
    /// Application Management Form - HR reviews and manages job applications
    /// 
    /// ARCHITECTURE: UI Layer Only
    /// - Shows applications in DataGridView (read-only applicant info)
    /// - HR can filter by status (pending, screening, interview, etc.)
    /// - HR can view applicant profile (read-only)
    /// - HR can change application status
    /// - HR can review submitted documents
    /// - NO editing of applicant personal information
    /// </summary>
    public partial class ApplicationManagementForm : Form
    {
        private readonly DatabaseHelper _db;
        private readonly int _userRoleID;
        private readonly string _username;
        private string _defaultStatusFilter = "All Statuses"; // Default filter
        private List<dynamic>? _allApplications;
        private DataGridView? applicationsDataGridView;
        private TextBox? searchTextBox;
        private Label? statusLabel;
        private Button? viewApplicantButton;
        private Button? viewDocumentsButton;
        private Button? changeStatusButton;
        private Button? refreshButton;
        private Panel? filterPanel;
        private Label? titleLabel;
        private Label? searchLabel;
        private Panel? buttonPanel;
        private Button? closeButton;

        public ApplicationManagementForm(DatabaseHelper db, int userRoleID = 0, string initialStatusFilter = "All Statuses", string username = "HR")
        {
            InitializeComponent();
            _db = db;
            _userRoleID = userRoleID;
            _username = username;
            _defaultStatusFilter = initialStatusFilter;
            this.Text = "Application Management";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(1400, 700);
        }

        private void ApplicationManagementForm_Load(object? sender, EventArgs? e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[ApplicationManagementForm_Load] Starting, defaultStatusFilter={_defaultStatusFilter}");
                
                // Debug: Check what's in the database
                _db.DebugDatabaseStatus();
                
                LoadApplications();
                
                // Apply the initial status filter if one was specified
                if (_defaultStatusFilter != "All Statuses")
                {
                    System.Diagnostics.Debug.WriteLine($"[ApplicationManagementForm_Load] Applying filter for status: {_defaultStatusFilter}");
                    ApplyAllFilters();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApplicationManagementForm_Load] Exception: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Error loading applications: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void LoadApplications()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[LoadApplications] Getting all applications from database");
                // Debug: Check database status first
                _db.DebugDatabaseStatus();
                
                // Get all applications from database
                _allApplications = _db.GetAllApplications();
                
                System.Diagnostics.Debug.WriteLine($"[LoadApplications] Retrieved {_allApplications?.Count ?? 0} applications");
                
                if (_allApplications == null || _allApplications.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("[LoadApplications] WARNING: No applications retrieved from database!");
                    statusLabel!.Text = "No applications found in database";
                    return;
                }
                
                // Log the statuses of retrieved applications
                var statusCounts = _allApplications
                    .GroupBy(a => a.Status?.ToString() ?? "unknown")
                    .ToDictionary(g => g.Key, g => g.Count());
                    
                System.Diagnostics.Debug.WriteLine("[LoadApplications] Applications by status:");
                foreach (var kvp in statusCounts)
                {
                    System.Diagnostics.Debug.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }
                
                // Populate DataGridView - only populate grid if showing all statuses
                if (_defaultStatusFilter == "All Statuses")
                {
                    PopulateDataGridView(_allApplications ?? new List<dynamic>());
                    statusLabel!.Text = $"All Applications: {_allApplications?.Count ?? 0} total";
                }
                else
                {
                    // For filtered views, don't populate yet - ApplyAllFilters will do it
                    System.Diagnostics.Debug.WriteLine($"[LoadApplications] Skipping initial population - will apply filter '{_defaultStatusFilter}'");
                }
                
                System.Diagnostics.Debug.WriteLine($"[LoadApplications] LoadApplications completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadApplications] Exception: {ex.Message}\n{ex.StackTrace}");
                statusLabel!.Text = $"Error: {ex.Message}";
                _allApplications = new List<dynamic>();
            }
        }

        private void PopulateDataGridView(List<dynamic> applications)
        {
            System.Diagnostics.Debug.WriteLine($"[PopulateDataGridView] Called with {applications?.Count ?? 0} applications");
            
            // Clear existing columns
            applicationsDataGridView!.Columns.Clear();
            
            // Create columns explicitly for the dynamic object properties
            applicationsDataGridView.Columns.Add("ApplicationID", "Application ID");
            applicationsDataGridView.Columns.Add("ApplicantID", "Applicant ID");
            applicationsDataGridView.Columns.Add("JobID", "Job ID");
            applicationsDataGridView.Columns.Add("FirstName", "First Name");
            applicationsDataGridView.Columns.Add("LastName", "Last Name");
            applicationsDataGridView.Columns.Add("JobTitle", "Job Title");
            applicationsDataGridView.Columns.Add("Status", "Status");
            applicationsDataGridView.Columns.Add("DateApplied", "Date Applied");
            
            // Populate rows
            int rowCount = 0;
            foreach (var app in applications ?? new List<dynamic>())
            {
                applicationsDataGridView.Rows.Add(
                    app.ApplicationID,
                    app.ApplicantID,
                    app.JobID,
                    app.FirstName,
                    app.LastName,
                    app.JobTitle,
                    app.Status,
                    app.DateApplied
                );
                System.Diagnostics.Debug.WriteLine($"[PopulateDataGridView] Added row: {app.FirstName} {app.LastName} - {app.JobTitle} (Status: {app.Status})");
                rowCount++;
            }
            
            // Configure grid appearance
            applicationsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            applicationsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            applicationsDataGridView.AllowUserToAddRows = false;
            applicationsDataGridView.AllowUserToDeleteRows = false;
            applicationsDataGridView.ReadOnly = true;
            applicationsDataGridView.CellDoubleClick += ApplicationsDataGridView_CellDoubleClick;
            System.Diagnostics.Debug.WriteLine($"[PopulateDataGridView] DataGridView now has {applicationsDataGridView.Rows.Count} rows");
        }

        private void ApplicationsDataGridView_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ViewApplicantProfile();
            }
        }

        private void FilterApplications()
        {
            if (_allApplications == null) return;

            // Removed status filter - applications are pre-filtered by workflow stage
            // Only search filter is now used
            ApplyAllFilters();
        }

        private void SearchTextBox_TextChanged(object? sender, EventArgs? e)
        {
            ApplyAllFilters();
        }

        private void ApplyAllFilters()
        {
            System.Diagnostics.Debug.WriteLine("[ApplyAllFilters] Called");
            if (_allApplications == null) 
            {
                System.Diagnostics.Debug.WriteLine("[ApplyAllFilters] _allApplications is null, returning");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"[ApplyAllFilters] Starting with {_allApplications.Count} total applications");
            
            // Log all statuses in the data
            var statuses = _allApplications.Select(a => a.Status?.ToString() ?? "").Distinct().ToList();
            System.Diagnostics.Debug.WriteLine($"[ApplyAllFilters] Available statuses in data: {string.Join(", ", statuses)}");

            var filtered = _allApplications.AsEnumerable();

            // Filter by default status if one was specified during form initialization
            if (_defaultStatusFilter != "All Statuses")
            {
                System.Diagnostics.Debug.WriteLine($"[ApplyAllFilters] Filtering by filter mode: '{_defaultStatusFilter}'");
                int beforeCount = filtered.Count();
                
                // Handle special filter modes that include multiple statuses
                if (_defaultStatusFilter.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                {
                    // "Pending" mode shows both Submitted and Under Review applications
                    filtered = filtered.Where(a =>
                    {
                        string appStatus = (a.Status?.ToString() ?? "").Trim();
                        bool matches = appStatus.Equals("Submitted", StringComparison.OrdinalIgnoreCase) || 
                                      appStatus.Equals("Under Review", StringComparison.OrdinalIgnoreCase);
                        return matches;
                    });
                    System.Diagnostics.Debug.WriteLine($"[ApplyAllFilters] Using 'Pending' mode - showing Submitted and Under Review applications");
                }
                else if (_defaultStatusFilter.Equals("Shortlisted", StringComparison.OrdinalIgnoreCase))
                {
                    // "Shortlisted" mode shows Shortlisted applications (legacy "Screening" also supported)
                    filtered = filtered.Where(a =>
                    {
                        string appStatus = (a.Status?.ToString() ?? "").Trim();
                        bool matches = appStatus.Equals("Shortlisted", StringComparison.OrdinalIgnoreCase) ||
                                      appStatus.Equals("Screening", StringComparison.OrdinalIgnoreCase);
                        return matches;
                    });
                    System.Diagnostics.Debug.WriteLine($"[ApplyAllFilters] Using 'Shortlisted' mode - showing Shortlisted and legacy Screening applications");
                }
                else if (_defaultStatusFilter.Equals("Interview Scheduled", StringComparison.OrdinalIgnoreCase))
                {
                    // "Interview Scheduled" mode shows Interview Scheduled applications (legacy "Interview" also supported)
                    filtered = filtered.Where(a =>
                    {
                        string appStatus = (a.Status?.ToString() ?? "").Trim();
                        bool matches = appStatus.Equals("Interview Scheduled", StringComparison.OrdinalIgnoreCase) ||
                                      appStatus.Equals("Interview", StringComparison.OrdinalIgnoreCase);
                        return matches;
                    });
                    System.Diagnostics.Debug.WriteLine($"[ApplyAllFilters] Using 'Interview Scheduled' mode - showing Interview Scheduled and legacy Interview applications");
                }
                else
                {
                    // Standard single-status filter
                    filtered = filtered.Where(a =>
                    {
                        string appStatus = (a.Status?.ToString() ?? "").Trim();
                        bool matches = appStatus.Equals(_defaultStatusFilter, StringComparison.OrdinalIgnoreCase);
                        if (matches)
                        {
                            System.Diagnostics.Debug.WriteLine($"[ApplyAllFilters]   MATCH: App status '{appStatus}' matches filter '{_defaultStatusFilter}'");
                        }
                        return matches;
                    });
                }
                
                int afterCount = filtered.Count();
                System.Diagnostics.Debug.WriteLine($"[ApplyAllFilters] After status filter: {beforeCount} -> {afterCount} applications");
            }

            // Filter by search text (applicant name or job title)
            string searchTerm = searchTextBox!.Text.Trim();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                System.Diagnostics.Debug.WriteLine($"[ApplyAllFilters] Filtering by search term: {searchTerm}");
                filtered = filtered.Where(a =>
                    (a.FirstName?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (a.LastName?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (a.JobTitle?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }

            var filteredList = filtered.ToList();
            System.Diagnostics.Debug.WriteLine($"[ApplyAllFilters] Final result: {filteredList.Count} applications");
            foreach (var app in filteredList.Take(5)) // Log first 5 for verification
            {
                System.Diagnostics.Debug.WriteLine($"  - {app.FirstName} {app.LastName}, Job: {app.JobTitle}, Status: {app.Status}");
            }
            
            PopulateDataGridView(filteredList);
            
            // Update status label based on filter
            string statusText;
            if (_defaultStatusFilter == "Pending")
            {
                statusText = $"Pending Review (Submitted & Under Review): {filteredList.Count} applications";
            }
            else if (_defaultStatusFilter == "All Statuses")
            {
                statusText = $"All Applications: {filteredList.Count} applications";
            }
            else
            {
                statusText = $"Filter: {_defaultStatusFilter} - {filteredList.Count} applications";
            }
            statusLabel!.Text = statusText;
        }

        private void ViewApplicantProfile()
        {
            if (applicationsDataGridView!.SelectedRows.Count > 0)
            {
                try
                {
                    var row = applicationsDataGridView.SelectedRows[0];
                    int applicantId = Convert.ToInt32(row.Cells["ApplicantID"]?.Value ?? 0);
                    
                    if (applicantId <= 0)
                    {
                        MessageBox.Show("Invalid applicant ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Get applicant info
                    var applicant = _db.GetApplicantByID(applicantId);
                    
                    if (applicant == null)
                    {
                        MessageBox.Show("Could not retrieve applicant information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Create profile form
                    Form profileForm = new Form();
                    profileForm.Text = "View Applicant Profile";
                    profileForm.Size = new System.Drawing.Size(650, 550);
                    profileForm.StartPosition = FormStartPosition.CenterParent;
                    profileForm.AutoScroll = true;

                    // Create main panel
                    Panel mainPanel = new Panel();
                    mainPanel.Dock = DockStyle.Fill;
                    mainPanel.AutoScroll = true;
                    mainPanel.Padding = new Padding(15);

                    int yPos = 10;

                    // Title
                    Label titleLabel = new Label() { Text = "Applicant Information (Read-Only)", Location = new System.Drawing.Point(15, yPos), Width = 300, Font = new Font("Arial", 12, FontStyle.Bold) };
                    mainPanel.Controls.Add(titleLabel);
                    yPos += 35;

                    // First Name
                    Label firstNameLabel = new Label() { Text = "First Name:", Location = new System.Drawing.Point(15, yPos), Width = 150 };
                    Label firstNameValue = new Label() { Text = applicant.FirstName ?? "", Location = new System.Drawing.Point(180, yPos), Width = 380 };
                    mainPanel.Controls.Add(firstNameLabel);
                    mainPanel.Controls.Add(firstNameValue);
                    yPos += 30;

                    // Last Name
                    Label lastNameLabel = new Label() { Text = "Last Name:", Location = new System.Drawing.Point(15, yPos), Width = 150 };
                    Label lastNameValue = new Label() { Text = applicant.LastName ?? "", Location = new System.Drawing.Point(180, yPos), Width = 380 };
                    mainPanel.Controls.Add(lastNameLabel);
                    mainPanel.Controls.Add(lastNameValue);
                    yPos += 30;

                    // Contact
                    Label contactLabel = new Label() { Text = "Contact Number:", Location = new System.Drawing.Point(15, yPos), Width = 150 };
                    Label contactValue = new Label() { Text = applicant.ContactNo ?? "", Location = new System.Drawing.Point(180, yPos), Width = 380 };
                    mainPanel.Controls.Add(contactLabel);
                    mainPanel.Controls.Add(contactValue);
                    yPos += 30;

                    // Address
                    Label addressLabel = new Label() { Text = "Address:", Location = new System.Drawing.Point(15, yPos), Width = 150 };
                    mainPanel.Controls.Add(addressLabel);
                    yPos += 25;

                    TextBox addressValue = new TextBox() { Text = applicant.Address ?? "", Location = new System.Drawing.Point(15, yPos), Width = 590, Height = 60, ReadOnly = true, Multiline = true, BackColor = System.Drawing.Color.WhiteSmoke };
                    mainPanel.Controls.Add(addressValue);
                    yPos += 70;

                    // Education
                    Label educationLabel = new Label() { Text = "Education:", Location = new System.Drawing.Point(15, yPos), Width = 150 };
                    mainPanel.Controls.Add(educationLabel);
                    yPos += 25;

                    TextBox educationValue = new TextBox() { Text = applicant.Education ?? "", Location = new System.Drawing.Point(15, yPos), Width = 590, Height = 60, ReadOnly = true, Multiline = true, BackColor = System.Drawing.Color.WhiteSmoke };
                    mainPanel.Controls.Add(educationValue);
                    yPos += 70;

                    // Skills
                    Label skillsLabel = new Label() { Text = "Skills:", Location = new System.Drawing.Point(15, yPos), Width = 150 };
                    mainPanel.Controls.Add(skillsLabel);
                    yPos += 25;

                    TextBox skillsValue = new TextBox() { Text = applicant.Skills ?? "", Location = new System.Drawing.Point(15, yPos), Width = 590, Height = 60, ReadOnly = true, Multiline = true, BackColor = System.Drawing.Color.WhiteSmoke };
                    mainPanel.Controls.Add(skillsValue);

                    profileForm.Controls.Add(mainPanel);

                    // Create bottom button panel
                    Panel bottomPanel = new Panel();
                    bottomPanel.Height = 60;
                    bottomPanel.Dock = DockStyle.Bottom;
                    bottomPanel.BackColor = System.Drawing.Color.WhiteSmoke;
                    bottomPanel.Padding = new Padding(10);

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
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an application first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ViewDocuments()
        {
            if (applicationsDataGridView!.SelectedRows.Count > 0)
            {
                try
                {
                    var row = applicationsDataGridView.SelectedRows[0];
                    int applicationId = Convert.ToInt32(row.Cells["ApplicationID"]?.Value ?? 0);
                    int applicantId = Convert.ToInt32(row.Cells["ApplicantID"]?.Value ?? 0);
                    int jobId = Convert.ToInt32(row.Cells["JobID"]?.Value ?? 0);
                    string jobTitle = row.Cells["JobTitle"]?.Value?.ToString() ?? "Unknown";
                    
                    if (applicationId <= 0 || applicantId <= 0 || jobId <= 0)
                    {
                        MessageBox.Show("Invalid application data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Get submitted documents
                    var documents = _db.GetApplicantDocuments(applicantId, jobId);
                    
                    // Get all job requirements
                    var requirements = _db.GetJobRequirements(jobId);

                    // Create documents form
                    Form documentsForm = new Form();
                    documentsForm.Text = "View Submitted Documents";
                    documentsForm.Size = new System.Drawing.Size(700, 500);
                    documentsForm.StartPosition = FormStartPosition.CenterParent;
                    documentsForm.AutoScroll = true;

                    // Create main panel
                    Panel mainPanel = new Panel();
                    mainPanel.Dock = DockStyle.Fill;
                    mainPanel.AutoScroll = true;
                    mainPanel.Padding = new Padding(15);

                    int yPos = 10;

                    // Title
                    Label titleLabel = new Label() { Text = "Submitted Documents (Read-Only)", Location = new System.Drawing.Point(15, yPos), Width = 500, Font = new Font("Arial", 12, FontStyle.Bold) };
                    mainPanel.Controls.Add(titleLabel);
                    yPos += 35;

                    // Job Title
                    Label jobTitleLabel = new Label() { Text = "Job Title:", Location = new System.Drawing.Point(15, yPos), Width = 150, Font = new Font("Arial", 10, FontStyle.Bold) };
                    Label jobTitleValue = new Label() { Text = jobTitle, Location = new System.Drawing.Point(180, yPos), Width = 450 };
                    mainPanel.Controls.Add(jobTitleLabel);
                    mainPanel.Controls.Add(jobTitleValue);
                    yPos += 35;

                    // Application ID
                    Label appIdLabel = new Label() { Text = "Application ID:", Location = new System.Drawing.Point(15, yPos), Width = 150, Font = new Font("Arial", 10, FontStyle.Bold) };
                    Label appIdValue = new Label() { Text = applicationId.ToString(), Location = new System.Drawing.Point(180, yPos), Width = 450 };
                    mainPanel.Controls.Add(appIdLabel);
                    mainPanel.Controls.Add(appIdValue);
                    yPos += 35;

                    // Separator
                    Label separatorLabel = new Label() { Text = new string('─', 80), Location = new System.Drawing.Point(15, yPos), Width = 650 };
                    mainPanel.Controls.Add(separatorLabel);
                    yPos += 25;

                    // Documents List
                    Label documentsHeader = new Label() { Text = "Required Documents:", Location = new System.Drawing.Point(15, yPos), Width = 300, Font = new Font("Arial", 10, FontStyle.Bold) };
                    mainPanel.Controls.Add(documentsHeader);
                    yPos += 30;

                    if (requirements == null || requirements.Count == 0)
                    {
                        Label noReqLabel = new Label() { Text = "No requirements defined for this job.", Location = new System.Drawing.Point(15, yPos), Width = 600, ForeColor = System.Drawing.Color.Gray };
                        mainPanel.Controls.Add(noReqLabel);
                    }
                    else
                    {
                        foreach (var req in requirements)
                        {
                            int reqTypeId = req.RequirementTypeID;
                            string reqName = req.RequirementName ?? "Unknown";

                            // Check if this requirement has been submitted
                            var submittedDoc = documents?.FirstOrDefault(d => d.RequirementTypeID == reqTypeId);
                            bool isSubmitted = submittedDoc != null;

                            // Create a panel for each requirement
                            Panel reqPanel = new Panel();
                            reqPanel.Location = new System.Drawing.Point(15, yPos);
                            reqPanel.Size = new System.Drawing.Size(650, 45);
                            reqPanel.BackColor = isSubmitted ? System.Drawing.Color.FromArgb(200, 255, 200) : System.Drawing.Color.FromArgb(255, 200, 200);
                            reqPanel.BorderStyle = BorderStyle.FixedSingle;

                            Label reqNameLabel = new Label() { Text = "• " + reqName, Location = new System.Drawing.Point(10, 5), Width = 450, Font = new Font("Arial", 9) };
                            Label statusLabel = new Label() { Text = isSubmitted ? "✓ Submitted" : "✗ Not Submitted", Location = new System.Drawing.Point(470, 5), Width = 160, Font = new Font("Arial", 9, FontStyle.Bold), ForeColor = isSubmitted ? System.Drawing.Color.FromArgb(0, 100, 0) : System.Drawing.Color.FromArgb(200, 0, 0) };

                            if (isSubmitted)
                            {
                                Label statusTextLabel = new Label() { Text = $"Status: {submittedDoc?.DocumentStatus ?? "Submitted"}", Location = new System.Drawing.Point(10, 22), Width = 620, Font = new Font("Arial", 8), ForeColor = System.Drawing.Color.DarkGreen };
                                reqPanel.Controls.Add(statusTextLabel);
                            }

                            reqPanel.Controls.Add(reqNameLabel);
                            reqPanel.Controls.Add(statusLabel);

                            mainPanel.Controls.Add(reqPanel);
                            yPos += 55;
                        }
                    }

                    documentsForm.Controls.Add(mainPanel);

                    // Create bottom button panel
                    Panel bottomPanel = new Panel();
                    bottomPanel.Height = 60;
                    bottomPanel.Dock = DockStyle.Bottom;
                    bottomPanel.BackColor = System.Drawing.Color.WhiteSmoke;
                    bottomPanel.Padding = new Padding(10);

                    Button closeButton = new Button();
                    closeButton.Text = "Close";
                    closeButton.Size = new System.Drawing.Size(100, 35);
                    closeButton.Location = new System.Drawing.Point(590, 10);
                    closeButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
                    closeButton.ForeColor = System.Drawing.Color.White;
                    closeButton.Font = new Font("Arial", 10, FontStyle.Bold);
                    closeButton.Click += (s, e) => documentsForm.Close();
                    bottomPanel.Controls.Add(closeButton);

                    documentsForm.Controls.Add(bottomPanel);

                    documentsForm.ShowDialog(this);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an application first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ChangeStatus()
        {
            if (applicationsDataGridView!.SelectedRows.Count > 0)
            {
                try
                {
                    var row = applicationsDataGridView.SelectedRows[0];
                    int applicationId = Convert.ToInt32(row.Cells["ApplicationID"]?.Value ?? 0);
                    string currentStatus = row.Cells["Status"]?.Value?.ToString() ?? "";
                    
                    System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Called for ApplicationID={applicationId}, CurrentStatus={currentStatus}");

                    // Route to appropriate dialog based on current status and user role
                    DialogResult result = DialogResult.Cancel;
                    string newStatus = "";

                    switch (currentStatus)
                    {
                        case "Submitted":
                        case "Under Review":
                            // Stage 1: Application Review (HR Staff, Manager, Admin can do)
                            // Both "Submitted" (new) and "Under Review" (in progress) go to review dialog
                            System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Opening ApplicationReviewDialog for review");
                            
                            // First, update status to "Under Review" to indicate HR is reviewing it
                            if (currentStatus == "Submitted")
                            {
                                System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Changing status from 'Submitted' to 'Under Review'");
                                _db.UpdateApplicationStatus(applicationId, "Under Review", "HR started reviewing application", _userRoleID.ToString());
                            }
                            
                            using (ApplicationReviewDialog dialog = new ApplicationReviewDialog(_db, applicationId, _username))
                            {
                                result = dialog.ShowDialog(this);
                                newStatus = dialog.ReviewDecision;
                                System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Review dialog closed with decision: {newStatus}");
                            }
                            break;

                        case "Screening":
                        case "Shortlisted":
                            // Stage 2a: Schedule Interview (HR Staff, Manager, Admin can do)
                            System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Opening InterviewSchedulingDialog for scheduling");
                            using (InterviewSchedulingDialog dialog = new InterviewSchedulingDialog(_db, applicationId))
                            {
                                result = dialog.ShowDialog(this);
                                if (result == DialogResult.OK)
                                {
                                    newStatus = "Interview Scheduled";
                                    System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Interview scheduled, status changed to Interview Scheduled");
                                }
                            }
                            break;

                        case "Interview Scheduled":
                            // Stage 2b: Evaluate Interview (HR Staff, Manager, Admin can do)
                            System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Opening InterviewDialog for evaluation");
                            using (InterviewDialog dialog = new InterviewDialog(_db, applicationId))
                            {
                                result = dialog.ShowDialog(this);
                                newStatus = dialog.InterviewDecision;
                                System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Interview evaluation dialog closed with decision: {newStatus}");
                            }
                            break;

                        case "For Final Review":
                            // Final Review: Manager and Admin ONLY - Accept or Reject decision
                            if (_userRoleID == 3 || _userRoleID == 4) // HR_MANAGER or ADMIN
                            {
                                System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Opening HiringDecisionDialog for final review");
                                using (HiringDecisionDialog dialog = new HiringDecisionDialog(_db, applicationId))
                                {
                                    result = dialog.ShowDialog(this);
                                    newStatus = dialog.HiringDecision;
                                    System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Final review dialog closed with decision: {newStatus}");
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Access denied for non-manager user (RoleID={_userRoleID})");
                                MessageBox.Show("Only HR Manager or Admin can make final hiring decisions.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            break;

                        case "Accepted":
                        case "Rejected":
                            System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Application is finalized with status '{currentStatus}', cannot change");
                            MessageBox.Show("This application has already been finalized and cannot be changed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;

                        default:
                            System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Unknown application status: '{currentStatus}'");
                            MessageBox.Show("Unknown application status.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                    }

                    if (result == DialogResult.OK)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Dialog returned OK, refreshing applications");
                        MessageBox.Show($"Application status updated to: {newStatus}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadApplications(); // Refresh the grid
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Dialog was cancelled or closed without OK");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ChangeStatus] Exception: {ex.Message}\n{ex.StackTrace}");
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[ChangeStatus] No row selected in grid");
                MessageBox.Show("Please select an application first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        private void RefreshButton_Click(object? sender, EventArgs? e)
        {
            searchTextBox!.Clear();

            LoadApplications();
            
            // Re-apply the status filter to maintain the current workflow stage
            if (_defaultStatusFilter != "All Statuses")
            {
                ApplyAllFilters();
            }
        }

        private void ViewApplicantButton_Click(object? sender, EventArgs? e) => ViewApplicantProfile();
        private void ViewDocumentsButton_Click(object? sender, EventArgs? e) => ViewDocuments();
        private void ChangeStatusButton_Click(object? sender, EventArgs? e) => ChangeStatus();
        private void CloseButton_Click(object? sender, EventArgs? e) => this.Close();

        private void InitializeComponent()
        {
            applicationsDataGridView = new DataGridView();
            filterPanel = new Panel();
            titleLabel = new Label();
            searchLabel = new Label();
            searchTextBox = new TextBox();
            buttonPanel = new Panel();
            viewApplicantButton = new Button();
            viewDocumentsButton = new Button();
            changeStatusButton = new Button();
            refreshButton = new Button();
            closeButton = new Button();
            statusLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)applicationsDataGridView).BeginInit();
            filterPanel.SuspendLayout();
            buttonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // applicationsDataGridView
            // 
            applicationsDataGridView.BackgroundColor = Color.White;
            applicationsDataGridView.ColumnHeadersHeight = 29;
            applicationsDataGridView.Dock = DockStyle.Fill;
            applicationsDataGridView.Location = new Point(0, 159);
            applicationsDataGridView.Margin = new Padding(3, 4, 3, 4);
            applicationsDataGridView.Name = "applicationsDataGridView";
            applicationsDataGridView.RowHeadersWidth = 51;
            applicationsDataGridView.Size = new Size(1600, 774);
            applicationsDataGridView.TabIndex = 2;
            // 
            // filterPanel
            // 
            filterPanel.BackColor = Color.FromArgb(240, 240, 240);
            filterPanel.BorderStyle = BorderStyle.FixedSingle;
            filterPanel.Controls.Add(titleLabel);
            filterPanel.Controls.Add(searchLabel);
            filterPanel.Controls.Add(searchTextBox);
            filterPanel.Controls.Add(buttonPanel);
            filterPanel.Controls.Add(statusLabel);
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Location = new Point(0, 0);
            filterPanel.Margin = new Padding(3, 4, 3, 4);
            filterPanel.Name = "filterPanel";
            filterPanel.Padding = new Padding(11, 13, 11, 13);
            filterPanel.Size = new Size(1600, 159);
            filterPanel.TabIndex = 3;
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(0, 51, 102);
            titleLabel.Location = new Point(11, 13);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(291, 28);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Job Applications - HR Review";
            // 
            // searchLabel
            // 
            searchLabel.AutoSize = true;
            searchLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            searchLabel.Location = new Point(11, 53);
            searchLabel.Name = "searchLabel";
            searchLabel.Size = new Size(59, 20);
            searchLabel.TabIndex = 1;
            searchLabel.Text = "Search:";
            // 
            // searchTextBox
            // 
            searchTextBox.BorderStyle = BorderStyle.FixedSingle;
            searchTextBox.Font = new Font("Segoe UI", 9F);
            searchTextBox.Location = new Point(69, 49);
            searchTextBox.Margin = new Padding(3, 4, 3, 4);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new Size(457, 27);
            searchTextBox.TabIndex = 2;
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            // 
            // buttonPanel
            // 
            buttonPanel.AutoSize = true;
            buttonPanel.Controls.Add(viewApplicantButton);
            buttonPanel.Controls.Add(viewDocumentsButton);
            buttonPanel.Controls.Add(changeStatusButton);
            buttonPanel.Controls.Add(refreshButton);
            buttonPanel.Controls.Add(closeButton);
            buttonPanel.Location = new Point(11, 87);
            buttonPanel.Margin = new Padding(3, 4, 3, 4);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Size = new Size(1143, 53);
            buttonPanel.TabIndex = 3;
            // 
            // viewApplicantButton
            // 
            viewApplicantButton.BackColor = Color.FromArgb(0, 120, 215);
            viewApplicantButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            viewApplicantButton.ForeColor = Color.White;
            viewApplicantButton.Location = new Point(0, 0);
            viewApplicantButton.Margin = new Padding(3, 4, 3, 4);
            viewApplicantButton.Name = "viewApplicantButton";
            viewApplicantButton.Size = new Size(114, 40);
            viewApplicantButton.TabIndex = 0;
            viewApplicantButton.Text = "View Applicant";
            viewApplicantButton.UseVisualStyleBackColor = false;
            viewApplicantButton.Click += ViewApplicantButton_Click;
            // 
            // viewDocumentsButton
            // 
            viewDocumentsButton.BackColor = Color.FromArgb(0, 120, 215);
            viewDocumentsButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            viewDocumentsButton.ForeColor = Color.White;
            viewDocumentsButton.Location = new Point(120, 0);
            viewDocumentsButton.Margin = new Padding(3, 4, 3, 4);
            viewDocumentsButton.Name = "viewDocumentsButton";
            viewDocumentsButton.Size = new Size(126, 40);
            viewDocumentsButton.TabIndex = 1;
            viewDocumentsButton.Text = "View Documents";
            viewDocumentsButton.UseVisualStyleBackColor = false;
            viewDocumentsButton.Click += ViewDocumentsButton_Click;
            // 
            // changeStatusButton
            // 
            changeStatusButton.BackColor = Color.FromArgb(34, 139, 34);
            changeStatusButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            changeStatusButton.ForeColor = Color.White;
            changeStatusButton.Location = new Point(251, 0);
            changeStatusButton.Margin = new Padding(3, 4, 3, 4);
            changeStatusButton.Name = "changeStatusButton";
            changeStatusButton.Size = new Size(126, 40);
            changeStatusButton.TabIndex = 2;
            changeStatusButton.Text = "Change Status";
            changeStatusButton.UseVisualStyleBackColor = false;
            changeStatusButton.Click += ChangeStatusButton_Click;
            // 
            // refreshButton
            // 
            refreshButton.BackColor = Color.FromArgb(107, 142, 35);
            refreshButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            refreshButton.ForeColor = Color.White;
            refreshButton.Location = new Point(383, 0);
            refreshButton.Margin = new Padding(3, 4, 3, 4);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(91, 40);
            refreshButton.TabIndex = 3;
            refreshButton.Text = "Refresh";
            refreshButton.UseVisualStyleBackColor = false;
            refreshButton.Click += RefreshButton_Click;
            // 
            // closeButton
            // 
            closeButton.BackColor = Color.FromArgb(128, 128, 128);
            closeButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            closeButton.ForeColor = Color.White;
            closeButton.Location = new Point(489, 0);
            closeButton.Margin = new Padding(3, 4, 3, 4);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(91, 40);
            closeButton.TabIndex = 4;
            closeButton.Text = "Close";
            closeButton.UseVisualStyleBackColor = false;
            closeButton.Click += CloseButton_Click;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Font = new Font("Segoe UI", 9F);
            statusLabel.ForeColor = Color.FromArgb(100, 100, 100);
            statusLabel.Location = new Point(11, 140);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(72, 20);
            statusLabel.TabIndex = 4;
            statusLabel.Text = "Loading...";
            // 
            // ApplicationManagementForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1600, 933);
            Controls.Add(applicationsDataGridView);
            Controls.Add(filterPanel);
            Margin = new Padding(3, 4, 3, 4);
            Name = "ApplicationManagementForm";
            Load += ApplicationManagementForm_Load;
            ((System.ComponentModel.ISupportInitialize)applicationsDataGridView).EndInit();
            filterPanel.ResumeLayout(false);
            filterPanel.PerformLayout();
            buttonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
