using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using ApplicationModel = HRAndApplicantSystem.Models.Application;

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
        private string _defaultStatusFilter = "All Statuses"; // Default filter
        private List<dynamic> _allApplications;
        private DataGridView applicationsDataGridView;
        private TextBox searchTextBox;
        private Label statusLabel;
        private Button viewApplicantButton;
        private Button viewDocumentsButton;
        private Button changeStatusButton;
        private Button refreshButton;
        private Button closeButton;

        public ApplicationManagementForm(DatabaseHelper db, int userRoleID = 0, string initialStatusFilter = "All Statuses")
        {
            InitializeComponent();
            _db = db;
            _userRoleID = userRoleID;
            _defaultStatusFilter = initialStatusFilter;
            this.Text = "Application Management";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(1400, 700);
        }

        private void ApplicationManagementForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadApplications();
                
                // Apply the initial status filter if one was specified
                if (_defaultStatusFilter != "All Statuses")
                {
                    ApplyAllFilters();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading applications: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void LoadApplications()
        {
            try
            {
                // Get all applications from database
                _allApplications = _db.GetAllApplications();
                
                // Populate DataGridView
                PopulateDataGridView(_allApplications);
                
                statusLabel.Text = $"Total Applications: {_allApplications.Count}";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error: {ex.Message}";
                _allApplications = new List<dynamic>();
            }
        }

        private void PopulateDataGridView(List<dynamic> applications)
        {
            applicationsDataGridView.DataSource = applications;
            applicationsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            applicationsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            applicationsDataGridView.AllowUserToAddRows = false;
            applicationsDataGridView.AllowUserToDeleteRows = false;
            applicationsDataGridView.ReadOnly = true;
            applicationsDataGridView.CellDoubleClick += ApplicationsDataGridView_CellDoubleClick;
        }

        private void ApplicationsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
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

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            ApplyAllFilters();
        }

        private void ApplyAllFilters()
        {
            if (_allApplications == null) return;

            var filtered = _allApplications.AsEnumerable();

            // Filter by default status if one was specified during form initialization
            if (_defaultStatusFilter != "All Statuses")
            {
                filtered = filtered.Where(a =>
                    (a.Status?.ToString() ?? "").Equals(_defaultStatusFilter, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Filter by search text (applicant name or job title)
            string searchTerm = searchTextBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filtered = filtered.Where(a =>
                    (a.FirstName?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (a.LastName?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (a.JobTitle?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }

            var filteredList = filtered.ToList();
            PopulateDataGridView(filteredList);
            statusLabel.Text = $"Filtered: {filteredList.Count} applications";
        }

        private void ViewApplicantProfile()
        {
            if (applicationsDataGridView.SelectedRows.Count > 0)
            {
                try
                {
                    var row = applicationsDataGridView.SelectedRows[0];
                    int applicantId = Convert.ToInt32(row.Cells["ApplicantID"]?.Value ?? 0);
                    string applicantName = $"{row.Cells["FirstName"]?.Value} {row.Cells["LastName"]?.Value}";

                    // TODO: Open ApplicantProfileForm (read-only)
                    MessageBox.Show($"Viewing profile for: {applicantName} (ID: {applicantId})\n\nApplicant Profile viewer coming in Phase 3.2",
                        "View Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (applicationsDataGridView.SelectedRows.Count > 0)
            {
                try
                {
                    var row = applicationsDataGridView.SelectedRows[0];
                    int applicationId = Convert.ToInt32(row.Cells["ApplicationID"]?.Value ?? 0);

                    // TODO: Open DocumentReviewForm
                    MessageBox.Show($"Viewing documents for Application ID: {applicationId}\n\nDocument viewer coming in Phase 3.2",
                        "View Documents", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (applicationsDataGridView.SelectedRows.Count > 0)
            {
                try
                {
                    var row = applicationsDataGridView.SelectedRows[0];
                    int applicationId = Convert.ToInt32(row.Cells["ApplicationID"]?.Value ?? 0);
                    string currentStatus = row.Cells["Status"]?.Value?.ToString() ?? "";

                    // Route to appropriate dialog based on current status and user role
                    DialogResult result = DialogResult.Cancel;
                    string newStatus = "";

                    switch (currentStatus)
                    {
                        case "Pending":
                            // Stage 1: Application Review (HR Staff, Manager, Admin can do)
                            using (ApplicationReviewDialog dialog = new ApplicationReviewDialog(_db, applicationId))
                            {
                                result = dialog.ShowDialog(this);
                                newStatus = dialog.ReviewDecision;
                            }
                            break;

                        case "Screening":
                            // Stage 2: Interview (HR Staff, Manager, Admin can do)
                            using (InterviewDialog dialog = new InterviewDialog(_db, applicationId))
                            {
                                result = dialog.ShowDialog(this);
                                newStatus = dialog.InterviewDecision;
                            }
                            break;

                        case "Interview":
                            // Stage 3: Hiring Decision (Manager and Admin ONLY)
                            if (_userRoleID == 3 || _userRoleID == 4) // HR_MANAGER or ADMIN
                            {
                                using (HiringDecisionDialog dialog = new HiringDecisionDialog(_db, applicationId))
                                {
                                    result = dialog.ShowDialog(this);
                                    newStatus = dialog.HiringDecision;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Only HR Manager or Admin can make final hiring decisions.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            break;

                        case "Accepted":
                        case "Rejected":
                            MessageBox.Show("This application has already been finalized and cannot be changed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;

                        default:
                            MessageBox.Show("Unknown application status.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                    }

                    if (result == DialogResult.OK)
                    {
                        MessageBox.Show($"Application status updated to: {newStatus}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadApplications(); // Refresh the grid
                    }
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



        private void RefreshButton_Click(object sender, EventArgs e)
        {
            searchTextBox.Clear();

            LoadApplications();
            
            // Re-apply the status filter to maintain the current workflow stage
            if (_defaultStatusFilter != "All Statuses")
            {
                ApplyAllFilters();
            }
        }

        private void ViewApplicantButton_Click(object sender, EventArgs e) => ViewApplicantProfile();
        private void ViewDocumentsButton_Click(object sender, EventArgs e) => ViewDocuments();
        private void ChangeStatusButton_Click(object sender, EventArgs e) => ChangeStatus();
        private void CloseButton_Click(object sender, EventArgs e) => this.Close();

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // DataGridView
            applicationsDataGridView = new DataGridView();
            applicationsDataGridView.Dock = DockStyle.Fill;
            applicationsDataGridView.BackgroundColor = System.Drawing.Color.White;
            applicationsDataGridView.Location = new System.Drawing.Point(0, 120);
            applicationsDataGridView.Name = "applicationsDataGridView";
            applicationsDataGridView.Size = new System.Drawing.Size(1400, 550);
            applicationsDataGridView.TabIndex = 2;
            this.Controls.Add(applicationsDataGridView);

            // Filter Panel
            Panel filterPanel = new Panel();
            filterPanel.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Height = 120;
            filterPanel.Padding = new Padding(10);
            filterPanel.BorderStyle = BorderStyle.FixedSingle;

            // Title Label
            Label titleLabel = new Label();
            titleLabel.Text = "Job Applications - HR Review";
            titleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            titleLabel.Location = new System.Drawing.Point(10, 10);
            titleLabel.AutoSize = true;
            filterPanel.Controls.Add(titleLabel);

            // Search Label (no status filter anymore)
            Label searchLabel = new Label();
            searchLabel.Text = "Search:";
            searchLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            searchLabel.Location = new System.Drawing.Point(10, 40);
            searchLabel.AutoSize = true;
            filterPanel.Controls.Add(searchLabel);

            // Search TextBox
            searchTextBox = new TextBox();
            searchTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            searchTextBox.Location = new System.Drawing.Point(60, 37);
            searchTextBox.Width = 400;
            searchTextBox.Height = 28;
            searchTextBox.BorderStyle = BorderStyle.FixedSingle;
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            filterPanel.Controls.Add(searchTextBox);

            // Buttons Row 2
            Panel buttonPanel = new Panel();
            buttonPanel.Location = new System.Drawing.Point(10, 65);
            buttonPanel.Size = new System.Drawing.Size(1000, 40);
            buttonPanel.AutoSize = true;

            // View Applicant Button
            viewApplicantButton = new Button();
            viewApplicantButton.Text = "View Applicant";
            viewApplicantButton.Size = new System.Drawing.Size(100, 30);
            viewApplicantButton.Location = new System.Drawing.Point(0, 0);
            viewApplicantButton.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            viewApplicantButton.ForeColor = System.Drawing.Color.White;
            viewApplicantButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            viewApplicantButton.Click += ViewApplicantButton_Click;
            buttonPanel.Controls.Add(viewApplicantButton);

            // View Documents Button
            viewDocumentsButton = new Button();
            viewDocumentsButton.Text = "View Documents";
            viewDocumentsButton.Size = new System.Drawing.Size(110, 30);
            viewDocumentsButton.Location = new System.Drawing.Point(105, 0);
            viewDocumentsButton.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            viewDocumentsButton.ForeColor = System.Drawing.Color.White;
            viewDocumentsButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            viewDocumentsButton.Click += ViewDocumentsButton_Click;
            buttonPanel.Controls.Add(viewDocumentsButton);

            // Change Status Button
            changeStatusButton = new Button();
            changeStatusButton.Text = "Change Status";
            changeStatusButton.Size = new System.Drawing.Size(110, 30);
            changeStatusButton.Location = new System.Drawing.Point(220, 0);
            changeStatusButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            changeStatusButton.ForeColor = System.Drawing.Color.White;
            changeStatusButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            changeStatusButton.Click += ChangeStatusButton_Click;
            buttonPanel.Controls.Add(changeStatusButton);



            // Refresh Button
            refreshButton = new Button();
            refreshButton.Text = "Refresh";
            refreshButton.Size = new System.Drawing.Size(80, 30);
            refreshButton.Location = new System.Drawing.Point(335, 0);
            refreshButton.BackColor = System.Drawing.Color.FromArgb(107, 142, 35);
            refreshButton.ForeColor = System.Drawing.Color.White;
            refreshButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            refreshButton.Click += RefreshButton_Click;
            buttonPanel.Controls.Add(refreshButton);

            // Close Button
            closeButton = new Button();
            closeButton.Text = "Close";
            closeButton.Size = new System.Drawing.Size(80, 30);
            closeButton.Location = new System.Drawing.Point(420, 0);
            closeButton.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
            closeButton.ForeColor = System.Drawing.Color.White;
            closeButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            closeButton.Click += CloseButton_Click;
            buttonPanel.Controls.Add(closeButton);

            filterPanel.Controls.Add(buttonPanel);

            // Status Label (info)
            statusLabel = new Label();
            statusLabel.AutoSize = true;
            statusLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            statusLabel.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            statusLabel.Location = new System.Drawing.Point(10, 105);
            statusLabel.Text = "Loading...";
            filterPanel.Controls.Add(statusLabel);

            this.Controls.Add(filterPanel);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1400, 700);
            this.Name = "ApplicationManagementForm";
            this.Load += new System.EventHandler(this.ApplicationManagementForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
