using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Login;
using System;
using System.Windows.Forms;

namespace HRAndApplicantSystem.Forms
{
    /// <summary>
    /// Dialog for applicants to change their username or password
    /// </summary>
    public partial class ChangeCredentialsDialog : Form
    {
        private readonly string _currentUsername;
        private readonly LoginService _loginService;
        private readonly DatabaseHelper _db;

        public ChangeCredentialsDialog(string currentUsername)
        {
            InitializeComponent();
            _currentUsername = currentUsername;
            _loginService = new LoginService();
            _db = new DatabaseHelper();
            
            this.Text = "Change Username or Password";
            this.Size = new System.Drawing.Size(450, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeComponent()
        {
            // Enable auto scroll on form
            this.AutoScroll = true;

            // Main scrollable panel
            Panel scrollPanel = new Panel();
            scrollPanel.Dock = DockStyle.Fill;
            scrollPanel.AutoScroll = true;
            scrollPanel.Padding = new Padding(20);
            scrollPanel.BackColor = System.Drawing.Color.White;

            int yPos = 10;

            // Title
            Label titleLabel = new Label();
            titleLabel.Text = "Change Your Credentials";
            titleLabel.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            titleLabel.Location = new System.Drawing.Point(20, yPos);
            titleLabel.Width = 350;
            titleLabel.Height = 30;
            scrollPanel.Controls.Add(titleLabel);
            yPos += 40;

            // Current Password section
            Label currentPasswordLabel = new Label();
            currentPasswordLabel.Text = "Current Password:";
            currentPasswordLabel.Location = new System.Drawing.Point(20, yPos);
            currentPasswordLabel.Width = 350;
            currentPasswordLabel.Height = 20;
            scrollPanel.Controls.Add(currentPasswordLabel);
            yPos += 25;

            TextBox currentPasswordBox = new TextBox();
            currentPasswordBox.PasswordChar = '*';
            currentPasswordBox.Location = new System.Drawing.Point(20, yPos);
            currentPasswordBox.Width = 350;
            currentPasswordBox.Height = 30;
            scrollPanel.Controls.Add(currentPasswordBox);
            yPos += 40;

            // Section: Change Username
            Label changeUsernameLabel = new Label();
            changeUsernameLabel.Text = "Change Username (Optional):";
            changeUsernameLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            changeUsernameLabel.Location = new System.Drawing.Point(20, yPos);
            changeUsernameLabel.Width = 350;
            changeUsernameLabel.Height = 20;
            scrollPanel.Controls.Add(changeUsernameLabel);
            yPos += 25;

            Label newUsernameLabel = new Label();
            newUsernameLabel.Text = "New Username:";
            newUsernameLabel.Location = new System.Drawing.Point(20, yPos);
            newUsernameLabel.Width = 350;
            newUsernameLabel.Height = 20;
            scrollPanel.Controls.Add(newUsernameLabel);
            yPos += 25;

            TextBox newUsernameBox = new TextBox();
            newUsernameBox.Location = new System.Drawing.Point(20, yPos);
            newUsernameBox.Width = 350;
            newUsernameBox.Height = 30;
            scrollPanel.Controls.Add(newUsernameBox);
            yPos += 40;

            // Section: Change Password
            Label changePasswordLabel = new Label();
            changePasswordLabel.Text = "Change Password (Optional):";
            changePasswordLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            changePasswordLabel.Location = new System.Drawing.Point(20, yPos);
            changePasswordLabel.Width = 350;
            changePasswordLabel.Height = 20;
            scrollPanel.Controls.Add(changePasswordLabel);
            yPos += 25;

            Label newPasswordLabel = new Label();
            newPasswordLabel.Text = "New Password:";
            newPasswordLabel.Location = new System.Drawing.Point(20, yPos);
            newPasswordLabel.Width = 350;
            newPasswordLabel.Height = 20;
            scrollPanel.Controls.Add(newPasswordLabel);
            yPos += 25;

            TextBox newPasswordBox = new TextBox();
            newPasswordBox.PasswordChar = '*';
            newPasswordBox.Location = new System.Drawing.Point(20, yPos);
            newPasswordBox.Width = 350;
            newPasswordBox.Height = 30;
            scrollPanel.Controls.Add(newPasswordBox);
            yPos += 30;

            Label confirmPasswordLabel = new Label();
            confirmPasswordLabel.Text = "Confirm New Password:";
            confirmPasswordLabel.Location = new System.Drawing.Point(20, yPos);
            confirmPasswordLabel.Width = 350;
            confirmPasswordLabel.Height = 20;
            scrollPanel.Controls.Add(confirmPasswordLabel);
            yPos += 25;

            TextBox confirmPasswordBox = new TextBox();
            confirmPasswordBox.PasswordChar = '*';
            confirmPasswordBox.Location = new System.Drawing.Point(20, yPos);
            confirmPasswordBox.Width = 350;
            confirmPasswordBox.Height = 30;
            scrollPanel.Controls.Add(confirmPasswordBox);

            this.Controls.Add(scrollPanel);

            // Button panel
            Panel buttonPanel = new Panel();
            buttonPanel.Height = 60;
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            buttonPanel.Padding = new Padding(10);

            // Update button
            Button updateButton = new Button();
            updateButton.Text = "Update";
            updateButton.Size = new System.Drawing.Size(100, 35);
            updateButton.Location = new System.Drawing.Point(200, 10);
            updateButton.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            updateButton.ForeColor = System.Drawing.Color.White;
            updateButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            updateButton.Click += (s, e) =>
            {
                try
                {
                    string currentPassword = currentPasswordBox.Text;
                    string newUsername = newUsernameBox.Text.Trim();
                    string newPassword = newPasswordBox.Text;
                    string confirmPassword = confirmPasswordBox.Text;

                    // Validation
                    if (string.IsNullOrWhiteSpace(currentPassword))
                    {
                        MessageBox.Show("Current password is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        currentPasswordBox.Focus();
                        return;
                    }

                    // Validate current password
                    if (!_loginService.Login(_currentUsername, currentPassword))
                    {
                        MessageBox.Show("Current password is incorrect.", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        currentPasswordBox.Clear();
                        currentPasswordBox.Focus();
                        return;
                    }

                    bool usernameChanged = false;
                    bool passwordChanged = false;

                    // Change username if provided
                    if (!string.IsNullOrWhiteSpace(newUsername))
                    {
                        if (newUsername.Length < 3)
                        {
                            MessageBox.Show("New username must be at least 3 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            newUsernameBox.Focus();
                            return;
                        }

                        if (_db.UsernameExists(newUsername) && newUsername != _currentUsername)
                        {
                            MessageBox.Show("Username already exists. Please choose another.", "Duplicate Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            newUsernameBox.Focus();
                            return;
                        }

                        if (_db.ChangeUsername(_currentUsername, newUsername))
                        {
                            usernameChanged = true;
                        }
                        else
                        {
                            MessageBox.Show("Failed to change username.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Change password if provided
                    if (!string.IsNullOrWhiteSpace(newPassword))
                    {
                        if (newPassword.Length < 6)
                        {
                            MessageBox.Show("New password must be at least 6 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            newPasswordBox.Focus();
                            return;
                        }

                        if (newPassword != confirmPassword)
                        {
                            MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            confirmPasswordBox.Focus();
                            return;
                        }

                        // Use the updated username if it was changed
                        string usernameToUpdate = usernameChanged ? newUsername : _currentUsername;

                        if (_db.ChangeUserPassword(usernameToUpdate, currentPassword, newPassword))
                        {
                            passwordChanged = true;
                        }
                        else
                        {
                            MessageBox.Show("Failed to change password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // If nothing was changed, show message
                    if (!usernameChanged && !passwordChanged)
                    {
                        MessageBox.Show("No changes made.", "No Changes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Success message
                    string successMessage = "Credentials updated successfully";
                    if (usernameChanged && passwordChanged)
                    {
                        successMessage += " - Your username and password have been changed. Please log in with your new credentials.";
                    }
                    else if (usernameChanged)
                    {
                        successMessage += " - Your username has been changed.";
                    }
                    else
                    {
                        successMessage += " - Your password has been changed.";
                    }

                    MessageBox.Show(successMessage, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating credentials: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            buttonPanel.Controls.Add(updateButton);

            // Cancel button
            Button cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Size = new System.Drawing.Size(100, 35);
            cancelButton.Location = new System.Drawing.Point(310, 10);
            cancelButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
            cancelButton.ForeColor = System.Drawing.Color.White;
            cancelButton.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            cancelButton.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            buttonPanel.Controls.Add(cancelButton);

            this.Controls.Add(buttonPanel);
        }
    }
}
