using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Utilities;

namespace HRAndApplicantSystem.Forms
{
    public partial class JobVacancyManagementControl : UserControl
    {
        private readonly DatabaseHelper db;

        public JobVacancyManagementControl()
        {
            InitializeComponent();
            db = new DatabaseHelper();
        }

        private void JobVacancyControl_Load(object sender, EventArgs e)
        {
            RefreshGridData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshGridData();
        }

        private void RefreshGridData()
        {
            try
            {
                var vacancies = db.GetAllJobVacancies();
                dgvVacancies.DataSource = null;
                dgvVacancies.DataSource = vacancies;

                //
                if (dgvVacancies.Columns["JobID"] != null) dgvVacancies.Columns["JobID"].HeaderText = "ID";
                if (dgvVacancies.Columns["JobTitle"] != null) dgvVacancies.Columns["JobTitle"].HeaderText = "Job Title";
                if (dgvVacancies.Columns["Status"] != null) dgvVacancies.Columns["Status"].HeaderText = "Status";

                // 
                if (dgvVacancies.Columns["JobDetail"] != null) dgvVacancies.Columns["JobDetail"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading jobs: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvVacancies_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVacancies.SelectedRows.Count == 0)
            {
                lblBreakdownData.Text = "Select a job vacancy to view application status breakdown.";
                return;
            }

            var selectedJob = (JobVacancy)dgvVacancies.SelectedRows[0].DataBoundItem;
            UpdatePipelineBreakdown(selectedJob.JobID);
        }

        private void UpdatePipelineBreakdown(int jobId)
        {
            var applications = db.GetApplicationsByJob(jobId);

            int total = 0;
            int submitted = 0, underReview = 0, shortlisted = 0;
            int interview = 0, accepted = 0, rejected = 0;

            foreach (var app in applications)
            {
                string s = (string)app.Status;
                if (s == ApplicationStatus.Draft) continue; // Laktawan ang draft items gaya ng sa console

                total++;
                switch (s)
                {
                    case ApplicationStatus.Submitted: submitted++; break;
                    case ApplicationStatus.UnderReview: underReview++; break;
                    case ApplicationStatus.Shortlisted: shortlisted++; break;
                    case ApplicationStatus.InterviewScheduled: interview++; break;
                    case ApplicationStatus.Accepted: accepted++; break;
                    case ApplicationStatus.Blinded: 
                    case ApplicationStatus.Rejected: rejected++; break;
                }
            }

            //
            lblBreakdownData.Text =
                $"Total Applied:  {total}\n" +
                $"───────────────────\n" +
                $"Submitted:      {submitted}\n" +
                $"Under Review:   {underReview}\n" +
                $"Shortlisted:    {shortlisted}\n" +
                $"Interviewing:   {interview}\n" +
                $"Accepted:       {accepted}\n" +
                $"Rejected:       {rejected}";
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            using (var dialog = new JobVacancyDialogForm())
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    if (db.CreateJobVacancy(dialog.JobData))
                    {
                        MessageBox.Show("Job vacancy created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshGridData();
                    }
                    else
                    {
                        MessageBox.Show("Failed to create vacancy. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvVacancies.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a job vacancy to edit.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedJob = (JobVacancy)dgvVacancies.SelectedRows[0].DataBoundItem;

            using (var dialog = new JobVacancyDialogForm(selectedJob))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    if (db.UpdateJobVacancy(dialog.JobData))
                    {
                        MessageBox.Show("Job vacancy updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshGridData();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update vacancy details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvVacancies.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a job vacancy to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedJob = (JobVacancy)dgvVacancies.SelectedRows[0].DataBoundItem;

            var confirm = MessageBox.Show(
                $"⚠️ WARNING: This will permanently delete '{selectedJob.JobTitle}' and all related data.\nAre you absolutely sure?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                if (db.DeleteJobVacancy(selectedJob.JobID))
                {
                    MessageBox.Show("Job vacancy deleted completely.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshGridData();
                }
                else
                {
                    MessageBox.Show("Failed to delete from the database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
