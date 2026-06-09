using System;
using System.Windows.Forms;
using HRAndApplicantSystem.Database;
using ApplicationModel = HRAndApplicantSystem.Models.Application;

namespace HRAndApplicantSystem.Forms
{
    public partial class InterviewDialog : Form
    {
        private readonly DatabaseHelper _db;
        private readonly int _applicationID;
        private string _interviewDecision; // "Interview" or "Rejected"

        public string InterviewDecision => _interviewDecision;
        public string InterviewNotes => interviewNotesTextBox.Text.Trim();
        public DateTime InterviewDate => interviewDatePicker.Value;
        public string InterviewTime => interviewTimeTextBox.Text.Trim();

        public InterviewDialog(DatabaseHelper db, int applicationID)
        {
            InitializeComponent();
            _db = db;
            _applicationID = applicationID;
            
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            this.Text = "Interview Scheduling - Stage 2";
            this.Size = new System.Drawing.Size(600, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label titleLabel = new Label();
            titleLabel.Text = "STAGE 2: Interview Scheduling & Assessment";
            titleLabel.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            titleLabel.Location = new System.Drawing.Point(20, 15);
            titleLabel.Size = new System.Drawing.Size(550, 25);
            this.Controls.Add(titleLabel);

            // Description
            Label descriptionLabel = new Label();
            descriptionLabel.Text = "Schedule an interview or mark the applicant as not suitable. After interview, record if they passed or failed.";
            descriptionLabel.Font = new System.Drawing.Font("Arial", 9);
            descriptionLabel.Location = new System.Drawing.Point(20, 45);
            descriptionLabel.Size = new System.Drawing.Size(550, 30);
            descriptionLabel.AutoSize = false;
            this.Controls.Add(descriptionLabel);

            // Interview Date Label
            Label dateLabel = new Label();
            dateLabel.Text = "Interview Date:";
            dateLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            dateLabel.Location = new System.Drawing.Point(20, 85);
            dateLabel.Size = new System.Drawing.Size(150, 20);
            this.Controls.Add(dateLabel);

            // Interview Date Picker
            interviewDatePicker.Location = new System.Drawing.Point(20, 110);
            interviewDatePicker.Size = new System.Drawing.Size(200, 25);
            interviewDatePicker.Format = DateTimePickerFormat.Short;
            this.Controls.Add(interviewDatePicker);

            // Interview Time Label
            Label timeLabel = new Label();
            timeLabel.Text = "Interview Time (HH:MM):";
            timeLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            timeLabel.Location = new System.Drawing.Point(240, 85);
            timeLabel.Size = new System.Drawing.Size(200, 20);
            this.Controls.Add(timeLabel);

            // Interview Time TextBox
            interviewTimeTextBox.Location = new System.Drawing.Point(240, 110);
            interviewTimeTextBox.Size = new System.Drawing.Size(100, 25);
            interviewTimeTextBox.Font = new System.Drawing.Font("Arial", 9);
            interviewTimeTextBox.Text = "14:30"; // Default time format example
            this.Controls.Add(interviewTimeTextBox);

            // Interview Notes Label
            Label notesLabel = new Label();
            notesLabel.Text = "Interview Notes (Required):";
            notesLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            notesLabel.Location = new System.Drawing.Point(20, 150);
            notesLabel.Size = new System.Drawing.Size(300, 20);
            this.Controls.Add(notesLabel);

            // Interview Notes TextBox
            interviewNotesTextBox.Location = new System.Drawing.Point(20, 175);
            interviewNotesTextBox.Size = new System.Drawing.Size(550, 120);
            interviewNotesTextBox.Multiline = true;
            interviewNotesTextBox.ScrollBars = ScrollBars.Vertical;
            interviewNotesTextBox.Font = new System.Drawing.Font("Arial", 9);
            this.Controls.Add(interviewNotesTextBox);

            // Decision Label
            Label decisionLabel = new Label();
            decisionLabel.Text = "Interview Outcome:";
            decisionLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            decisionLabel.Location = new System.Drawing.Point(20, 305);
            decisionLabel.Size = new System.Drawing.Size(300, 20);
            this.Controls.Add(decisionLabel);

            // Passed Button
            passedButton.Text = "✓ Applicant Passed";
            passedButton.Location = new System.Drawing.Point(20, 335);
            passedButton.Size = new System.Drawing.Size(200, 50);
            passedButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            passedButton.ForeColor = System.Drawing.Color.White;
            passedButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            passedButton.FlatStyle = FlatStyle.Flat;
            passedButton.Click += PassedButton_Click;
            this.Controls.Add(passedButton);

            // Failed Button
            failedButton.Text = "✗ Applicant Failed";
            failedButton.Location = new System.Drawing.Point(230, 335);
            failedButton.Size = new System.Drawing.Size(200, 50);
            failedButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
            failedButton.ForeColor = System.Drawing.Color.White;
            failedButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            failedButton.FlatStyle = FlatStyle.Flat;
            failedButton.Click += FailedButton_Click;
            this.Controls.Add(failedButton);

            // Cancel Button
            cancelButton.Text = "Cancel";
            cancelButton.Location = new System.Drawing.Point(440, 335);
            cancelButton.Size = new System.Drawing.Size(130, 50);
            cancelButton.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
            cancelButton.ForeColor = System.Drawing.Color.White;
            cancelButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(cancelButton);

            // Error Label
            errorLabel.Text = "";
            errorLabel.ForeColor = System.Drawing.Color.FromArgb(220, 20, 60);
            errorLabel.Location = new System.Drawing.Point(20, 395);
            errorLabel.Size = new System.Drawing.Size(550, 40);
            errorLabel.Font = new System.Drawing.Font("Arial", 9);
            errorLabel.AutoSize = false;
            this.Controls.Add(errorLabel);
        }

        private void PassedButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            _interviewDecision = "Interview";
            SaveInterview();
        }

        private void FailedButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            _interviewDecision = "Rejected";
            SaveInterview();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(interviewNotesTextBox.Text))
            {
                ShowError("Interview notes are required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(interviewTimeTextBox.Text))
            {
                ShowError("Interview time is required (HH:MM format)");
                return false;
            }

            // Basic time validation
            if (!System.Text.RegularExpressions.Regex.IsMatch(interviewTimeTextBox.Text, @"^\d{1,2}:\d{2}$"))
            {
                ShowError("Invalid time format. Use HH:MM (e.g., 14:30)");
                return false;
            }

            return true;
        }

        private void SaveInterview()
        {
            try
            {
                string remarks = $"Interview (Stage 2) - Date: {InterviewDate:yyyy-MM-dd} {InterviewTime}. Outcome: {_interviewDecision}. Notes: {InterviewNotes}";
                bool success = _db.UpdateApplicationStatus(_applicationID, _interviewDecision, remarks, "HR Interview");

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowError("Failed to save interview details. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            errorLabel.Text = message;
            errorLabel.ForeColor = System.Drawing.Color.FromArgb(220, 20, 60);
        }

        private DateTimePicker interviewDatePicker = new DateTimePicker();
        private TextBox interviewTimeTextBox = new TextBox();
        private TextBox interviewNotesTextBox = new TextBox();
        private Button passedButton = new Button();
        private Button failedButton = new Button();
        private Button cancelButton = new Button();
        private Label errorLabel = new Label();
    }
}
