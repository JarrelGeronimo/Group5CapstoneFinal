using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Forms
{
    public partial class JobVacancyControl : UserControl
    {
        private readonly DatabaseHelper db;
        private List<JobVacancy> allJobs;

        public JobVacancyControl()
        {
            InitializeComponent();
            db = new DatabaseHelper();
            allJobs = new List<JobVacancy>();
        }

        private void JobVacancyControl_Load(object sender, EventArgs e)
        {
            LoadJobs();
        }

        private void LoadJobs()
        {
            try
            {
                allJobs = db.GetAllJobVacancies();
                dgvJobs.DataSource = null;
                dgvJobs.DataSource = allJobs;

                // Set column headers
                if (dgvJobs.Columns["JobID"] != null)
                    dgvJobs.Columns["JobID"].HeaderText = "ID";
                if (dgvJobs.Columns["JobTitle"] != null)
                    dgvJobs.Columns["JobTitle"].HeaderText = "Job Title";
                if (dgvJobs.Columns["Status"] != null)
                    dgvJobs.Columns["Status"].HeaderText = "Status";
                
                // Hide detail column
                if (dgvJobs.Columns["JobDetail"] != null)
                    dgvJobs.Columns["JobDetail"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading jobs: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim().ToLower();
            
            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadJobs();
                return;
            }

            try
            {
                var filtered = new List<JobVacancy>();
                foreach (var job in allJobs)
                {
                    if (job.JobTitle.ToLower().Contains(searchTerm) || 
                        job.JobDetail.ToLower().Contains(searchTerm))
                    {
                        filtered.Add(job);
                    }
                }

                dgvJobs.DataSource = null;
                dgvJobs.DataSource = filtered;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching: {ex.Message}", "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (dgvJobs.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a job to apply for.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedJob = dgvJobs.SelectedRows[0].DataBoundItem as JobVacancy;
            if (selectedJob == null)
                return;

            MessageBox.Show($"You selected: {selectedJob.JobTitle}\n\nApplication workflow would start here.", 
                "Job Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
