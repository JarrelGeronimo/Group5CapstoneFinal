namespace HRAndApplicantSystem.Forms
{
    partial class JobVacancyDialogForm
    {
        private System.ComponentModel.IContainer components = null;

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
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtJobTitle = new System.Windows.Forms.TextBox();
            this.lblDetails = new System.Windows.Forms.Label();
            this.txtDetails = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(100, 20);
            this.lblTitle.Text = "Job Title:";
            // 
            // txtJobTitle
            // 
            this.txtJobTitle.Location = new System.Drawing.Point(20, 45);
            this.txtJobTitle.Name = "txtJobTitle";
            this.txtJobTitle.Size = new System.Drawing.Size(395, 25);
            // 
            // lblDetails
            // 
            this.lblDetails.Location = new System.Drawing.Point(20, 85);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(150, 20);
            this.lblDetails.Text = "Job Details/Description:";
            // 
            // txtDetails
            // 
            this.txtDetails.Location = new System.Drawing.Point(20, 110);
            this.txtDetails.Multiline = true;
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDetails.Size = new System.Drawing.Size(395, 150);
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(20, 275);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(100, 20);
            this.lblStatus.Text = "Status:";
            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new object[] { "Open", "Closed", "Archived" });
            this.cmbStatus.Location = new System.Drawing.Point(20, 300);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(150, 25);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(235, 345);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 30);
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(330, 345);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // JobVacancyDialogForm
            // 
            this.ClientSize = new System.Drawing.Size(440, 395);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtDetails);
            this.Controls.Add(this.lblDetails);
            this.Controls.Add(this.txtJobTitle);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Job Vacancy Editor";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtJobTitle;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.TextBox txtDetails;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
