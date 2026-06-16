namespace HRAndApplicantSystem.Forms
{
    partial class AccountSettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;

        private GroupBox grpPassword;
        private Label lblCurrentPassword;
        private Label lblNewPassword;
        private Label lblConfirmPassword;

        private TextBox txtCurrentPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;

        private Button btnChangePassword;

        private GroupBox grpUsername;
        private Label lblUsernamePassword;
        private Label lblNewUsername;

        private TextBox txtUsernamePassword;
        private TextBox txtNewUsername;

        private Button btnChangeUsername;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();

            grpPassword = new GroupBox();
            lblCurrentPassword = new Label();
            lblNewPassword = new Label();
            lblConfirmPassword = new Label();

            txtCurrentPassword = new TextBox();
            txtNewPassword = new TextBox();
            txtConfirmPassword = new TextBox();

            btnChangePassword = new Button();

            grpUsername = new GroupBox();
            lblUsernamePassword = new Label();
            lblNewUsername = new Label();

            txtUsernamePassword = new TextBox();
            txtNewUsername = new TextBox();

            btnChangeUsername = new Button();

            SuspendLayout();

            // Form
            Text = "Account Settings";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(700, 500);
            BackColor = Color.White;

            // Title
            lblTitle.Text = "ACCOUNT SETTINGS";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.Location = new Point(210, 20);
            lblTitle.AutoSize = true;

            // PASSWORD GROUP
            grpPassword.Text = "Change Password";
            grpPassword.Location = new Point(30, 70);
            grpPassword.Size = new Size(620, 180);

            lblCurrentPassword.Text = "Current Password:";
            lblCurrentPassword.Location = new Point(20, 40);

            txtCurrentPassword.Location = new Point(180, 35);
            txtCurrentPassword.Size = new Size(350, 30);
            txtCurrentPassword.PasswordChar = '*';

            lblNewPassword.Text = "New Password:";
            lblNewPassword.Location = new Point(20, 80);

            txtNewPassword.Location = new Point(180, 75);
            txtNewPassword.Size = new Size(350, 30);
            txtNewPassword.PasswordChar = '*';

            lblConfirmPassword.Text = "Confirm Password:";
            lblConfirmPassword.Location = new Point(20, 120);

            txtConfirmPassword.Location = new Point(180, 115);
            txtConfirmPassword.Size = new Size(350, 30);
            txtConfirmPassword.PasswordChar = '*';

            btnChangePassword.Text = "Change Password";
            btnChangePassword.Location = new Point(220, 145);
            btnChangePassword.Size = new Size(180, 30);
            btnChangePassword.Click += btnChangePassword_Click;

            grpPassword.Controls.Add(lblCurrentPassword);
            grpPassword.Controls.Add(txtCurrentPassword);
            grpPassword.Controls.Add(lblNewPassword);
            grpPassword.Controls.Add(txtNewPassword);
            grpPassword.Controls.Add(lblConfirmPassword);
            grpPassword.Controls.Add(txtConfirmPassword);
            grpPassword.Controls.Add(btnChangePassword);

            // USERNAME GROUP
            grpUsername.Text = "Change Username";
            grpUsername.Location = new Point(30, 270);
            grpUsername.Size = new Size(620, 150);

            lblUsernamePassword.Text = "Password:";
            lblUsernamePassword.Location = new Point(20, 40);

            txtUsernamePassword.Location = new Point(180, 35);
            txtUsernamePassword.Size = new Size(350, 30);
            txtUsernamePassword.PasswordChar = '*';

            lblNewUsername.Text = "New Username:";
            lblNewUsername.Location = new Point(20, 80);

            txtNewUsername.Location = new Point(180, 75);
            txtNewUsername.Size = new Size(350, 30);

            btnChangeUsername.Text = "Change Username";
            btnChangeUsername.Location = new Point(220, 110);
            btnChangeUsername.Size = new Size(180, 30);
            btnChangeUsername.Click += btnChangeUsername_Click;

            grpUsername.Controls.Add(lblUsernamePassword);
            grpUsername.Controls.Add(txtUsernamePassword);
            grpUsername.Controls.Add(lblNewUsername);
            grpUsername.Controls.Add(txtNewUsername);
            grpUsername.Controls.Add(btnChangeUsername);

            Controls.Add(lblTitle);
            Controls.Add(grpPassword);
            Controls.Add(grpUsername);

            ResumeLayout(false);
        }
    }
}
