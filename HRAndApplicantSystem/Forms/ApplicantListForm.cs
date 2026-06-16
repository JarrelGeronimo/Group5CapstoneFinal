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
            applicantsDataGridView = new DataGridView();
            this.searchPanel = new Panel();
            this.searchLabel = new Label();
            searchTextBox = new TextBox();
            this.buttonPanel = new Panel();
            addButton = new Button();
            editButton = new Button();
            deleteButton = new Button();
            refreshButton = new Button();
            closeButton = new Button();
            statusLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)applicantsDataGridView).BeginInit();
            this.searchPanel.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // applicantsDataGridView
            // 
            applicantsDataGridView.BackgroundColor = Color.White;
            applicantsDataGridView.ColumnHeadersHeight = 29;
            applicantsDataGridView.Dock = DockStyle.Fill;
            applicantsDataGridView.Location = new Point(0, 106);
            applicantsDataGridView.Margin = new Padding(3, 4, 3, 4);
            applicantsDataGridView.Name = "applicantsDataGridView";
            applicantsDataGridView.RowHeadersWidth = 51;
            applicantsDataGridView.Size = new Size(1349, 827);
            applicantsDataGridView.TabIndex = 2;
            // 
            // searchPanel
            // 
            this.searchPanel.BackColor = Color.FromArgb(240, 240, 240);
            this.searchPanel.BorderStyle = BorderStyle.FixedSingle;
            this.searchPanel.Controls.Add(this.searchLabel);
            this.searchPanel.Controls.Add(searchTextBox);
            this.searchPanel.Controls.Add(this.buttonPanel);
            this.searchPanel.Controls.Add(statusLabel);
            this.searchPanel.Dock = DockStyle.Top;
            this.searchPanel.Location = new Point(0, 0);
            this.searchPanel.Margin = new Padding(3, 4, 3, 4);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Padding = new Padding(11, 13, 11, 13);
            this.searchPanel.Size = new Size(1349, 106);
            this.searchPanel.TabIndex = 3;
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.searchLabel.ForeColor = Color.FromArgb(0, 51, 102);
            this.searchLabel.Location = new Point(11, 13);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new Size(158, 23);
            this.searchLabel.TabIndex = 0;
            this.searchLabel.Text = "Search Applicants:";
            // 
            // searchTextBox
            // 
            searchTextBox.BorderStyle = BorderStyle.FixedSingle;
            searchTextBox.Font = new Font("Segoe UI", 10F);
            searchTextBox.Location = new Point(11, 43);
            searchTextBox.Margin = new Padding(3, 4, 3, 4);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new Size(343, 30);
            searchTextBox.TabIndex = 1;
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            // 
            // buttonPanel
            // 
            this.buttonPanel.AutoSize = true;
            this.buttonPanel.Controls.Add(addButton);
            this.buttonPanel.Controls.Add(editButton);
            this.buttonPanel.Controls.Add(deleteButton);
            this.buttonPanel.Controls.Add(refreshButton);
            this.buttonPanel.Controls.Add(closeButton);
            this.buttonPanel.Location = new Point(366, 43);
            this.buttonPanel.Margin = new Padding(3, 4, 3, 4);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new Size(514, 47);
            this.buttonPanel.TabIndex = 2;
            // 
            // addButton
            // 
            addButton.BackColor = Color.FromArgb(34, 139, 34);
            addButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            addButton.ForeColor = Color.White;
            addButton.Location = new Point(0, 0);
            addButton.Margin = new Padding(3, 4, 3, 4);
            addButton.Name = "addButton";
            addButton.Size = new Size(91, 40);
            addButton.TabIndex = 0;
            addButton.Text = "Add New";
            addButton.UseVisualStyleBackColor = false;
            addButton.Click += AddButton_Click;
            // 
            // editButton
            // 
            editButton.BackColor = Color.FromArgb(0, 120, 215);
            editButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            editButton.ForeColor = Color.White;
            editButton.Location = new Point(97, 0);
            editButton.Margin = new Padding(3, 4, 3, 4);
            editButton.Name = "editButton";
            editButton.Size = new Size(80, 40);
            editButton.TabIndex = 1;
            editButton.Text = "Edit";
            editButton.UseVisualStyleBackColor = false;
            editButton.Click += EditButton_Click;
            // 
            // deleteButton
            // 
            deleteButton.BackColor = Color.FromArgb(220, 20, 60);
            deleteButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            deleteButton.ForeColor = Color.White;
            deleteButton.Location = new Point(183, 0);
            deleteButton.Margin = new Padding(3, 4, 3, 4);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(80, 40);
            deleteButton.TabIndex = 2;
            deleteButton.Text = "Delete";
            deleteButton.UseVisualStyleBackColor = false;
            deleteButton.Click += DeleteButton_Click;
            // 
            // refreshButton
            // 
            refreshButton.BackColor = Color.FromArgb(107, 142, 35);
            refreshButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            refreshButton.ForeColor = Color.White;
            refreshButton.Location = new Point(269, 0);
            refreshButton.Margin = new Padding(3, 4, 3, 4);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(80, 40);
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
            closeButton.Location = new Point(354, 0);
            closeButton.Margin = new Padding(3, 4, 3, 4);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(80, 40);
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
            statusLabel.Location = new Point(11, 77);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(72, 20);
            statusLabel.TabIndex = 3;
            statusLabel.Text = "Loading...";
            // 
            // ApplicantListForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1349, 933);
            Controls.Add(applicantsDataGridView);
            Controls.Add(this.searchPanel);
            Margin = new Padding(3, 4, 3, 4);
            Name = "ApplicantListForm";
            Load += ApplicantListForm_Load;
            ((System.ComponentModel.ISupportInitialize)applicantsDataGridView).EndInit();
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
