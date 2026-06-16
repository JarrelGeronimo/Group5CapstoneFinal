using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HRAndApplicantSystem.Database;

namespace HRAndApplicantSystem.Forms
{
    public partial class ApplicationDraftForm : Form
    {
        private readonly DatabaseHelper _db;
        private readonly int _applicantID;
        private readonly int _jobID;
        private readonly string _jobTitle;
        private List<dynamic>? _jobRequirements;
        private TextBox? coverLetterTextBox;
        private Label? statusLabel;
        private Button? submitButton;
        private Button? saveDraftButton;
        private Button? cancelButton;

        public ApplicationDraftForm(DatabaseHelper db, int applicantID, int jobID, string jobTitle)
        {
            InitializeComponent();
            _db = db;
            _applicantID = applicantID;
            _jobID = jobID;
            _jobTitle = jobTitle;
            this.Text = $"Apply for: {jobTitle}";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(900, 700);
            
            System.Diagnostics.Debug.WriteLine("ApplicationDraftForm Constructor: Initialized");
        }

        protected override void OnLoad(EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ApplicationDraftForm OnLoad: Called");
            base.OnLoad(e);
            
            try
            {
                System.Diagnostics.Debug.WriteLine("ApplicationDraftForm OnLoad: Starting");
                System.Diagnostics.Debug.WriteLine($"  ApplicantID: {_applicantID}, JobID: {_jobID}, JobTitle: {_jobTitle}");
                
                LoadJobRequirements();
                System.Diagnostics.Debug.WriteLine($"  Loaded {_jobRequirements?.Count ?? 0} requirements");
                
                InitializeLayout();
                System.Diagnostics.Debug.WriteLine($"  Layout initialized, form has {this.Controls.Count} controls");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ApplicationDraftForm OnLoad Exception: {ex}");
                MessageBox.Show($"Error loading form: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplicationDraftForm_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ApplicationDraftForm_Load: Called (should use OnLoad instead)");
        }

        private void LoadJobRequirements()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("LoadJobRequirements: Starting");
                System.Diagnostics.Debug.WriteLine($"  JobID: {_jobID}");
                
                if (_db == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadJobRequirements: ERROR - _db is null!");
                    _jobRequirements = new List<dynamic>();
                    return;
                }
                
                // Get job requirements
                _jobRequirements = _db.GetJobRequirements(_jobID);
                System.Diagnostics.Debug.WriteLine($"LoadJobRequirements: Retrieved {_jobRequirements?.Count ?? 0} requirements");
                
                if (_jobRequirements == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadJobRequirements: _jobRequirements is null, creating empty list");
                    _jobRequirements = new List<dynamic>();
                }
                
                if (_jobRequirements.Count > 0)
                {
                    for (int i = 0; i < _jobRequirements.Count; i++)
                    {
                        var req = _jobRequirements[i];
                        try
                        {
                            var name = req.RequirementName;
                            System.Diagnostics.Debug.WriteLine($"  [{i}] RequirementName: {name}");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"  [{i}] ERROR accessing RequirementName: {ex.Message}");
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("LoadJobRequirements: No requirements found");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadJobRequirements Exception: {ex}");
                MessageBox.Show($"Error loading requirements: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _jobRequirements = new List<dynamic>();
            }
        }

        private void InitializeLayout()
        {
            System.Diagnostics.Debug.WriteLine("InitializeLayout: Starting");
            
            try
            {
                // Ensure form properties are set
                this.AutoScroll = true;
                this.AutoScrollMinSize = new System.Drawing.Size(900, 0);
                
                System.Diagnostics.Debug.WriteLine($"InitializeLayout: Form AutoScroll={this.AutoScroll}, Size={this.Size}");
                
                // Title
                Label titleLabel = new Label();
                titleLabel.Text = $"Application Form: {_jobTitle}";
                titleLabel.Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
                titleLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
                titleLabel.Location = new System.Drawing.Point(20, 20);
                titleLabel.Size = new System.Drawing.Size(850, 30);
                titleLabel.AutoSize = false;
                this.Controls.Add(titleLabel);
                System.Diagnostics.Debug.WriteLine($"InitializeLayout: Added title label");

                // Instructions
                Label instructionsLabel = new Label();
                instructionsLabel.Text = "Complete all required fields below. Upload all required documents.";
                instructionsLabel.Font = new System.Drawing.Font("Arial", 9);
                instructionsLabel.Location = new System.Drawing.Point(20, 55);
                instructionsLabel.Size = new System.Drawing.Size(850, 20);
                instructionsLabel.AutoSize = false;
                this.Controls.Add(instructionsLabel);
                System.Diagnostics.Debug.WriteLine($"InitializeLayout: Added instructions label");

                // Requirements Section
                Label requirementsLabel = new Label();
                requirementsLabel.Text = "Required Documents:";
                requirementsLabel.Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold);
                requirementsLabel.Location = new System.Drawing.Point(20, 85);
                requirementsLabel.Size = new System.Drawing.Size(200, 20);
                requirementsLabel.AutoSize = false;
                this.Controls.Add(requirementsLabel);
                System.Diagnostics.Debug.WriteLine($"InitializeLayout: Added requirements label");

                int yOffset = 110;
                if (_jobRequirements != null && _jobRequirements.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"InitializeLayout: Adding {_jobRequirements.Count} requirements");
                    
                    foreach (var req in _jobRequirements)
                    {
                        // Requirement Label
                        Label reqLabel = new Label();
                        reqLabel.Text = $"• {req.RequirementName ?? "Document"}";
                        reqLabel.Font = new System.Drawing.Font("Arial", 9);
                        reqLabel.Location = new System.Drawing.Point(40, yOffset);
                        reqLabel.Size = new System.Drawing.Size(200, 20);
                        reqLabel.AutoSize = false;
                        this.Controls.Add(reqLabel);
                        System.Diagnostics.Debug.WriteLine($"  Added requirement: {req.RequirementName}");

                        // Upload Button
                        Button uploadBtn = new Button();
                        uploadBtn.Text = "Upload";
                        uploadBtn.Size = new System.Drawing.Size(80, 25);
                        uploadBtn.Location = new System.Drawing.Point(250, yOffset - 2);
                        uploadBtn.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
                        uploadBtn.ForeColor = System.Drawing.Color.White;
                        uploadBtn.Font = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);
                        uploadBtn.Click += (s, e) => MessageBox.Show($"Document upload for {req.RequirementName} - Coming in Phase 4.2", "Feature", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Controls.Add(uploadBtn);

                        // Status Label
                        Label statusLbl = new Label();
                        statusLbl.Text = "Not Uploaded";
                        statusLbl.Font = new System.Drawing.Font("Arial", 8);
                        statusLbl.ForeColor = System.Drawing.Color.FromArgb(220, 20, 60);
                        statusLbl.Location = new System.Drawing.Point(350, yOffset);
                        statusLbl.Size = new System.Drawing.Size(200, 20);
                        statusLbl.AutoSize = false;
                        this.Controls.Add(statusLbl);

                        yOffset += 35;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"InitializeLayout: No requirements to add");
                    Label noReqLabel = new Label();
                    noReqLabel.Text = "No specific documents required for this position.";
                    noReqLabel.Font = new System.Drawing.Font("Arial", 9);
                    noReqLabel.Location = new System.Drawing.Point(40, yOffset);
                    noReqLabel.Size = new System.Drawing.Size(400, 20);
                    noReqLabel.AutoSize = false;
                    this.Controls.Add(noReqLabel);
                    yOffset += 35;
                }

                // Cover Letter Section
                yOffset += 20;
                Label coverLetterLabel = new Label();
                coverLetterLabel.Text = "Cover Letter (Optional):";
                coverLetterLabel.Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold);
                coverLetterLabel.Location = new System.Drawing.Point(20, yOffset);
                coverLetterLabel.Size = new System.Drawing.Size(200, 20);
                coverLetterLabel.AutoSize = false;
                this.Controls.Add(coverLetterLabel);
                System.Diagnostics.Debug.WriteLine($"InitializeLayout: Added cover letter label");

                // Cover Letter TextBox
                yOffset += 30;
                coverLetterTextBox = new TextBox();
                coverLetterTextBox.Location = new System.Drawing.Point(20, yOffset);
                coverLetterTextBox.Size = new System.Drawing.Size(850, 180);
                coverLetterTextBox.Multiline = true;
                coverLetterTextBox.ScrollBars = ScrollBars.Vertical;
                coverLetterTextBox.Font = new System.Drawing.Font("Arial", 9);
                coverLetterTextBox.Text = "Tell the employer why you're interested in this position...";
                this.Controls.Add(coverLetterTextBox);
                System.Diagnostics.Debug.WriteLine($"InitializeLayout: Added cover letter textbox");

                // Buttons
                yOffset += 200;
                submitButton = new Button();
                submitButton.Text = "✓ Submit Application";
                submitButton.Size = new System.Drawing.Size(150, 40);
                submitButton.Location = new System.Drawing.Point(280, yOffset);
                submitButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
                submitButton.ForeColor = System.Drawing.Color.White;
                submitButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
                submitButton.Click += SubmitButton_Click;
                this.Controls.Add(submitButton);

                saveDraftButton = new Button();
                saveDraftButton.Text = "💾 Save as Draft";
                saveDraftButton.Size = new System.Drawing.Size(150, 40);
                saveDraftButton.Location = new System.Drawing.Point(450, yOffset);
                saveDraftButton.BackColor = System.Drawing.Color.FromArgb(107, 142, 35);
                saveDraftButton.ForeColor = System.Drawing.Color.White;
                saveDraftButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
                saveDraftButton.Click += SaveDraftButton_Click;
                this.Controls.Add(saveDraftButton);

                cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.Size = new System.Drawing.Size(100, 40);
                cancelButton.Location = new System.Drawing.Point(620, yOffset);
                cancelButton.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
                cancelButton.ForeColor = System.Drawing.Color.White;
                cancelButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
                cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
                this.Controls.Add(cancelButton);
                System.Diagnostics.Debug.WriteLine($"InitializeLayout: Added buttons");

                // Status Label
                statusLabel = new Label();
                statusLabel.Text = "";
                statusLabel.ForeColor = System.Drawing.Color.FromArgb(220, 20, 60);
                statusLabel.Location = new System.Drawing.Point(20, yOffset + 55);
                statusLabel.Size = new System.Drawing.Size(850, 30);
                statusLabel.Font = new System.Drawing.Font("Arial", 9);
                statusLabel.AutoSize = false;
                this.Controls.Add(statusLabel);
                
                System.Diagnostics.Debug.WriteLine($"InitializeLayout: Completed - Total controls: {this.Controls.Count}, YOffset: {yOffset}");
                
                // Ensure form is properly laid out and visible
                this.AutoScrollMinSize = new System.Drawing.Size(900, yOffset + 100);
                this.PerformLayout();
                this.Invalidate();
                this.Update();
                
                System.Diagnostics.Debug.WriteLine($"InitializeLayout: Form refresh complete");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InitializeLayout Exception: {ex}");
                throw;
            }
        }

