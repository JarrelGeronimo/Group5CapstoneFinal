using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HRAndApplicantSystem.Database;

namespace HRAndApplicantSystem.Forms
{
    public partial class JobVacanciesForm : Form
    {
        private readonly DatabaseHelper _db;
        private readonly int _applicantID;
        private List<dynamic> _allJobs;
        private DataGridView jobsDataGridView;
        private TextBox searchTextBox;
        private Button applyButton;
        private Button viewDetailsButton;
        private Button refreshButton;
        private Button closeButton;
        private Label statusLabel;

        public JobVacanciesForm(DatabaseHelper db, int applicantID)
        {
            InitializeComponent();
            _db = db;
            _applicantID = applicantID;
            this.Text = "Browse Job Vacancies";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(1200, 600);
        }

        private void JobVacanciesForm_Load(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("JobVacanciesForm_Load: Starting");
                LoadJobVacancies();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"JobVacanciesForm_Load Exception: {ex}");
                MessageBox.Show($"Error loading vacancies: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadJobVacancies()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("LoadJobVacancies: Starting");
                
                // Get all active job vacancies
                _allJobs = _db.GetActiveJobsAsDynamic();
                System.Diagnostics.Debug.WriteLine($"LoadJobVacancies: Retrieved {_allJobs?.Count ?? 0} jobs from database");
                
                if (_allJobs != null && _allJobs.Count > 0)
                {
                    foreach (var job in _allJobs)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Job: {job.JobTitle}");
                    }
                }
                
                ApplyFilters();
                UpdateStatusLabel();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadJobVacancies Exception: {ex}");
                MessageBox.Show($"Error loading jobs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            System.Diagnostics.Debug.WriteLine($"ApplyFilters: _allJobs count = {_allJobs?.Count ?? 0}");
            
            if (_allJobs == null)
            {
                System.Diagnostics.Debug.WriteLine("ApplyFilters: _allJobs is null, populating empty list");
                PopulateDataGridView(new List<dynamic>());
                return;
            }

            var filtered = new List<dynamic>(_allJobs);

            // Filter by search term
            string searchTerm = searchTextBox?.Text?.Trim() ?? "";
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filtered = filtered.FindAll(j =>
                    (j.JobTitle?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (j.CompanyName?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (j.JobDescription?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
                System.Diagnostics.Debug.WriteLine($"ApplyFilters: After search filter = {filtered.Count} jobs");
            }

            PopulateDataGridView(filtered);
        }

        private void PopulateDataGridView(List<dynamic> jobs)
        {
            System.Diagnostics.Debug.WriteLine($"PopulateDataGridView: Binding {jobs?.Count ?? 0} jobs");
            
            try
            {
                if (jobsDataGridView == null)
                {
                    System.Diagnostics.Debug.WriteLine("PopulateDataGridView: jobsDataGridView is null!");
                    return;
                }

                if (jobs == null || jobs.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("PopulateDataGridView: No jobs to display");
                    jobsDataGridView.DataSource = null;
                    statusLabel.Text = "No jobs available";
                    return;
                }
                
                // Bind data directly - works better with anonymous types
                jobsDataGridView.DataSource = jobs;
                System.Diagnostics.Debug.WriteLine($"PopulateDataGridView: DataGridView bound, rows: {jobsDataGridView.RowCount}, columns: {jobsDataGridView.Columns.Count}");
                
                // Log column information
                foreach (DataGridViewColumn col in jobsDataGridView.Columns)
                {
                    System.Diagnostics.Debug.WriteLine($"  Column: {col.Name} ({col.HeaderText})");
                }
                
                // Log first row data for verification
                if (jobsDataGridView.RowCount > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"First row cell count: {jobsDataGridView.Rows[0].Cells.Count}");
                    for (int i = 0; i < jobsDataGridView.Rows[0].Cells.Count && i < 5; i++)
                    {
                        var cellValue = jobsDataGridView.Rows[0].Cells[i].Value;
                        System.Diagnostics.Debug.WriteLine($"  Cell[{i}]: {cellValue}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PopulateDataGridView Exception: {ex}");
                MessageBox.Show($"Error displaying jobs: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatusLabel()
        {
            statusLabel.Text = $"Total Active Jobs: {_allJobs?.Count ?? 0}";
            System.Diagnostics.Debug.WriteLine($"UpdateStatusLabel: {statusLabel.Text}");
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_allJobs != null && jobsDataGridView != null)
            {
                ApplyFilters();
            }
        }

        private void ViewDetailsButton_Click(object sender, EventArgs e)
        {
            if (jobsDataGridView.SelectedRows.Count > 0)
            {
                var row = jobsDataGridView.SelectedRows[0];
                int jobID = Convert.ToInt32(row.Cells["JobID"]?.Value ?? 0);
                string jobTitle = row.Cells["JobTitle"]?.Value?.ToString() ?? "";
                string description = row.Cells["JobDescription"]?.Value?.ToString() ?? "";

                string details = $"Job: {jobTitle}\n\n{description}";
                MessageBox.Show(details, "Job Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a job to view details.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (jobsDataGridView.SelectedRows.Count > 0)
            {
                var row = jobsDataGridView.SelectedRows[0];
                int jobID = Convert.ToInt32(row.Cells["JobID"]?.Value ?? 0);
                string jobTitle = row.Cells["JobTitle"]?.Value?.ToString() ?? "";

                try
                {
                    // Check if applicant already applied for this job
                    if (_db.HasApplicantAppliedForJob(_applicantID, jobID))
                    {
                        MessageBox.Show("You have already applied for this job.", "Already Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Open ApplicationDraftForm
                    using (ApplicationDraftForm form = new ApplicationDraftForm(_db, _applicantID, jobID, jobTitle))
                    {
                        if (form.ShowDialog(this) == DialogResult.OK)
                        {
                            MessageBox.Show("Application submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadJobVacancies();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a job to apply.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            searchTextBox.Clear();
            LoadJobVacancies();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
