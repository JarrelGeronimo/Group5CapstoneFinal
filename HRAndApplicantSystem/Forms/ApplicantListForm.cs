using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Forms
{
    /// <summary>
    /// Applicant List Form - Display all applicants in DataGridView
    /// 
    /// ARCHITECTURE: UI Layer Only
    /// - Shows applicants in a DataGridView
    /// - Delegates data retrieval to ApplicantRepository
    /// - Allows filtering, sorting, and selection
    /// - Opens ApplicantDetailForm for editing
    /// </summary>
    public partial class ApplicantListForm : Form
    {
        private readonly DatabaseHelper _db;
        private readonly IApplicantRepository _applicantRepository;
        private List<dynamic> _allApplicants;
        private DataGridView applicantsDataGridView;
        private TextBox searchTextBox;
        private Label statusLabel;
        private Button addButton;
        private Button editButton;
        private Button deleteButton;
        private Button refreshButton;
        private Button closeButton;

        public ApplicantListForm(DatabaseHelper db)
        {
            InitializeComponent();
            _db = db;
            _applicantRepository = new ApplicantRepository(db);
            this.Text = "Applicant Management";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(1200, 700);
        }

        private void ApplicantListForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadApplicants();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading applicants: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadApplicants()
        {
            try
            {
                // Get all applicants from database
                _allApplicants = _db.GetAllApplicantsWithApplications();
                
                // Populate DataGridView
                applicantsDataGridView.DataSource = _allApplicants;
                
                // Format columns
                FormatDataGridView();
                
                // Bind double-click event
                applicantsDataGridView.CellDoubleClick += ApplicantsDataGridView_CellDoubleClick;

                statusLabel.Text = $"Total Applicants: {_allApplicants.Count}";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error: {ex.Message}";
                _allApplicants = new List<dynamic>();
            }
        }

        private void FormatDataGridView()
        {
            applicantsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            applicantsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            applicantsDataGridView.AllowUserToAddRows = false;
            applicantsDataGridView.AllowUserToDeleteRows = false;
            applicantsDataGridView.ReadOnly = true;
        }

        private void ApplicantsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    var row = applicantsDataGridView.Rows[e.RowIndex];
                    int applicantId = Convert.ToInt32(row.Cells["ApplicantID"]?.Value ?? 0);
                    
                    if (applicantId > 0)
                    {
                        MessageBox.Show($"Opening details for Applicant ID: {applicantId}\nApplicant Detail form coming in Phase 3.2", 
                            "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_allApplicants == null) return;

            string searchTerm = searchTextBox.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                applicantsDataGridView.DataSource = _allApplicants;
                statusLabel.Text = $"Total Applicants: {_allApplicants.Count}";
            }
            else
            {
                var filtered = _allApplicants.Where(a =>
                    (a.FirstName?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (a.LastName?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (a.ContactNo?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                applicantsDataGridView.DataSource = filtered;
                statusLabel.Text = $"Found: {filtered.Count} of {_allApplicants.Count} applicants";
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            searchTextBox.Clear();
            LoadApplicants();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Add new applicant feature coming in Phase 3.2", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (applicantsDataGridView.SelectedRows.Count > 0)
            {
                MessageBox.Show("Edit applicant feature coming in Phase 3.2", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select an applicant first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (applicantsDataGridView.SelectedRows.Count > 0)
            {
                MessageBox.Show("Delete applicant feature coming in Phase 3.2", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select an applicant first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // DataGridView
            applicantsDataGridView = new DataGridView();
            applicantsDataGridView.Dock = DockStyle.Fill;
            applicantsDataGridView.BackgroundColor = System.Drawing.Color.White;
            applicantsDataGridView.Location = new System.Drawing.Point(0, 80);
            applicantsDataGridView.Name = "applicantsDataGridView";
            applicantsDataGridView.Size = new System.Drawing.Size(1200, 570);
            applicantsDataGridView.TabIndex = 2;
            this.Controls.Add(applicantsDataGridView);

            // Search Panel
            Panel searchPanel = new Panel();
            searchPanel.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 80;
            searchPanel.Padding = new Padding(10);
            searchPanel.BorderStyle = BorderStyle.FixedSingle;

            // Search Label
            Label searchLabel = new Label();
            searchLabel.AutoSize = true;
            searchLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            searchLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            searchLabel.Location = new System.Drawing.Point(10, 10);
            searchLabel.Text = "Search Applicants:";
            searchPanel.Controls.Add(searchLabel);

            // Search TextBox
            searchTextBox = new TextBox();
            searchTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            searchTextBox.Location = new System.Drawing.Point(10, 32);
            searchTextBox.Width = 300;
            searchTextBox.Height = 28;
            searchTextBox.BorderStyle = BorderStyle.FixedSingle;
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            searchPanel.Controls.Add(searchTextBox);

            // Buttons Panel
            Panel buttonPanel = new Panel();
            buttonPanel.Location = new System.Drawing.Point(320, 32);
            buttonPanel.Size = new System.Drawing.Size(450, 35);
            buttonPanel.AutoSize = true;

            // Add Button
            addButton = new Button();
            addButton.Text = "Add New";
            addButton.Size = new System.Drawing.Size(80, 30);
            addButton.Location = new System.Drawing.Point(0, 0);
            addButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            addButton.ForeColor = System.Drawing.Color.White;
            addButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            addButton.Click += AddButton_Click;
            buttonPanel.Controls.Add(addButton);

            // Edit Button
            editButton = new Button();
            editButton.Text = "Edit";
            editButton.Size = new System.Drawing.Size(70, 30);
            editButton.Location = new System.Drawing.Point(85, 0);
            editButton.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            editButton.ForeColor = System.Drawing.Color.White;
            editButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            editButton.Click += EditButton_Click;
            buttonPanel.Controls.Add(editButton);

            // Delete Button
            deleteButton = new Button();
            deleteButton.Text = "Delete";
            deleteButton.Size = new System.Drawing.Size(70, 30);
            deleteButton.Location = new System.Drawing.Point(160, 0);
            deleteButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
            deleteButton.ForeColor = System.Drawing.Color.White;
            deleteButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            deleteButton.Click += DeleteButton_Click;
            buttonPanel.Controls.Add(deleteButton);

            // Refresh Button
            refreshButton = new Button();
            refreshButton.Text = "Refresh";
            refreshButton.Size = new System.Drawing.Size(70, 30);
            refreshButton.Location = new System.Drawing.Point(235, 0);
            refreshButton.BackColor = System.Drawing.Color.FromArgb(107, 142, 35);
            refreshButton.ForeColor = System.Drawing.Color.White;
            refreshButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            refreshButton.Click += RefreshButton_Click;
            buttonPanel.Controls.Add(refreshButton);

            // Close Button
            closeButton = new Button();
            closeButton.Text = "Close";
            closeButton.Size = new System.Drawing.Size(70, 30);
            closeButton.Location = new System.Drawing.Point(310, 0);
            closeButton.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
            closeButton.ForeColor = System.Drawing.Color.White;
            closeButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            closeButton.Click += CloseButton_Click;
            buttonPanel.Controls.Add(closeButton);

            searchPanel.Controls.Add(buttonPanel);

            // Status Label
            statusLabel = new Label();
            statusLabel.AutoSize = true;
            statusLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            statusLabel.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            statusLabel.Location = new System.Drawing.Point(10, 58);
            statusLabel.Text = "Loading...";
            searchPanel.Controls.Add(statusLabel);

            this.Controls.Add(searchPanel);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Name = "ApplicantListForm";
            this.Load += new System.EventHandler(this.ApplicantListForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
