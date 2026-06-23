using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Services;
using System;
using System.Windows.Forms;

namespace HRAndApplicantSystem.Forms
{
    public partial class EditJobVacancyDialog : Form
    {
        private readonly JobVacancy _jobVacancy;
        private readonly JobVacancyManagementService _jobService;
        private readonly string _username;
        private TextBox? jobTitleTextBox;
        private TextBox? jobDetailTextBox;
        private ComboBox? statusComboBox;
        private Label? datePostedLabel;

        public EditJobVacancyDialog(JobVacancy job, JobVacancyManagementService jobService, string username)
        {
            _jobVacancy = job;
            _jobService = jobService;
            _username = username;
            
            InitializeComponent();
            
            this.Text = "Edit Job Vacancy";
            this.Size = new System.Drawing.Size(600, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeComponent()
        {
            // Title
            Label titleLabel = new Label();
            titleLabel.Text = $"Edit Job Vacancy - ID: {_jobVacancy.JobID}";
            titleLabel.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            titleLabel.Location = new System.Drawing.Point(20, 15);
            titleLabel.Width = 400;
            titleLabel.Height = 25;
            this.Controls.Add(titleLabel);

            // Main panel with scrolling
            Panel mainPanel = new Panel();
            mainPanel.Location = new System.Drawing.Point(20, 45);
            mainPanel.Size = new System.Drawing.Size(540, 400);
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
            jobTitleTextBox.Text = _jobVacancy.JobTitle ?? "";
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
            jobDetailTextBox.Text = _jobVacancy.JobDetail ?? "";
            jobDetailTextBox.Location = new System.Drawing.Point(10, yPos);
            jobDetailTextBox.Width = 500;
            jobDetailTextBox.Height = 120;
            jobDetailTextBox.Multiline = true;
            jobDetailTextBox.Font = new System.Drawing.Font("Arial", 10);
            jobDetailTextBox.ScrollBars = ScrollBars.Vertical;
            mainPanel.Controls.Add(jobDetailTextBox);
            yPos += 130;

            // Status
            Label statusLabel = new Label();
            statusLabel.Text = "Status: *";
            statusLabel.Location = new System.Drawing.Point(10, yPos);
            statusLabel.Width = 300;
            statusLabel.Height = 20;
            mainPanel.Controls.Add(statusLabel);
            yPos += 25;

            statusComboBox = new ComboBox();
            statusComboBox.Items.AddRange(new[] { "Open", "Closed" });
            statusComboBox.SelectedItem = _jobVacancy.Status ?? "Open";
            statusComboBox.Location = new System.Drawing.Point(10, yPos);
            statusComboBox.Width = 200;
            statusComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            mainPanel.Controls.Add(statusComboBox);
            yPos += 35;

            // Date Posted (read-only)
            Label datePostedLabel2 = new Label();
            datePostedLabel2.Text = "Date Posted:";
            datePostedLabel2.Location = new System.Drawing.Point(10, yPos);
            datePostedLabel2.Width = 300;
            datePostedLabel2.Height = 20;
            mainPanel.Controls.Add(datePostedLabel2);
            yPos += 25;

            datePostedLabel = new Label();
            datePostedLabel.Text = _jobVacancy.DatePosted?.ToString("yyyy-MM-dd HH:mm") ?? "N/A";
            datePostedLabel.Location = new System.Drawing.Point(10, yPos);
            datePostedLabel.Width = 500;
            datePostedLabel.Height = 25;
            datePostedLabel.Font = new System.Drawing.Font("Arial", 10);
            datePostedLabel.ForeColor = System.Drawing.Color.Gray;
            mainPanel.Controls.Add(datePostedLabel);

            this.Controls.Add(mainPanel);

            // Button panel
            Panel buttonPanel = new Panel();
            buttonPanel.Location = new System.Drawing.Point(0, 460);
            buttonPanel.Size = new System.Drawing.Size(600, 60);
            buttonPanel.BackColor = System.Drawing.Color.WhiteSmoke;

            // Save Button
            Button saveButton = new Button();
            saveButton.Text = "Save";
            saveButton.Size = new System.Drawing.Size(120, 35);
            saveButton.Location = new System.Drawing.Point(300, 12);
            saveButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            saveButton.ForeColor = System.Drawing.Color.White;
            saveButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            saveButton.Click += SaveButton_Click;
            buttonPanel.Controls.Add(saveButton);

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

        private void SaveButton_Click(object? sender, EventArgs? e)
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

                // Update job
                _jobVacancy.JobTitle = jobTitleTextBox.Text.Trim();
                _jobVacancy.JobDetail = jobDetailTextBox.Text.Trim();
                _jobVacancy.Status = statusComboBox?.SelectedItem?.ToString() ?? "Open";

                if (_jobService.UpdateJob(_jobVacancy, _username))
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to update job vacancy.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
