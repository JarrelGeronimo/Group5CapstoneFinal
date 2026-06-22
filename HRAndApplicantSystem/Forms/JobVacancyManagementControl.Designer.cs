namespace HRAndApplicantSystem.Forms
{
    partial class JobVacancyManagementControl
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
            this.lblHeader = new System.Windows.Forms.Label();
            this.dgvVacancies = new System.Windows.Forms.DataGridView();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.pnlBreakdown = new System.Windows.Forms.Panel();
            this.lblBreakdownHeader = new System.Windows.Forms.Label();
            this.lblBreakdownData = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVacancies)).BeginInit();
            this.pnlBreakdown.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblHeader.Location = new System.Drawing.Point(20, 15);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(400, 35);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Job Vacancy Management";
            // 
            // dgvVacancies
            // 
            this.dgvVacancies.AllowUserToAddRows = false;
            this.dgvVacancies.AllowUserToDeleteRows = false;
            this.dgvVacancies.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvVacancies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVacancies.Location = new System.Drawing.Point(25, 65);
            this.dgvVacancies.MultiSelect = false;
            this.dgvVacancies.Name = "dgvVacancies";
            this.dgvVacancies.ReadOnly = true;
            this.dgvVacancies.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVacancies.Size = new System.Drawing.Size(580, 360);
            this.dgvVacancies.TabIndex = 1;
            this.dgvVacancies.SelectionChanged += new System.EventHandler(this.dgvVacancies_SelectionChanged);
            // 
            // btnCreate
            // 
            this.btnCreate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCreate.ForeColor = System.Drawing.Color.White;
            this.btnCreate.Location = new System.Drawing.Point(25, 440);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(130, 35);
            this.btnCreate.TabIndex = 2;
            this.btnCreate.Text = "➕ New Vacancy";
            this.btnCreate.UseVisualStyleBackColor = false;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnEdit.ForeColor = System.Drawing.Color.White;
            this.btnEdit.Location = new System.Drawing.Point(165, 440);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(120, 35);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "✏️ Edit Details";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(295, 440);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(120, 35);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "🗑️ Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(530, 20);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 30);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "🔄 Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // pnlBreakdown
            // 
            this.pnlBreakdown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlBreakdown.Controls.Add(this.lblBreakdownData);
            this.pnlBreakdown.Controls.Add(this.lblBreakdownHeader);
            this.pnlBreakdown.Location = new System.Drawing.Point(620, 65);
            this.pnlBreakdown.Name = "pnlBreakdown";
            this.pnlBreakdown.Size = new System.Drawing.Size(240, 360);
            this.pnlBreakdown.TabIndex = 6;
            // 
            // lblBreakdownHeader
            // 
            this.lblBreakdownHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.lblBreakdownHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblBreakdownHeader.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblBreakdownHeader.Location = new System.Drawing.Point(0, 0);
            this.lblBreakdownHeader.Name = "lblBreakdownHeader";
            this.lblBreakdownHeader.Size = new System.Drawing.Size(238, 30);
            this.lblBreakdownHeader.TabIndex = 0;
            this.lblBreakdownHeader.Text = "Pipeline Breakdown";
            this.lblBreakdownHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBreakdownData
            // 
            this.lblBreakdownData.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.lblBreakdownData.Location = new System.Drawing.Point(10, 45);
            this.lblBreakdownData.Name = "lblBreakdownData";
            this.lblBreakdownData.Size = new System.Drawing.Size(220, 300);
            this.lblBreakdownData.TabIndex = 1;
            this.lblBreakdownData.Text = "Select a job...";
            // 
            // JobVacancyControl
            // 
            this.Controls.Add(this.pnlBreakdown);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.dgvVacancies);
            this.Controls.Add(this.lblHeader);
            this.Name = "JobVacancyControl";
            this.Size = new System.Drawing.Size(880, 500);
            this.Load += new System.EventHandler(this.JobVacancyControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVacancies)).EndInit();
            this.pnlBreakdown.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.DataGridView dgvVacancies;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Panel pnlBreakdown;
        private System.Windows.Forms.Label lblBreakdownHeader;
        private System.Windows.Forms.Label lblBreakdownData;
    }
}
