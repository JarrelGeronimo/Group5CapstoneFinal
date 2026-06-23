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
        private readonly int _userRoleID;
        private string? _interviewDecision; // "For Final Review" or "Rejected"
        private TextBox? interviewNotesTextBox;
        private TextBox? scoreTextBox;
        private RadioButton? passRadio;
        private RadioButton? failRadio;
        private Button? approveButton;
        private Button? rejectButton;
        private Button? cancelButton;
        private Label? errorLabel;

        public string InterviewDecision => _interviewDecision ?? "";
        public string InterviewNotes => interviewNotesTextBox?.Text.Trim() ?? "";
        public int InterviewScore => int.TryParse(scoreTextBox?.Text.Trim(), out int score) ? Math.Clamp(score, 0, 100) : 0;
        public string InterviewResult => passRadio?.Checked == true ? "Pass" : "Fail";

        public InterviewDialog(DatabaseHelper db, int applicationID, int userRoleID = 2)
        {
            InitializeComponent();
            _db = db;
            _applicationID = applicationID;
            _userRoleID = userRoleID;
            
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            this.Text = "Interview Evaluation - Stage 2";
            this.Size = new System.Drawing.Size(600, 700);
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
            descriptionLabel.Text = "Evaluate the interview and provide score. This will be recorded in the InterviewEvaluations table.";
            descriptionLabel.Font = new System.Drawing.Font("Arial", 9);
            descriptionLabel.Location = new System.Drawing.Point(20, 45);
            descriptionLabel.Size = new System.Drawing.Size(550, 30);
            descriptionLabel.AutoSize = false;
            this.Controls.Add(descriptionLabel);

            int currentY = 85;

            // Interview Score Label
            Label scoreLabel = new Label();
            scoreLabel.Text = "Interview Score (0-100):";
            scoreLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            scoreLabel.Location = new System.Drawing.Point(20, currentY);
            scoreLabel.Size = new System.Drawing.Size(200, 20);
            this.Controls.Add(scoreLabel);

            scoreTextBox = new TextBox();
            scoreTextBox.Location = new System.Drawing.Point(220, currentY);
            scoreTextBox.Size = new System.Drawing.Size(100, 25);
            scoreTextBox.Text = "70";
            scoreTextBox.Font = new System.Drawing.Font("Arial", 10);
            this.Controls.Add(scoreTextBox);
            currentY += 35;

            // Interview Result Label
            Label resultLabel = new Label();
            resultLabel.Text = "Interview Result:";
            resultLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            resultLabel.Location = new System.Drawing.Point(20, currentY);
            resultLabel.Size = new System.Drawing.Size(200, 20);
            this.Controls.Add(resultLabel);

            passRadio = new RadioButton();
            passRadio.Text = "Pass";
            passRadio.Location = new System.Drawing.Point(220, currentY);
            passRadio.Size = new System.Drawing.Size(80, 25);
            passRadio.Checked = true;
            this.Controls.Add(passRadio);

            failRadio = new RadioButton();
            failRadio.Text = "Fail";
            failRadio.Location = new System.Drawing.Point(310, currentY);
            failRadio.Size = new System.Drawing.Size(80, 25);
            this.Controls.Add(failRadio);
            currentY += 35;

            // Interview Notes Label
            Label notesLabel = new Label();
            notesLabel.Text = "Interview Feedback (Required):";
            notesLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            notesLabel.Location = new System.Drawing.Point(20, currentY);
            notesLabel.Size = new System.Drawing.Size(300, 20);
            this.Controls.Add(notesLabel);
            currentY += 25;

            // Interview Notes TextBox
            interviewNotesTextBox = new TextBox();
            interviewNotesTextBox.Location = new System.Drawing.Point(20, currentY);
            interviewNotesTextBox.Size = new System.Drawing.Size(550, 120);
            interviewNotesTextBox.Multiline = true;
            interviewNotesTextBox.ScrollBars = ScrollBars.Vertical;
            interviewNotesTextBox.Font = new System.Drawing.Font("Arial", 9);
            this.Controls.Add(interviewNotesTextBox);
            currentY += 130;

            // Decision Label
            Label decisionLabel = new Label();
            decisionLabel.Text = "Decision:";
            decisionLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            decisionLabel.Location = new System.Drawing.Point(20, currentY);
            decisionLabel.Size = new System.Drawing.Size(300, 20);
            this.Controls.Add(decisionLabel);
            currentY += 30;

            // Approve Button
            approveButton = new Button();
            approveButton.Text = "✓ Approve for Final Review";
            approveButton.Location = new System.Drawing.Point(20, currentY);
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
            rejectButton.Location = new System.Drawing.Point(230, currentY);
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
            cancelButton.Location = new System.Drawing.Point(390, currentY);
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
            errorLabel.Location = new System.Drawing.Point(20, currentY + 60);
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
            // Validate score
            if (!int.TryParse(scoreTextBox?.Text.Trim(), out int score) || score < 0 || score > 100)
            {
                ShowError("Interview score must be a number between 0 and 100");
                return false;
            }

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
                // Get values
                int score = InterviewScore;
                string result = InterviewResult;
                string remarks = InterviewNotes;
                string newStatus = _interviewDecision ?? "";

                // Call EvaluateInterview to properly record to InterviewEvaluations table
                string hrRole = _db.GetRoleNameFromID(_userRoleID);
                bool success = _db.EvaluateInterview(
                    _applicationID,
                    score,
                    result,
                    remarks,
                    newStatus,
                    hrRole // Use actual role name
                );

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
