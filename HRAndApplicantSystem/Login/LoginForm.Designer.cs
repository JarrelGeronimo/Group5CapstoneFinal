namespace HRAndApplicantSystem.Login
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.Label statusLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.titleLabel = new System.Windows.Forms.Label();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.loginButton = new System.Windows.Forms.Button();
            this.registerButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // titleLabel
            this.titleLabel.AutoSize = false;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.titleLabel.Location = new System.Drawing.Point(50, 30);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(300, 40);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "HR and Applicant System";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // Subtitle
            System.Windows.Forms.Label subtitleLabel = new System.Windows.Forms.Label();
            subtitleLabel.AutoSize = false;
            subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            subtitleLabel.Location = new System.Drawing.Point(50, 70);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Size = new System.Drawing.Size(300, 20);
            subtitleLabel.TabIndex = 0;
            subtitleLabel.Text = "Login to your account";
            subtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Controls.Add(subtitleLabel);

            // usernameLabel
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.usernameLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.usernameLabel.Location = new System.Drawing.Point(50, 110);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(75, 19);
            this.usernameLabel.TabIndex = 1;
            this.usernameLabel.Text = "Username:";

            // usernameTextBox
            this.usernameTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.usernameTextBox.Location = new System.Drawing.Point(50, 135);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(300, 28);
            this.usernameTextBox.TabIndex = 2;
            this.usernameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // passwordLabel
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.passwordLabel.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.passwordLabel.Location = new System.Drawing.Point(50, 175);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(72, 19);
            this.passwordLabel.TabIndex = 3;
            this.passwordLabel.Text = "Password:";

            // passwordTextBox
            this.passwordTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.passwordTextBox.Location = new System.Drawing.Point(50, 200);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(300, 28);
            this.passwordTextBox.TabIndex = 4;
            this.passwordTextBox.UseSystemPasswordChar = true;
            this.passwordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // statusLabel (for error/success messages)
            this.statusLabel.AutoSize = false;
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.statusLabel.ForeColor = System.Drawing.Color.FromArgb(220, 20, 60);
            this.statusLabel.Location = new System.Drawing.Point(50, 240);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(300, 35);
            this.statusLabel.TabIndex = 7;
            this.statusLabel.Text = "";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.statusLabel.Visible = false;

            // loginButton
            this.loginButton.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.loginButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.loginButton.ForeColor = System.Drawing.Color.White;
            this.loginButton.Location = new System.Drawing.Point(90, 290);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(100, 40);
            this.loginButton.TabIndex = 5;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = false;
            this.loginButton.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.loginButton.Click += new System.EventHandler(this.LoginButton_Click);
            this.loginButton.MouseEnter += (s, e) => this.loginButton.BackColor = System.Drawing.Color.FromArgb(0, 100, 180);
            this.loginButton.MouseLeave += (s, e) => this.loginButton.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);

            // registerButton
            this.registerButton.BackColor = System.Drawing.Color.FromArgb(107, 142, 35);
            this.registerButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.registerButton.ForeColor = System.Drawing.Color.White;
            this.registerButton.Location = new System.Drawing.Point(210, 290);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(100, 40);
            this.registerButton.TabIndex = 6;
            this.registerButton.Text = "Register";
            this.registerButton.UseVisualStyleBackColor = false;
            this.registerButton.Click += new System.EventHandler(this.RegisterButton_Click);
            this.registerButton.MouseEnter += (s, e) => this.registerButton.BackColor = System.Drawing.Color.FromArgb(85, 107, 47);
            this.registerButton.MouseLeave += (s, e) => this.registerButton.BackColor = System.Drawing.Color.FromArgb(107, 142, 35);

            // LoginForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 360);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.registerButton);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.usernameTextBox);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.titleLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HR and Applicant System - Login";
            this.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
