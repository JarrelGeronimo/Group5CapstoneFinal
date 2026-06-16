using System;
using System.Windows.Forms;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Forms
{
    public partial class JobVacancyDialogForm : Form
    {
        
        public JobVacancy JobData { get; private set; }
        private readonly bool isEditMode;

        
        public JobVacancyDialogForm()
        {
            InitializeComponent();
            this.Text = "Create New Job Vacancy";
            cmbStatus.SelectedIndex = 0; // Default: Open
            JobData = new JobVacancy();
            isEditMode = false;
        }

        
        public JobVacancyDialogForm(JobVacancy existingJob)
        {
            InitializeComponent();
            this.Text = "Edit Job Vacancy Details";

            
            JobData = existingJob;
            txtJobTitle.Text = existingJob.JobTitle;
            txtDetails.Text = existingJob.JobDetail;
            cmbStatus.SelectedItem = existingJob.Status;

            isEditMode = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            
            string title = txtJobTitle.Text.Trim();
            string details = txtDetails.Text.Trim();

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Error: Job title cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (title.Length > 100)
            {
                MessageBox.Show("Error: Job title cannot exceed 100 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(details))
            {
                MessageBox.Show("Error: Job details cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            JobData.JobTitle = title;
            JobData.JobDetail = details;
            JobData.Status = cmbStatus.SelectedItem?.ToString() ?? "Open";

            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
