using System;
using System.Windows.Forms;
using HRAndApplicantSystem.Database;
using ApplicationModel = HRAndApplicantSystem.Models.Application;

namespace HRAndApplicantSystem.Forms
{
    public partial class InterviewSchedulingDialog : Form
    {
        private readonly DatabaseHelper _db;
        private readonly int _applicationID;
        private DateTimePicker? interviewDatePicker;
        private TextBox? interviewTimeTextBox;
        private RadioButton? faceToFaceRadio;
        private RadioButton? onlineRadio;
        private TextBox? locationTextBox;

        public bool SchedulingConfirmed { get; private set; }
        public DateTime ScheduledDate => interviewDatePicker?.Value ?? DateTime.Now;
        public string ScheduledTime => interviewTimeTextBox?.Text.Trim() ?? "09:00";
        public string InterviewMode => onlineRadio?.Checked == true ? "Online" : "Face-to-Face";
        public string Location => locationTextBox?.Text.Trim() ?? "";

        public InterviewSchedulingDialog(DatabaseHelper db, int applicationID)
        {
            InitializeComponent();
            _db = db;
            _applicationID = applicationID;
            SchedulingConfirmed = false;

            InitializeDialog();
        }

        private void InitializeDialog()
        {
            this.Text = "Schedule Interview - Stage 2";
            this.Size = new System.Drawing.Size(600, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label titleLabel = new Label();
            titleLabel.Text = "STAGE 2: SCHEDULE INTERVIEW";
            titleLabel.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            titleLabel.Location = new System.Drawing.Point(20, 20);
            titleLabel.Size = new System.Drawing.Size(560, 30);
            this.Controls.Add(titleLabel);

            // Interview Date
            Label dateLabel = new Label();
            dateLabel.Text = "Interview Date:";
            dateLabel.Location = new System.Drawing.Point(20, 70);
            dateLabel.Size = new System.Drawing.Size(150, 25);
            dateLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            this.Controls.Add(dateLabel);

            interviewDatePicker = new DateTimePicker();
            interviewDatePicker.Location = new System.Drawing.Point(180, 70);
            interviewDatePicker.Size = new System.Drawing.Size(200, 30);
            interviewDatePicker.Value = DateTime.Now.AddDays(1); // Default to tomorrow
            interviewDatePicker.MinDate = DateTime.Now.Date;
            this.Controls.Add(interviewDatePicker);

            // Interview Time
            Label timeLabel = new Label();
            timeLabel.Text = "Interview Time (HH:MM):";
            timeLabel.Location = new System.Drawing.Point(20, 110);
            timeLabel.Size = new System.Drawing.Size(150, 25);
            timeLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            this.Controls.Add(timeLabel);

            interviewTimeTextBox = new TextBox();
            interviewTimeTextBox.Location = new System.Drawing.Point(180, 110);
            interviewTimeTextBox.Size = new System.Drawing.Size(200, 30);
            interviewTimeTextBox.Text = "09:00";
            interviewTimeTextBox.Placeholder = "09:00";
            this.Controls.Add(interviewTimeTextBox);

            // Interview Mode
            Label modeLabel = new Label();
            modeLabel.Text = "Interview Mode:";
            modeLabel.Location = new System.Drawing.Point(20, 155);
            modeLabel.Size = new System.Drawing.Size(150, 25);
            modeLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            this.Controls.Add(modeLabel);

            faceToFaceRadio = new RadioButton();
            faceToFaceRadio.Text = "Face-to-Face";
            faceToFaceRadio.Location = new System.Drawing.Point(180, 155);
            faceToFaceRadio.Size = new System.Drawing.Size(150, 25);
            faceToFaceRadio.Checked = true;
            faceToFaceRadio.CheckedChanged += (s, e) => UpdateLocationLabel();
            this.Controls.Add(faceToFaceRadio);

            onlineRadio = new RadioButton();
            onlineRadio.Text = "Online (Video Call)";
            onlineRadio.Location = new System.Drawing.Point(330, 155);
            onlineRadio.Size = new System.Drawing.Size(180, 25);
            onlineRadio.CheckedChanged += (s, e) => UpdateLocationLabel();
            this.Controls.Add(onlineRadio);

            // Location/Platform
            Label locationLabel = new Label();
            locationLabel.Name = "locationLabel";
            locationLabel.Text = "Location/Venue:";
            locationLabel.Location = new System.Drawing.Point(20, 200);
            locationLabel.Size = new System.Drawing.Size(150, 25);
            locationLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            this.Controls.Add(locationLabel);

            locationTextBox = new TextBox();
            locationTextBox.Location = new System.Drawing.Point(180, 200);
            locationTextBox.Size = new System.Drawing.Size(350, 60);
            locationTextBox.Multiline = true;
            locationTextBox.Placeholder = "Enter location or meeting link";
            this.Controls.Add(locationTextBox);

            // Schedule Button
            Button scheduleButton = new Button();
            scheduleButton.Text = "✓ Schedule Interview";
            scheduleButton.Location = new System.Drawing.Point(200, 420);
            scheduleButton.Size = new System.Drawing.Size(150, 50);
            scheduleButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            scheduleButton.ForeColor = System.Drawing.Color.White;
            scheduleButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            scheduleButton.FlatStyle = FlatStyle.Flat;
            scheduleButton.Click += ScheduleButton_Click;
            this.Controls.Add(scheduleButton);

            // Cancel Button
            Button cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Location = new System.Drawing.Point(360, 420);
            cancelButton.Size = new System.Drawing.Size(120, 50);
            cancelButton.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
            cancelButton.ForeColor = System.Drawing.Color.White;
            cancelButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(cancelButton);
        }

        private void UpdateLocationLabel()
        {
            var locationLabel = this.Controls["locationLabel"] as Label;
            if (locationLabel != null)
            {
                locationLabel.Text = onlineRadio?.Checked == true ? "Meeting Link/Platform:" : "Location/Venue:";
            }
        }

        private void ScheduleButton_Click(object? sender, EventArgs? e)
        {
            if (!ValidateInput())
                return;

            SaveSchedule();
        }

        private bool ValidateInput()
        {
            // Validate date is not in the past
            if (interviewDatePicker?.Value.Date < DateTime.Today)
            {
                ShowError("Interview date cannot be in the past. Please select today or a future date.");
                return false;
            }

            // Validate time format
            if (!System.Text.RegularExpressions.Regex.IsMatch(interviewTimeTextBox?.Text ?? "", @"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$"))
            {
                ShowError("Invalid time format. Use HH:MM (e.g., 14:30)");
                return false;
            }

            // Validate location
            if (string.IsNullOrWhiteSpace(locationTextBox?.Text))
            {
                ShowError($"Please enter {(onlineRadio?.Checked == true ? "meeting link" : "location")}");
                return false;
            }

            return true;
        }

        private void SaveSchedule()
        {
            try
            {
                // Update status to Interview
                string remarks = $"Interview scheduled for {ScheduledDate:yyyy-MM-dd} at {ScheduledTime}. Mode: {InterviewMode}. {(InterviewMode == "Online" ? "Link/Platform" : "Location")}: {Location}";
                bool success = _db.UpdateApplicationStatus(_applicationID, "Interview", remarks, "HR Scheduling");

                if (success)
                {
                    SchedulingConfirmed = true;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowError("Failed to schedule interview. Please try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[InterviewSchedulingDialog] Error: {ex.Message}");
                ShowError($"Error: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
