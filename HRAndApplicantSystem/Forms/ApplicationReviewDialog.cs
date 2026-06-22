using System;
using System.Windows.Forms;
using HRAndApplicantSystem.Database;
using ApplicationModel = HRAndApplicantSystem.Models.Application;

namespace HRAndApplicantSystem.Forms
{
    public partial class ApplicationReviewDialog : Form
    {
        private readonly DatabaseHelper _db;
        private readonly int _applicationID;
        private readonly string _username;
        private string _reviewDecision; // "Shortlisted" or "Rejected"

        public string ReviewDecision => _reviewDecision;
        public string ReviewNotes => reviewNotesTextBox.Text.Trim();

        public ApplicationReviewDialog(DatabaseHelper db, int applicationID, string username = "HR")
        {
            InitializeComponent();
            _db = db;
            _applicationID = applicationID;
            _username = username;
            
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            this.Text = "Application Review - Stage 1";
            this.Size = new System.Drawing.Size(600, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label titleLabel = new Label();
            titleLabel.Text = "STAGE 1: Application Review";
            titleLabel.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            titleLabel.Location = new System.Drawing.Point(20, 15);
            titleLabel.Size = new System.Drawing.Size(550, 25);
            this.Controls.Add(titleLabel);

            // Description
            Label descriptionLabel = new Label();
            descriptionLabel.Text = "Review the submitted application. Does the applicant meet the basic requirements for an interview?";
            descriptionLabel.Font = new System.Drawing.Font("Arial", 9);
            descriptionLabel.Location = new System.Drawing.Point(20, 45);
            descriptionLabel.Size = new System.Drawing.Size(550, 35);
            descriptionLabel.AutoSize = false;
            this.Controls.Add(descriptionLabel);

            // Review Notes Label
            Label notesLabel = new Label();
            notesLabel.Text = "Review Notes (Required):";
            notesLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            notesLabel.Location = new System.Drawing.Point(20, 85);
            notesLabel.Size = new System.Drawing.Size(300, 20);
            this.Controls.Add(notesLabel);

            // Review Notes TextBox
            reviewNotesTextBox.Location = new System.Drawing.Point(20, 110);
            reviewNotesTextBox.Size = new System.Drawing.Size(550, 100);
            reviewNotesTextBox.Multiline = true;
            reviewNotesTextBox.ScrollBars = ScrollBars.Vertical;
            reviewNotesTextBox.Font = new System.Drawing.Font("Arial", 9);
            this.Controls.Add(reviewNotesTextBox);

            // Decision Buttons Panel
            Label decisionLabel = new Label();
            decisionLabel.Text = "Your Decision:";
            decisionLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            decisionLabel.Location = new System.Drawing.Point(20, 220);
            decisionLabel.Size = new System.Drawing.Size(300, 20);
            this.Controls.Add(decisionLabel);

            // Approve for Interview Button
            approveButton.Text = "✓ Approve for Interview";
            approveButton.Location = new System.Drawing.Point(20, 250);
            approveButton.Size = new System.Drawing.Size(200, 50);
            approveButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            approveButton.ForeColor = System.Drawing.Color.White;
            approveButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            approveButton.FlatStyle = FlatStyle.Flat;
            approveButton.Click += ApproveButton_Click;
            this.Controls.Add(approveButton);

            // Reject Button
            rejectButton.Text = "✗ Reject Application";
            rejectButton.Location = new System.Drawing.Point(230, 250);
            rejectButton.Size = new System.Drawing.Size(200, 50);
            rejectButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
            rejectButton.ForeColor = System.Drawing.Color.White;
            rejectButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            rejectButton.FlatStyle = FlatStyle.Flat;
            rejectButton.Click += RejectButton_Click;
            this.Controls.Add(rejectButton);

            // Cancel Button
            cancelButton.Text = "Cancel";
            cancelButton.Location = new System.Drawing.Point(440, 250);
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
            errorLabel.Location = new System.Drawing.Point(20, 310);
            errorLabel.Size = new System.Drawing.Size(550, 40);
            errorLabel.Font = new System.Drawing.Font("Arial", 9);
            errorLabel.AutoSize = false;
            this.Controls.Add(errorLabel);
        }

        private void ApproveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(reviewNotesTextBox.Text))
            {
                ShowError("Review notes are required");
                return;
            }

            _reviewDecision = "Shortlisted";
            SaveReview();
        }

        private void RejectButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(reviewNotesTextBox.Text))
            {
                ShowError("Review notes are required");
                return;
            }

            _reviewDecision = "Rejected";
            SaveReview();
        }

        private void SaveReview()
        {
            try
            {
                // Convert review decision to screening result for ScreenApplication
                string result = _reviewDecision == "Shortlisted" ? "Qualified" : "Rejected";
                string remarks = ReviewNotes;
                
                // Call ScreenApplication which records to ScreeningResults table
                bool success = _db.ScreenApplication(_applicationID, result, remarks, _username);

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowError("Failed to save review. Please try again.");
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

        private TextBox reviewNotesTextBox = new TextBox();
        private Button approveButton = new Button();
        private Button rejectButton = new Button();
        private Button cancelButton = new Button();
        private Label errorLabel = new Label();
    }
}
