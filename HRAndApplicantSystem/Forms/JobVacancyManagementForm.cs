using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Services;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HRAndApplicantSystem.Forms
{
    /// <summary>
    /// Job Vacancy Management Form - HR manages job postings
    /// 
    /// ARCHITECTURE: UI Layer Only
    /// - Display all job vacancies in DataGridView
    /// - Filter by status (Open/Closed/All)
    /// - Create, Edit, Close, Delete operations
    /// - Delegates business logic to JobVacancyManagementService
    /// </summary>
    public partial class JobVacancyManagementForm : Form
    {
        private readonly DatabaseHelper _db;
        private readonly string _username;
        private JobVacancyManagementService _jobService;
        private List<JobVacancy> _allVacancies;
        
        private DataGridView? vacanciesDataGridView;
        private ComboBox? statusFilterComboBox;
        private Button? createButton;
        private Button? editButton;
        private Button? closeButton;
        private Button? deleteButton;
        private Button? refreshButton;
        private Button? exitButton;
        private Label? statusLabel;
        private TextBox? searchTextBox;

        public JobVacancyManagementForm(DatabaseHelper db, string username = "HR")
        {
            InitializeComponent();
            _db = db;
            _username = username;
            _jobService = new JobVacancyManagementService();
            _allVacancies = new List<JobVacancy>();
            
            this.Text = "Job Vacancy Management";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(1200, 700);
            this.BackColor = System.Drawing.Color.White;
            this.Load += JobVacancyManagementForm_Load;
        }

        private void InitializeComponent()
        {
            // Title
            Label titleLabel = new Label();
            titleLabel.Text = "Job Vacancy Management";
            titleLabel.Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            titleLabel.Location = new System.Drawing.Point(15, 10);
            titleLabel.Width = 400;
            titleLabel.Height = 30;
            this.Controls.Add(titleLabel);

            // Filter Panel
            Panel filterPanel = new Panel();
            filterPanel.Location = new System.Drawing.Point(15, 45);
            filterPanel.Width = 1170;
            filterPanel.Height = 50;
            filterPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            filterPanel.BorderStyle = BorderStyle.FixedSingle;

            // Status Filter Label
            Label filterLabel = new Label();
            filterLabel.Text = "Filter by Status:";
            filterLabel.Location = new System.Drawing.Point(10, 12);
            filterLabel.Width = 100;
            filterLabel.Height = 25;
            filterPanel.Controls.Add(filterLabel);

            // Status Filter ComboBox
            statusFilterComboBox = new ComboBox();
            statusFilterComboBox.Items.AddRange(new[] { "All", "Open", "Closed" });
            statusFilterComboBox.SelectedIndex = 0;
            statusFilterComboBox.Location = new System.Drawing.Point(120, 12);
            statusFilterComboBox.Width = 150;
            statusFilterComboBox.Height = 25;
            statusFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            statusFilterComboBox.SelectedIndexChanged += (s, e) => ApplyFilters();
            filterPanel.Controls.Add(statusFilterComboBox);

            // Search Label
            Label searchLabel = new Label();
            searchLabel.Text = "Search Title:";
            searchLabel.Location = new System.Drawing.Point(290, 12);
            searchLabel.Width = 80;
            searchLabel.Height = 25;
            filterPanel.Controls.Add(searchLabel);

            // Search TextBox
            searchTextBox = new TextBox();
            searchTextBox.Location = new System.Drawing.Point(380, 12);
            searchTextBox.Width = 200;
            searchTextBox.Height = 25;
            searchTextBox.TextChanged += (s, e) => ApplyFilters();
            filterPanel.Controls.Add(searchTextBox);

            this.Controls.Add(filterPanel);

            // DataGridView
            vacanciesDataGridView = new DataGridView();
            vacanciesDataGridView.Location = new System.Drawing.Point(15, 105);
            vacanciesDataGridView.Size = new System.Drawing.Size(1170, 450);
            vacanciesDataGridView.BackgroundColor = System.Drawing.Color.White;
            vacanciesDataGridView.BorderStyle = BorderStyle.Fixed3D;
            vacanciesDataGridView.AllowUserToAddRows = false;
            vacanciesDataGridView.AllowUserToDeleteRows = false;
            vacanciesDataGridView.ReadOnly = true;
            vacanciesDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            vacanciesDataGridView.MultiSelect = false;
            vacanciesDataGridView.RowHeadersVisible = false;
            vacanciesDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            vacanciesDataGridView.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(vacanciesDataGridView);

            // Status Label
            statusLabel = new Label();
            statusLabel.Location = new System.Drawing.Point(15, 565);
            statusLabel.Width = 600;
            statusLabel.Height = 20;
            statusLabel.Font = new System.Drawing.Font("Arial", 9);
            statusLabel.ForeColor = System.Drawing.Color.Gray;
            this.Controls.Add(statusLabel);

            // Button Panel
            Panel buttonPanel = new Panel();
            buttonPanel.Location = new System.Drawing.Point(15, 595);
            buttonPanel.Width = 1170;
            buttonPanel.Height = 50;
            buttonPanel.BackColor = System.Drawing.Color.WhiteSmoke;

            // Create Button
            createButton = new Button();
            createButton.Text = "Create New";
            createButton.Size = new System.Drawing.Size(120, 35);
            createButton.Location = new System.Drawing.Point(10, 7);
            createButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            createButton.ForeColor = System.Drawing.Color.White;
            createButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            createButton.Click += CreateButton_Click;
            buttonPanel.Controls.Add(createButton);

            // Edit Button
            editButton = new Button();
            editButton.Text = "Edit";
            editButton.Size = new System.Drawing.Size(100, 35);
            editButton.Location = new System.Drawing.Point(140, 7);
            editButton.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            editButton.ForeColor = System.Drawing.Color.White;
            editButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            editButton.Click += EditButton_Click;
            buttonPanel.Controls.Add(editButton);

            // Close Button
            closeButton = new Button();
            closeButton.Text = "Close";
            closeButton.Size = new System.Drawing.Size(100, 35);
            closeButton.Location = new System.Drawing.Point(250, 7);
            closeButton.BackColor = System.Drawing.Color.FromArgb(255, 165, 0);
            closeButton.ForeColor = System.Drawing.Color.White;
            closeButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            closeButton.Click += CloseVacancyButton_Click;
            buttonPanel.Controls.Add(closeButton);

            // Delete Button
            deleteButton = new Button();
            deleteButton.Text = "Delete";
            deleteButton.Size = new System.Drawing.Size(100, 35);
            deleteButton.Location = new System.Drawing.Point(360, 7);
            deleteButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
            deleteButton.ForeColor = System.Drawing.Color.White;
            deleteButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            deleteButton.Click += DeleteButton_Click;
            buttonPanel.Controls.Add(deleteButton);

            // Refresh Button (moved to left side)
            refreshButton = new Button();
            refreshButton.Text = "Refresh";
            refreshButton.Size = new System.Drawing.Size(100, 35);
            refreshButton.Location = new System.Drawing.Point(470, 7);
            refreshButton.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
            refreshButton.ForeColor = System.Drawing.Color.White;
            refreshButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            refreshButton.Click += (s, e) => LoadVacancies();
            buttonPanel.Controls.Add(refreshButton);

            // Exit Button
            exitButton = new Button();
            exitButton.Text = "Exit";
            exitButton.Size = new System.Drawing.Size(100, 35);
            exitButton.Location = new System.Drawing.Point(1050, 7);
            exitButton.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
            exitButton.ForeColor = System.Drawing.Color.White;
            exitButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            exitButton.Click += (s, e) => this.Close();
            buttonPanel.Controls.Add(exitButton);

            this.Controls.Add(buttonPanel);
        }

        private void JobVacancyManagementForm_Load(object? sender, EventArgs? e)
        {
            try
            {
                LoadVacancies();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vacancies: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadVacancies()
        {
            try
            {
                _allVacancies = _jobService.GetAllJobs();
                ApplyFilters();
                statusLabel!.Text = $"Total vacancies: {_allVacancies.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vacancies: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                if (vacanciesDataGridView == null) return;

                vacanciesDataGridView.DataSource = null;
                var filtered = new List<JobVacancy>(_allVacancies);

                // Status filter
                string statusFilter = statusFilterComboBox?.SelectedItem?.ToString() ?? "All";
                if (statusFilter != "All")
                {
                    filtered = filtered.FindAll(j => j.Status == statusFilter);
                }

                // Search filter
                string searchText = searchTextBox?.Text?.ToLower() ?? "";
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    filtered = filtered.FindAll(j => j.JobTitle?.ToLower().Contains(searchText) ?? false);
                }

                // Bind to grid
                var bindingSource = new BindingSource(filtered, null);
                vacanciesDataGridView.DataSource = bindingSource;

                // Configure columns
                if (vacanciesDataGridView.Columns.Count > 0)
                {
                    if (vacanciesDataGridView.Columns["JobID"] != null)
                        vacanciesDataGridView.Columns["JobID"].Width = 80;
                    if (vacanciesDataGridView.Columns["JobTitle"] != null)
                        vacanciesDataGridView.Columns["JobTitle"].Width = 300;
                    if (vacanciesDataGridView.Columns["JobDetail"] != null)
                        vacanciesDataGridView.Columns["JobDetail"].Width = 600;
                    if (vacanciesDataGridView.Columns["Status"] != null)
                        vacanciesDataGridView.Columns["Status"].Width = 100;
                    // Hide DatePosted column if it exists
                    if (vacanciesDataGridView.Columns["DatePosted"] != null)
                        vacanciesDataGridView.Columns["DatePosted"].Visible = false;
                }

                statusLabel!.Text = $"Displaying {filtered.Count} of {_allVacancies.Count} vacancies";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filters: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateButton_Click(object? sender, EventArgs? e)
        {
            try
            {
                CreateJobVacancyDialog dialog = new CreateJobVacancyDialog(_jobService, _username);
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    LoadVacancies();
                    MessageBox.Show("Job vacancy created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating vacancy: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditButton_Click(object? sender, EventArgs? e)
        {
            try
            {
                if (vacanciesDataGridView?.SelectedRows.Count != 1)
                {
                    MessageBox.Show("Please select a job vacancy to edit.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRow = vacanciesDataGridView?.SelectedRows[0];
                if (selectedRow?.Cells["JobID"].Value is not int jobID)
                {
                    MessageBox.Show("Invalid selection. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                JobVacancy? job = _jobService.GetJobByID(jobID);

                if (job == null)
                {
                    MessageBox.Show("Could not retrieve job details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                EditJobVacancyDialog dialog = new EditJobVacancyDialog(job, _jobService, _username);
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    LoadVacancies();
                    MessageBox.Show("Job vacancy updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing vacancy: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseVacancyButton_Click(object? sender, EventArgs? e)
        {
            try
            {
                if (vacanciesDataGridView?.SelectedRows.Count != 1)
                {
                    MessageBox.Show("Please select a job vacancy to close.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRow = vacanciesDataGridView?.SelectedRows[0];
                if (selectedRow?.Cells["JobID"].Value is not int jobID)
                {
                    MessageBox.Show("Invalid selection. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var jobTitleValue = selectedRow?.Cells["JobTitle"].Value;
                string jobTitle = jobTitleValue?.ToString() ?? "Unknown Job";

                if (MessageBox.Show($"Close vacancy: {jobTitle}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (_jobService.CloseJob(jobID, _username))
                    {
                        LoadVacancies();
                        MessageBox.Show("Job vacancy closed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to close job vacancy.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error closing vacancy: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteButton_Click(object? sender, EventArgs? e)
        {
            try
            {
                if (vacanciesDataGridView?.SelectedRows.Count != 1)
                {
                    MessageBox.Show("Please select a job vacancy to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRow = vacanciesDataGridView?.SelectedRows[0];
                if (selectedRow?.Cells["JobID"].Value is not int jobID)
                {
                    MessageBox.Show("Invalid selection. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var jobTitleValue = selectedRow?.Cells["JobTitle"].Value;
                string jobTitle = jobTitleValue?.ToString() ?? "Unknown Job";

                if (MessageBox.Show($"Delete vacancy: {jobTitle}?\n\nThis action cannot be undone.", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (_jobService.DeleteJob(jobID, _username, out string error))
                    {
                        LoadVacancies();
                        MessageBox.Show("Job vacancy deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Cannot delete vacancy:\n{error}", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting vacancy: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
