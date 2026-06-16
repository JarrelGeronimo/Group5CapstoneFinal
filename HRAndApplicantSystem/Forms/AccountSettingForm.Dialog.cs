using HRAndApplicantSystem.Database;

namespace HRAndApplicantSystem.Forms
{
    public partial class AccountSettingsForm : Form
    {
        private readonly DatabaseHelper db;
        private string currentUsername;

        public AccountSettingsForm(string username)
        {
            InitializeComponent();

            currentUsername = username;
            db = new DatabaseHelper();
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            string currentPass = txtCurrentPassword.Text.Trim();
            string newPass = txtNewPassword.Text.Trim();
            string confirmPass = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(currentPass) ||
                string.IsNullOrWhiteSpace(newPass) ||
                string.IsNullOrWhiteSpace(confirmPass))
            {
                MessageBox.Show(
                    "All fields are required.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show(
                    "Passwords do not match.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (currentPass == newPass)
            {
                MessageBox.Show(
                    "New password must be different.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            bool success = db.ChangeUserPassword(
                currentUsername,
                currentPass,
                newPass);

            if (success)
            {
                string role =
                    db.GetRoleNameByUsername(currentUsername);

                db.LogAuditTrail(
                    role,
                    currentUsername,
                    "Changed password");

                MessageBox.Show(
                    "Password changed successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                txtCurrentPassword.Clear();
                txtNewPassword.Clear();
                txtConfirmPassword.Clear();
            }
            else
            {
                MessageBox.Show(
                    "Current password is incorrect.",
                    "Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnChangeUsername_Click(object sender, EventArgs e)
        {
            string password =
                txtUsernamePassword.Text.Trim();

            string newUsername =
                txtNewUsername.Text.Trim();

            if (!db.ValidateLogin(currentUsername, password))
            {
                MessageBox.Show(
                    "Invalid password.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (newUsername == currentUsername)
            {
                MessageBox.Show(
                    "New username must be different.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            string role =
                db.GetRoleNameByUsername(currentUsername);

            bool success =
                db.ChangeUsername(currentUsername, newUsername);

            if (success)
            {
                db.LogAuditTrail(
                    role,
                    newUsername,
                    "Changed username");

                currentUsername = newUsername;

                MessageBox.Show(
                    $"Username changed successfully!\n\nNew Username: {newUsername}",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                txtUsernamePassword.Clear();
                txtNewUsername.Clear();
            }
            else
            {
                MessageBox.Show(
                    "Username already exists.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
