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
                LoadJobVacancies();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vacancies: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadJobVacancies()
        {
            try
            {
                // Get all active job vacancies
                _allJobs = _db.GetActiveJobsAsDynamic();
                ApplyFilters();
                UpdateStatusLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading jobs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_allJobs == null) return;

            var filtered = _allJobs;

            // Filter by search term
            string searchTerm = searchTextBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filtered = filtered.FindAll(j =>
                    (j.JobTitle?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (j.CompanyName?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (j.JobDescription?.ToString() ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }

            PopulateDataGridView(filtered);
        }

        private void PopulateDataGridView(List<dynamic> jobs)
        {
            jobsDataGridView.DataSource = null;
            jobsDataGridView.DataSource = jobs;

            // Auto-size columns
            jobsDataGridView.AutoResizeColumns();
            jobsDataGridView.AutoResizeRows();
        }

        private void UpdateStatusLabel()
        {
            statusLabel.Text = $"Total Active Jobs: {_allJobs?.Count ?? 0}";
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
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
