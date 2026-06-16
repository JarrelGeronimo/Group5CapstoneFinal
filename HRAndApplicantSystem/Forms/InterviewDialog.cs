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
        private string? _interviewDecision; // "For Final Review" or "Rejected"
        private TextBox? interviewNotesTextBox;
        private Button? approveButton;
        private Button? rejectButton;
        private Button? cancelButton;
        private Label? errorLabel;

        public string InterviewDecision => _interviewDecision ?? "";
        public string InterviewNotes => interviewNotesTextBox?.Text.Trim() ?? "";

        public InterviewDialog(DatabaseHelper db, int applicationID)
        {
            InitializeComponent();
            _db = db;
            _applicationID = applicationID;
            
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            this.Text = "Interview Evaluation - Stage 2";
            this.Size = new System.Drawing.Size(600, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label titleLabel = new Label();
            titleLabel.Text = "STAGE 2: INTERVIEW EVALUATION";
            titleLabel.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            titleLabel.Location = new System.Drawing.Point(20, 15);
            titleLabel.Size = new System.Drawing.Size(550, 25);
            this.Controls.Add(titleLabel);

            // Description
            Label descriptionLabel = new Label();
            descriptionLabel.Text = "Evaluate the interview. Approve to move to Final Review or reject the applicant.";
            descriptionLabel.Font = new System.Drawing.Font("Arial", 9);
            descriptionLabel.Location = new System.Drawing.Point(20, 45);
            descriptionLabel.Size = new System.Drawing.Size(550, 30);
            descriptionLabel.AutoSize = false;
            this.Controls.Add(descriptionLabel);

            // Interview Notes Label
            Label notesLabel = new Label();
            notesLabel.Text = "Interview Feedback (Required):";
            notesLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            notesLabel.Location = new System.Drawing.Point(20, 85);
            notesLabel.Size = new System.Drawing.Size(300, 20);
            this.Controls.Add(notesLabel);

            // Interview Notes TextBox
            interviewNotesTextBox = new TextBox();
            interviewNotesTextBox.Location = new System.Drawing.Point(20, 110);
            interviewNotesTextBox.Size = new System.Drawing.Size(550, 200);
            interviewNotesTextBox.Multiline = true;
            interviewNotesTextBox.ScrollBars = ScrollBars.Vertical;
            interviewNotesTextBox.Font = new System.Drawing.Font("Arial", 9);
            this.Controls.Add(interviewNotesTextBox);

            // Decision Label
            Label decisionLabel = new Label();
            decisionLabel.Text = "Decision:";
            decisionLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            decisionLabel.Location = new System.Drawing.Point(20, 320);
            decisionLabel.Size = new System.Drawing.Size(300, 20);
            this.Controls.Add(decisionLabel);

            // Approve Button
            approveButton = new Button();
            approveButton.Text = "✓ Approve for Final Review";
            approveButton.Location = new System.Drawing.Point(20, 350);
            approveButton.Size = new System.Drawing.Size(200, 50);
            approveButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            approveButton.ForeColor = System.Drawing.Color.White;
            approveButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            approveButton.FlatStyle = FlatStyle.Flat;
            approveButton.Click += ApproveButton_Click;
            this.Controls.Add(approveButton);

            // Reject Button
            rejectButton = new Button();
            rejectButton.Text = "✗ Reject";
            rejectButton.Location = new System.Drawing.Point(230, 350);
            rejectButton.Size = new System.Drawing.Size(150, 50);
            rejectButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
            rejectButton.ForeColor = System.Drawing.Color.White;
            rejectButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            rejectButton.FlatStyle = FlatStyle.Flat;
            rejectButton.Click += RejectButton_Click;
            this.Controls.Add(rejectButton);

            // Cancel Button
            cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Location = new System.Drawing.Point(390, 350);
            cancelButton.Size = new System.Drawing.Size(130, 50);
            cancelButton.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
            cancelButton.ForeColor = System.Drawing.Color.White;
            cancelButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(cancelButton);

            // Error Label
            errorLabel = new Label();
            errorLabel.Text = "";
            errorLabel.ForeColor = System.Drawing.Color.FromArgb(220, 20, 60);
            errorLabel.Location = new System.Drawing.Point(20, 405);
            errorLabel.Size = new System.Drawing.Size(550, 30);
            errorLabel.Font = new System.Drawing.Font("Arial", 9);
            errorLabel.AutoSize = false;
            this.Controls.Add(errorLabel);
        }

        private void ApproveButton_Click(object? sender, EventArgs? e)
        {
            if (!ValidateInput())
                return;

            _interviewDecision = "For Final Review";
            SaveEvaluation();
        }

        private void RejectButton_Click(object? sender, EventArgs? e)
        {
            if (!ValidateInput())
                return;

            _interviewDecision = "Rejected";
            SaveEvaluation();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(interviewNotesTextBox?.Text))
            {
                ShowError("Interview feedback is required");
                return false;
            }

            return true;
        }

        private void SaveEvaluation()
        {
            try
            {
                string remarks = $"Interview Evaluation (Stage 2) - Outcome: {_interviewDecision}. Feedback: {InterviewNotes}";
                bool success = _db.UpdateApplicationStatus(_applicationID, _interviewDecision ?? "", remarks, "HR Interview");

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowError("Failed to save evaluation. Please try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[InterviewDialog] Error: {ex.Message}");
                ShowError($"Error: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            if (errorLabel != null)
            {
                errorLabel.Text = message;
                errorLabel.ForeColor = System.Drawing.Color.FromArgb(220, 20, 60);
            }
        }
    }
}
