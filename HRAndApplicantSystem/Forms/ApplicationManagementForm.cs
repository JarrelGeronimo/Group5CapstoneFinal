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
        private Panel filterPanel;
        private Label titleLabel;
        private Label searchLabel;
        private Panel buttonPanel;
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
