using System;
using System.Windows.Forms;
using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Forms
{
    public partial class ApplicantProfileForm : Form
    {
        private readonly DatabaseHelper _db;
        private readonly string _username;
        private TextBox firstNameTextBox;
        private TextBox lastNameTextBox;
        private TextBox contactNoTextBox;
        private TextBox addressTextBox;
        private TextBox educationTextBox;
        private TextBox skillsTextBox;
        private Button saveButton;
        private Button cancelButton;
        private Label titleLabel;

        public ApplicantProfileForm(DatabaseHelper db, string username)
        {
            InitializeComponent();
            _db = db;
            _username = username;
            this.Text = "Complete Your Profile";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(600, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApplicantProfileForm));
            
            titleLabel = new Label();
            firstNameTextBox = new TextBox();
            lastNameTextBox = new TextBox();
            contactNoTextBox = new TextBox();
            addressTextBox = new TextBox();
            educationTextBox = new TextBox();
            skillsTextBox = new TextBox();
            saveButton = new Button();
            cancelButton = new Button();

            SuspendLayout();

            // Title Label
            titleLabel.AutoSize = true;
            titleLabel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            titleLabel.Location = new System.Drawing.Point(50, 20);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new System.Drawing.Size(300, 22);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Complete Your Profile";

            // First Name
            Label firstNameLabel = new Label();
            firstNameLabel.AutoSize = true;
            firstNameLabel.Location = new System.Drawing.Point(50, 60);
            firstNameLabel.Name = "firstNameLabel";
            firstNameLabel.Size = new System.Drawing.Size(75, 15);
            firstNameLabel.TabIndex = 1;
            firstNameLabel.Text = "First Name:";

            firstNameTextBox.Location = new System.Drawing.Point(150, 60);
            firstNameTextBox.Name = "firstNameTextBox";
            firstNameTextBox.Size = new System.Drawing.Size(350, 23);
            firstNameTextBox.TabIndex = 2;

            // Last Name
            Label lastNameLabel = new Label();
            lastNameLabel.AutoSize = true;
            lastNameLabel.Location = new System.Drawing.Point(50, 100);
            lastNameLabel.Name = "lastNameLabel";
            lastNameLabel.Size = new System.Drawing.Size(75, 15);
            lastNameLabel.TabIndex = 3;
            lastNameLabel.Text = "Last Name:";

            lastNameTextBox.Location = new System.Drawing.Point(150, 100);
            lastNameTextBox.Name = "lastNameTextBox";
            lastNameTextBox.Size = new System.Drawing.Size(350, 23);
            lastNameTextBox.TabIndex = 4;

            // Contact Number
            Label contactNoLabel = new Label();
            contactNoLabel.AutoSize = true;
            contactNoLabel.Location = new System.Drawing.Point(50, 140);
            contactNoLabel.Name = "contactNoLabel";
            contactNoLabel.Size = new System.Drawing.Size(95, 15);
            contactNoLabel.TabIndex = 5;
            contactNoLabel.Text = "Contact Number:";

            contactNoTextBox.Location = new System.Drawing.Point(150, 140);
            contactNoTextBox.Name = "contactNoTextBox";
            contactNoTextBox.Size = new System.Drawing.Size(350, 23);
            contactNoTextBox.TabIndex = 6;

            // Address
            Label addressLabel = new Label();
            addressLabel.AutoSize = true;
            addressLabel.Location = new System.Drawing.Point(50, 180);
            addressLabel.Name = "addressLabel";
            addressLabel.Size = new System.Drawing.Size(61, 15);
            addressLabel.TabIndex = 7;
            addressLabel.Text = "Address:";

            addressTextBox.Location = new System.Drawing.Point(150, 180);
            addressTextBox.Multiline = true;
            addressTextBox.Name = "addressTextBox";
            addressTextBox.Size = new System.Drawing.Size(350, 60);
            addressTextBox.TabIndex = 8;

            // Education
            Label educationLabel = new Label();
            educationLabel.AutoSize = true;
            educationLabel.Location = new System.Drawing.Point(50, 250);
            educationLabel.Name = "educationLabel";
            educationLabel.Size = new System.Drawing.Size(71, 15);
            educationLabel.TabIndex = 9;
            educationLabel.Text = "Education:";

            educationTextBox.Location = new System.Drawing.Point(150, 250);
            educationTextBox.Multiline = true;
            educationTextBox.Name = "educationTextBox";
            educationTextBox.Size = new System.Drawing.Size(350, 60);
            educationTextBox.TabIndex = 10;

            // Skills
            Label skillsLabel = new Label();
            skillsLabel.AutoSize = true;
            skillsLabel.Location = new System.Drawing.Point(50, 320);
            skillsLabel.Name = "skillsLabel";
            skillsLabel.Size = new System.Drawing.Size(50, 15);
            skillsLabel.TabIndex = 11;
            skillsLabel.Text = "Skills:";

            skillsTextBox.Location = new System.Drawing.Point(150, 320);
            skillsTextBox.Multiline = true;
            skillsTextBox.Name = "skillsTextBox";
            skillsTextBox.Size = new System.Drawing.Size(350, 60);
            skillsTextBox.TabIndex = 12;

            // Save Button
            saveButton.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
            saveButton.ForeColor = System.Drawing.Color.White;
            saveButton.Location = new System.Drawing.Point(150, 400);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(150, 40);
            saveButton.TabIndex = 13;
            saveButton.Text = "Save Profile";
            saveButton.UseVisualStyleBackColor = false;
            saveButton.Click += SaveButton_Click;

            // Cancel Button
            cancelButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
            cancelButton.ForeColor = System.Drawing.Color.White;
            cancelButton.Location = new System.Drawing.Point(320, 400);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(150, 40);
            cancelButton.TabIndex = 14;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = false;
            cancelButton.Click += CancelButton_Click;

            // Form
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(600, 500);
            Controls.Add(titleLabel);
            Controls.Add(firstNameLabel);
            Controls.Add(firstNameTextBox);
            Controls.Add(lastNameLabel);
            Controls.Add(lastNameTextBox);
            Controls.Add(contactNoLabel);
            Controls.Add(contactNoTextBox);
            Controls.Add(addressLabel);
            Controls.Add(addressTextBox);
            Controls.Add(educationLabel);
            Controls.Add(educationTextBox);
            Controls.Add(skillsLabel);
            Controls.Add(skillsTextBox);
            Controls.Add(saveButton);
            Controls.Add(cancelButton);

            Name = "ApplicantProfileForm";
            Text = "Complete Your Profile";
            ResumeLayout(false);
            PerformLayout();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(firstNameTextBox.Text))
            {
                MessageBox.Show("Please enter your first name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                firstNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(lastNameTextBox.Text))
            {
                MessageBox.Show("Please enter your last name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lastNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(contactNoTextBox.Text))
            {
                MessageBox.Show("Please enter your contact number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                contactNoTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(addressTextBox.Text))
            {
                MessageBox.Show("Please enter your address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                addressTextBox.Focus();
                return;
            }

            try
            {
                // Create applicant object
                Applicant applicant = new Applicant
                {
                    Username = _username,
                    FirstName = firstNameTextBox.Text.Trim(),
                    LastName = lastNameTextBox.Text.Trim(),
                    ContactNo = contactNoTextBox.Text.Trim(),
                    Address = addressTextBox.Text.Trim(),
                    Education = educationTextBox.Text.Trim(),
                    Skills = skillsTextBox.Text.Trim()
                };

                // Save to database
                bool success = _db.SaveApplicantInfo(_username, applicant);

                if (success)
                {
                    MessageBox.Show("Your profile has been saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error saving your profile. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to cancel? You need to complete your profile to access the system.",
                "Confirm Cancel",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}
