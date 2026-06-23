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
        private int _applicationID = 0; // Track the created application ID
        private List<dynamic>? _jobRequirements;
        private Dictionary<int, bool> _submittedDocuments = new Dictionary<int, bool>(); // Track submitted requirements
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
                
                // Create draft application immediately so we can track document submissions
                _applicationID = _db.CreateDraftApplication(_applicantID, _jobID);
                System.Diagnostics.Debug.WriteLine($"  Created draft application with ID: {_applicationID}");
                
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
                        int reqTypeId = req.RequirementTypeID;
                        
                        // Check if document has been submitted
                        var submittedDoc = _db.GetApplicantDocuments(_applicantID, _jobID).FirstOrDefault(d => d.RequirementTypeID == reqTypeId);
                        bool isSubmitted = submittedDoc != null;
                        _submittedDocuments[reqTypeId] = isSubmitted;
                        
                        // Requirement Label
                        Label reqLabel = new Label();
                        reqLabel.Text = $"• {req.RequirementName ?? "Document"} {(isSubmitted ? "✓" : "✗")}";
                        reqLabel.Font = new System.Drawing.Font("Arial", 9);
                        reqLabel.ForeColor = isSubmitted ? System.Drawing.Color.FromArgb(0, 100, 0) : System.Drawing.Color.FromArgb(200, 0, 0);
                        reqLabel.Location = new System.Drawing.Point(40, yOffset);
                        reqLabel.Size = new System.Drawing.Size(300, 20);
                        reqLabel.AutoSize = false;
                        this.Controls.Add(reqLabel);
                        System.Diagnostics.Debug.WriteLine($"  Added requirement: {req.RequirementName}");

                        // Submit Button
                        Button submitBtn = new Button();
                        submitBtn.Text = isSubmitted ? "Unsubmit" : "Submit";
                        submitBtn.Size = new System.Drawing.Size(80, 25);
                        submitBtn.Location = new System.Drawing.Point(350, yOffset - 2);
                        submitBtn.BackColor = isSubmitted ? System.Drawing.Color.FromArgb(169, 169, 169) : System.Drawing.Color.FromArgb(0, 102, 204);
                        submitBtn.ForeColor = System.Drawing.Color.White;
                        submitBtn.Font = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);
                        
                        // Closure to capture current values
                        int capturedReqTypeId = reqTypeId;
                        string capturedReqName = req.RequirementName ?? "Document";
                        Label capturedLabel = reqLabel;
                        Button capturedButton = submitBtn;
                        
                        submitBtn.Click += (s, e) =>
                        {
                            try
                            {
                                if (_submittedDocuments.ContainsKey(capturedReqTypeId) && _submittedDocuments[capturedReqTypeId])
                                {
                                    // Unsubmit
                                    bool success = _db.DeleteApplicantDocument(_applicantID, _jobID, capturedReqTypeId);
                                    if (success)
                                    {
                                        _submittedDocuments[capturedReqTypeId] = false;
                                        capturedLabel.Text = $"• {capturedReqName} ✗";
                                        capturedLabel.ForeColor = System.Drawing.Color.FromArgb(200, 0, 0);
                                        capturedButton.Text = "Submit";
                                        capturedButton.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
                                        MessageBox.Show("Document unsubmitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                else
                                {
                                    // Submit
                                    bool success = _db.SubmitApplicantDocument(_applicantID, _jobID, capturedReqTypeId, "Submitted by applicant", "Submitted");
                                    if (success)
                                    {
                                        _submittedDocuments[capturedReqTypeId] = true;
                                        capturedLabel.Text = $"• {capturedReqName} ✓";
                                        capturedLabel.ForeColor = System.Drawing.Color.FromArgb(0, 100, 0);
                                        capturedButton.Text = "Unsubmit";
                                        capturedButton.BackColor = System.Drawing.Color.FromArgb(169, 169, 169);
                                        MessageBox.Show("Document submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        };
                        
                        this.Controls.Add(submitBtn);

                        // Status Label
                        Label statusLbl = new Label();
                        statusLbl.Text = isSubmitted ? "Submitted" : "Not Submitted";
                        statusLbl.Font = new System.Drawing.Font("Arial", 8);
                        statusLbl.ForeColor = isSubmitted ? System.Drawing.Color.FromArgb(0, 100, 0) : System.Drawing.Color.FromArgb(200, 0, 0);
                        statusLbl.Location = new System.Drawing.Point(450, yOffset);
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
            // Submit the application by changing status from Draft to Submitted
            System.Diagnostics.Debug.WriteLine($"[SubmitButton_Click] Called for applicationID={_applicationID}, applicantID={_applicantID}, jobID={_jobID}");
            
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
                    System.Diagnostics.Debug.WriteLine($"[SubmitButton_Click] Updating application status from Draft to Submitted");
                    // Get applicant full name for history record
                    string applicantFullName = _db.GetApplicantFullName(_applicantID);
                    bool success = _db.UpdateApplicationStatus(_applicationID, "Submitted", "Submitted by applicant", applicantFullName);
                    System.Diagnostics.Debug.WriteLine($"[SubmitButton_Click] UpdateApplicationStatus returned: {success}");
                    
                    if (success)
                    {
                        System.Diagnostics.Debug.WriteLine("[SubmitButton_Click] Application submitted successfully, closing form");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[SubmitButton_Click] UpdateApplicationStatus failed");
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
                // Draft is already created on load, just close
                MessageBox.Show("Application saved as draft. You can return to edit it later.", "Draft Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
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
