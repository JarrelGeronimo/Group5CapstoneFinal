using System;
using System.Windows.Forms;
using HRAndApplicantSystem.Database;
using ApplicationModel = HRAndApplicantSystem.Models.Application;

namespace HRAndApplicantSystem.Forms
{
    public partial class HiringDecisionDialog : Form
    {
        private readonly DatabaseHelper _db;
        private readonly int _applicationID;
        private readonly int _userRoleID;
        private string _hiringDecision; // "Accepted" or "Rejected"

        public string HiringDecision => _hiringDecision;
        public string DecisionReason => decisionReasonTextBox.Text.Trim();

        public HiringDecisionDialog(DatabaseHelper db, int applicationID, int userRoleID = 3)
        {
            InitializeComponent();
            _db = db;
            _applicationID = applicationID;
            _userRoleID = userRoleID;
            
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            this.Text = "Hiring Decision - Stage 3";
            this.Size = new System.Drawing.Size(600, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label titleLabel = new Label();
            titleLabel.Text = "STAGE 3: Final Hiring Decision (Manager/Admin Only)";
            titleLabel.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            titleLabel.Location = new System.Drawing.Point(20, 15);
            titleLabel.Size = new System.Drawing.Size(550, 25);
            this.Controls.Add(titleLabel);

            // Description
            Label descriptionLabel = new Label();
            descriptionLabel.Text = "Make the final hiring decision. The applicant has passed the interview stage. Accept or reject for employment.";
            descriptionLabel.Font = new System.Drawing.Font("Arial", 9);
            descriptionLabel.Location = new System.Drawing.Point(20, 45);
            descriptionLabel.Size = new System.Drawing.Size(550, 35);
            descriptionLabel.AutoSize = false;
            this.Controls.Add(descriptionLabel);

            // Decision Reason Label
            Label reasonLabel = new Label();
            reasonLabel.Text = "Decision Reason (Required):";
            reasonLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            reasonLabel.Location = new System.Drawing.Point(20, 90);
            reasonLabel.Size = new System.Drawing.Size(300, 20);
            this.Controls.Add(reasonLabel);

            // Decision Reason TextBox
            decisionReasonTextBox.Location = new System.Drawing.Point(20, 115);
            decisionReasonTextBox.Size = new System.Drawing.Size(550, 120);
            decisionReasonTextBox.Multiline = true;
            decisionReasonTextBox.ScrollBars = ScrollBars.Vertical;
            decisionReasonTextBox.Font = new System.Drawing.Font("Arial", 9);
            this.Controls.Add(decisionReasonTextBox);

            // Final Decision Label
            Label finalLabel = new Label();
            finalLabel.Text = "Final Decision:";
            finalLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            finalLabel.Location = new System.Drawing.Point(20, 245);
            finalLabel.Size = new System.Drawing.Size(300, 20);
            this.Controls.Add(finalLabel);

            // Accept Button
            acceptButton.Text = "✓ ACCEPT - Hire Applicant";
            acceptButton.Location = new System.Drawing.Point(20, 275);
            acceptButton.Size = new System.Drawing.Size(210, 55);
            acceptButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            acceptButton.ForeColor = System.Drawing.Color.White;
            acceptButton.Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold);
            acceptButton.FlatStyle = FlatStyle.Flat;
            acceptButton.Click += AcceptButton_Click;
            this.Controls.Add(acceptButton);

            // Reject Button
            rejectButton.Text = "✗ REJECT - Do Not Hire";
            rejectButton.Location = new System.Drawing.Point(240, 275);
            rejectButton.Size = new System.Drawing.Size(210, 55);
            rejectButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
            rejectButton.ForeColor = System.Drawing.Color.White;
            rejectButton.Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold);
            rejectButton.FlatStyle = FlatStyle.Flat;
            rejectButton.Click += RejectButton_Click;
            this.Controls.Add(rejectButton);

            // Cancel Button
            cancelButton.Text = "Cancel";
            cancelButton.Location = new System.Drawing.Point(460, 275);
            cancelButton.Size = new System.Drawing.Size(110, 55);
            cancelButton.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
            cancelButton.ForeColor = System.Drawing.Color.White;
            cancelButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(cancelButton);

            // Error Label
            errorLabel.Text = "";
            errorLabel.ForeColor = System.Drawing.Color.FromArgb(220, 20, 60);
            errorLabel.Location = new System.Drawing.Point(20, 345);
            errorLabel.Size = new System.Drawing.Size(550, 35);
            errorLabel.Font = new System.Drawing.Font("Arial", 9);
            errorLabel.AutoSize = false;
            this.Controls.Add(errorLabel);
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(decisionReasonTextBox.Text))
            {
                ShowError("Decision reason is required");
                return;
            }

            _hiringDecision = "Accepted";
            SaveDecision();
        }

        private void RejectButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(decisionReasonTextBox.Text))
            {
                ShowError("Decision reason is required");
                return;
            }

            _hiringDecision = "Rejected";
            SaveDecision();
        }

        private void SaveDecision()
        {
            try
            {
                string remarks = $"Final Hiring Decision (Stage 3): {_hiringDecision}. Reason: {DecisionReason}";
                bool success = _db.UpdateApplicationStatus(_applicationID, _hiringDecision, remarks, _db.GetRoleNameFromID(_userRoleID));

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowError("Failed to save hiring decision. Please try again.");
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

        private TextBox decisionReasonTextBox = new TextBox();
        private Button acceptButton = new Button();
        private Button rejectButton = new Button();
        private Button cancelButton = new Button();
        private Label errorLabel = new Label();
    }
}
