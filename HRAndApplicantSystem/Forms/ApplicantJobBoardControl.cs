using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Forms
{
    public partial class ApplicantJobBoardControl : UserControl
    {
        private readonly IJobVacancyRepository _jobVacancyRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly Applicant _currentApplicant;
        private List<JobVacancy> _allOpenJobs;

        
        public ApplicantJobBoardControl(Applicant loggedInApplicant)
        {
            InitializeComponent();
            
            var dbHelper = new DatabaseHelper();
            _jobVacancyRepository = new JobVacancyRepository(dbHelper);
            _applicationRepository = new ApplicationRepository(dbHelper);
            _currentApplicant = loggedInApplicant;
        }

        private void ApplicantJobBoardControl_Load(object sender, EventArgs e)
        {
            LoadAvailableJobs();
        }

        private void LoadAvailableJobs()
        {
            try
            {
                
                var totalVacancies = _jobVacancyRepository.GetAllJobVacancies();
                _allOpenJobs = totalVacancies.Where(j => j.Status.Equals("Open", StringComparison.OrdinalIgnoreCase)).ToList();

                DisplayJobsInGrid(_allOpenJobs);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading job vacancies: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayJobsInGrid(List<JobVacancy> list)
        {
            dgvJobs.DataSource = null;
            dgvJobs.DataSource = list;

            
            if (dgvJobs.Columns["JobID"] != null) dgvJobs.Columns["JobID"].Visible = false; // Itago ang ID block
            if (dgvJobs.Columns["JobTitle"] != null) dgvJobs.Columns["JobTitle"].HeaderText = "Available Positions";
            if (dgvJobs.Columns["Status"] != null) dgvJobs.Columns["Status"].HeaderText = "Status";
            if (dgvJobs.Columns["JobDetail"] != null) dgvJobs.Columns["JobDetail"].Visible = false;
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string term = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(term))
            {
                MessageBox.Show("Please type a keyword or title to search.", "Search Query Empty", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            
            var filtered = _allOpenJobs
                .Where(j => j.JobTitle.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();

            DisplayJobsInGrid(filtered);
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            DisplayJobsInGrid(_allOpenJobs);
        }

        private void dgvJobs_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvJobs.SelectedRows.Count == 0)
            {
                ResetDetailsPanel();
                return;
            }

            var selectedJob = (JobVacancy)dgvJobs.SelectedRows[0].DataBoundItem;

            
            lblJobTitle.Text = selectedJob.JobTitle;
            lblJobStatus.Text = $"Status: {selectedJob.Status}";
            txtJobDescription.Text = selectedJob.JobDetail;
            btnApply.Enabled = true;
        }

        private void ResetDetailsPanel()
        {
            lblJobTitle.Text = "Select a Position";
            lblJobStatus.Text = "Status: --";
            txtJobDescription.Clear();
            btnApply.Enabled = false;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (dgvJobs.SelectedRows.Count == 0) return;
            var selectedJob = (JobVacancy)dgvJobs.SelectedRows[0].DataBoundItem;

            
            if (selectedJob.Status != "Open")
            {
                MessageBox.Show($"The position '{selectedJob.JobTitle}' is no longer accepting submissions.", "Application Closed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            bool alreadyApplied = _applicationRepository.HasApplicantAppliedForJob(_currentApplicant.ApplicantID, selectedJob.JobID);
            
            if (alreadyApplied)
            {
                MessageBox.Show($"You have already submitted an application for the '{selectedJob.JobTitle}' role.\nDuplicate records are locked.", 
                    "Application Lockout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            
            using (var appDialog = new ApplicationDialogForm(selectedJob, _currentApplicant))
            {
                if (appDialog.ShowDialog(this) == DialogResult.OK)
                {
                    
                    LoadAvailableJobs();
                    ResetDetailsPanel();
                }
            }
        }
    }
}
