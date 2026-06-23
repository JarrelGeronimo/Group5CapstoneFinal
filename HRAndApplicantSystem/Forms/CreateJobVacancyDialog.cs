using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Services;
using System;
using System.Windows.Forms;

namespace HRAndApplicantSystem.Forms
{
    public partial class CreateJobVacancyDialog : Form
    {
        private readonly JobVacancyManagementService _jobService;
        private readonly string _username;
        private TextBox? jobTitleTextBox;
        private TextBox? jobDetailTextBox;

        public CreateJobVacancyDialog(JobVacancyManagementService jobService, string username)
        {
            InitializeComponent();
            _jobService = jobService;
            _username = username;
            
            this.Text = "Create New Job Vacancy";
            this.Size = new System.Drawing.Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeComponent()
        {
            // Title
            Label titleLabel = new Label();
            titleLabel.Text = "Create New Job Vacancy";
            titleLabel.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            titleLabel.Location = new System.Drawing.Point(20, 15);
            titleLabel.Width = 350;
            titleLabel.Height = 25;
            this.Controls.Add(titleLabel);

            // Main panel with scrolling
            Panel mainPanel = new Panel();
            mainPanel.Location = new System.Drawing.Point(20, 45);
            mainPanel.Size = new System.Drawing.Size(540, 350);
            mainPanel.AutoScroll = true;
            mainPanel.BackColor = System.Drawing.Color.White;

            int yPos = 10;

            // Job Title
            Label jobTitleLabel = new Label();
            jobTitleLabel.Text = "Job Title: *";
            jobTitleLabel.Location = new System.Drawing.Point(10, yPos);
            jobTitleLabel.Width = 300;
            jobTitleLabel.Height = 20;
            mainPanel.Controls.Add(jobTitleLabel);
            yPos += 25;

            jobTitleTextBox = new TextBox();
            jobTitleTextBox.Location = new System.Drawing.Point(10, yPos);
            jobTitleTextBox.Width = 500;
            jobTitleTextBox.Height = 30;
            jobTitleTextBox.Font = new System.Drawing.Font("Arial", 10);
            mainPanel.Controls.Add(jobTitleTextBox);
            yPos += 40;

            // Job Detail
            Label jobDetailLabel = new Label();
            jobDetailLabel.Text = "Job Description: *";
            jobDetailLabel.Location = new System.Drawing.Point(10, yPos);
            jobDetailLabel.Width = 300;
            jobDetailLabel.Height = 20;
            mainPanel.Controls.Add(jobDetailLabel);
            yPos += 25;

            jobDetailTextBox = new TextBox();
            jobDetailTextBox.Location = new System.Drawing.Point(10, yPos);
            jobDetailTextBox.Width = 500;
            jobDetailTextBox.Height = 150;
            jobDetailTextBox.Multiline = true;
            jobDetailTextBox.Font = new System.Drawing.Font("Arial", 10);
            jobDetailTextBox.ScrollBars = ScrollBars.Vertical;
            mainPanel.Controls.Add(jobDetailTextBox);
            yPos += 160;

            // Status (read-only, always "Open")
            Label statusLabel = new Label();
            statusLabel.Text = "Status: Open (new vacancies always start as Open)";
            statusLabel.Location = new System.Drawing.Point(10, yPos);
            statusLabel.Width = 500;
            statusLabel.Height = 20;
            statusLabel.Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Italic);
            statusLabel.ForeColor = System.Drawing.Color.Gray;
            mainPanel.Controls.Add(statusLabel);

            this.Controls.Add(mainPanel);

            // Button panel
            Panel buttonPanel = new Panel();
            buttonPanel.Location = new System.Drawing.Point(0, 410);
            buttonPanel.Size = new System.Drawing.Size(600, 60);
            buttonPanel.BackColor = System.Drawing.Color.WhiteSmoke;

            // Create Button
            Button createButton = new Button();
            createButton.Text = "Create";
            createButton.Size = new System.Drawing.Size(120, 35);
            createButton.Location = new System.Drawing.Point(300, 12);
            createButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            createButton.ForeColor = System.Drawing.Color.White;
            createButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            createButton.Click += CreateButton_Click;
            buttonPanel.Controls.Add(createButton);

            // Cancel Button
            Button cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Size = new System.Drawing.Size(120, 35);
            cancelButton.Location = new System.Drawing.Point(430, 12);
            cancelButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
            cancelButton.ForeColor = System.Drawing.Color.White;
            cancelButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            cancelButton.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            buttonPanel.Controls.Add(cancelButton);

            this.Controls.Add(buttonPanel);
        }

        private void CreateButton_Click(object? sender, EventArgs? e)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(jobTitleTextBox?.Text))
                {
                    MessageBox.Show("Job title is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    jobTitleTextBox?.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(jobDetailTextBox?.Text))
                {
                    MessageBox.Show("Job description is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    jobDetailTextBox?.Focus();
                    return;
                }

                // Create job
                var newJob = new JobVacancy
                {
                    JobTitle = jobTitleTextBox.Text.Trim(),
                    JobDetail = jobDetailTextBox.Text.Trim(),
                    Status = "Open",
                    DatePosted = DateTime.Now
                };

                if (_jobService.CreateJob(newJob, _username))
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to create job vacancy.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
