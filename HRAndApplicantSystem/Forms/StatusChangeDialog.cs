using System;
using System.Windows.Forms;
using HRAndApplicantSystem.Database;
using ApplicationModel = HRAndApplicantSystem.Models.Application;

namespace HRAndApplicantSystem.Forms
{
    public partial class StatusChangeDialog : Form
    {
        private readonly DatabaseHelper _db;
        private readonly int _applicationID;
        private readonly string _currentStatus;
        private string _newStatus;

        public string NewStatus => _newStatus;
        public string Notes => notesTextBox.Text.Trim();

        public StatusChangeDialog(DatabaseHelper db, int applicationID, string currentStatus)
        {
            InitializeComponent();
            _db = db;
            _applicationID = applicationID;
            _currentStatus = currentStatus;
            
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            this.Text = "Change Application Status";
            this.Size = new System.Drawing.Size(500, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Current Status Label
            Label currentStatusLabel = new Label();
            currentStatusLabel.Text = $"Current Status: {_currentStatus}";
            currentStatusLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            currentStatusLabel.Location = new System.Drawing.Point(20, 20);
            currentStatusLabel.Size = new System.Drawing.Size(450, 25);
            currentStatusLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.Controls.Add(currentStatusLabel);

            // New Status Label
            Label newStatusLabel = new Label();
            newStatusLabel.Text = "Select New Status:";
            newStatusLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            newStatusLabel.Location = new System.Drawing.Point(20, 60);
            newStatusLabel.Size = new System.Drawing.Size(200, 25);
            this.Controls.Add(newStatusLabel);

            // Status ComboBox
            statusComboBox.Location = new System.Drawing.Point(20, 90);
            statusComboBox.Size = new System.Drawing.Size(450, 25);
            statusComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            statusComboBox.Items.AddRange(new string[] { "Pending", "Screening", "Interview", "Accepted", "Rejected" });
            statusComboBox.SelectedIndex = -1;
            this.Controls.Add(statusComboBox);

            // Notes Label
            Label notesLabel = new Label();
            notesLabel.Text = "Notes/Reason (Optional):";
            notesLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            notesLabel.Location = new System.Drawing.Point(20, 135);
            notesLabel.Size = new System.Drawing.Size(200, 25);
            this.Controls.Add(notesLabel);

            // Notes TextBox
            notesTextBox.Location = new System.Drawing.Point(20, 165);
            notesTextBox.Size = new System.Drawing.Size(450, 150);
            notesTextBox.Multiline = true;
            notesTextBox.ScrollBars = ScrollBars.Vertical;
            notesTextBox.Font = new System.Drawing.Font("Arial", 9);
            this.Controls.Add(notesTextBox);

            // OK Button
            okButton.Text = "Confirm";
            okButton.Location = new System.Drawing.Point(280, 335);
            okButton.Size = new System.Drawing.Size(90, 35);
            okButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            okButton.ForeColor = System.Drawing.Color.White;
            okButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            okButton.FlatStyle = FlatStyle.Flat;
            okButton.Click += OkButton_Click;
            this.Controls.Add(okButton);

            // Cancel Button
            cancelButton.Text = "Cancel";
            cancelButton.Location = new System.Drawing.Point(380, 335);
            cancelButton.Size = new System.Drawing.Size(90, 35);
            cancelButton.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
            cancelButton.ForeColor = System.Drawing.Color.White;
            cancelButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(cancelButton);

            // Error Label (initially hidden)
            errorLabel.Text = "";
            errorLabel.ForeColor = System.Drawing.Color.FromArgb(220, 20, 60);
            errorLabel.Location = new System.Drawing.Point(20, 375);
            errorLabel.Size = new System.Drawing.Size(450, 20);
            errorLabel.Font = new System.Drawing.Font("Arial", 9);
            this.Controls.Add(errorLabel);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            // Validation
            if (statusComboBox.SelectedIndex == -1)
            {
                ShowError("Please select a new status");
                return;
            }

            _newStatus = statusComboBox.SelectedItem.ToString();

            if (_newStatus == _currentStatus)
            {
                ShowError("New status cannot be the same as current status");
                return;
            }

            try
            {
                // Validate status transition rules
                if (!IsValidTransition(_currentStatus, _newStatus))
                {
                    ShowError($"Cannot transition from '{_currentStatus}' to '{_newStatus}'");
                    return;
                }

                // Update application status in database (includes audit logging)
                string remarks = $"Status changed from {_currentStatus} to {_newStatus}. Notes: {Notes}";
                bool success = _db.UpdateApplicationStatus(_applicationID, _newStatus, remarks, "HR User");

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowError("Failed to update application status. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating status: {ex.Message}");
            }
        }

        private bool IsValidTransition(string fromStatus, string toStatus)
        {
            // Define valid status transitions
            // Pending -> Screening, Rejected
            // Screening -> Interview, Rejected
            // Interview -> Accepted, Rejected
            // Accepted -> (final, no transition)
            // Rejected -> (final, no transition)

            return (fromStatus, toStatus) switch
            {
                ("Pending", "Screening") => true,
                ("Pending", "Rejected") => true,
                ("Screening", "Interview") => true,
                ("Screening", "Rejected") => true,
                ("Interview", "Accepted") => true,
                ("Interview", "Rejected") => true,
                // Allow reverting back for error correction
                ("Screening", "Pending") => true,
                ("Interview", "Screening") => true,
                _ => false
            };
        }

        private void ShowError(string message)
        {
            errorLabel.Text = message;
            errorLabel.ForeColor = System.Drawing.Color.FromArgb(220, 20, 60);
        }

        private ComboBox statusComboBox = new ComboBox();
        private TextBox notesTextBox = new TextBox();
        private Button okButton = new Button();
        private Button cancelButton = new Button();
        private Label errorLabel = new Label();
    }
}