        private void SubmitButton_Click(object? sender, EventArgs? e)
        {
            // In Phase 4.2, validate that all required documents are uploaded
            // For now, just submit
            System.Diagnostics.Debug.WriteLine($"[SubmitButton_Click] Called for applicantID={_applicantID}, jobID={_jobID}");
            
            DialogResult result = MessageBox.Show(
                "Are you sure you want to submit this application?\nYou will not be able to edit it after submission.",
                "Confirm Submission",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            System.Diagnostics.Debug.WriteLine($"[SubmitButton_Click] User confirmation: {result}");

            if (result == DialogResult.Yes)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"[SubmitButton_Click] Calling _db.SubmitJobApplication");
                    bool success = _db.SubmitJobApplication(_applicantID, _jobID);
                    System.Diagnostics.Debug.WriteLine($"[SubmitButton_Click] SubmitJobApplication returned: {success}");
                    
                    if (success)
                    {
                        System.Diagnostics.Debug.WriteLine("[SubmitButton_Click] Application submitted successfully, closing form");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[SubmitButton_Click] SubmitJobApplication failed");
                        ShowError("Failed to submit application. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"Error submitting application: {ex.Message}");
                }
            }
        }

        private void SaveDraftButton_Click(object? sender, EventArgs? e)
        {
            try
            {
                // Check if draft already exists for this application
                int applicationID = _db.CreateDraftApplication(_applicantID, _jobID);
                if (applicationID > 0)
                {
                    MessageBox.Show("Application saved as draft. You can return to edit it later.", "Draft Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowError("Failed to save draft. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error saving draft: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            if (statusLabel != null)
            {
                statusLabel.Text = message;
                statusLabel.ForeColor = System.Drawing.Color.FromArgb(220, 20, 60);
            }
        }
    }
}
