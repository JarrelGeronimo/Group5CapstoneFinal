namespace HRAndApplicantSystem.Forms
{
    partial class ApplicantJobBoardControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblHeaderTitle;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnClearSearch;
        private System.Windows.Forms.DataGridView dgvJobs;
        private System.Windows.Forms.Panel pnlDetails;
        private System.Windows.Forms.Label lblJobTitle;
        private System.Windows.Forms.Label lblJobStatus;
        private System.Windows.Forms.TextBox txtJobDescription;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label lblDescHeader;

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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblHeaderTitle = new System.Windows.Forms.Label();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnClearSearch = new System.Windows.Forms.Button();
            this.dgvJobs = new System.Windows.Forms.DataGridView();
            this.pnlDetails = new System.Windows.Forms.Panel();
            this.lblJobTitle = new System.Windows.Forms.Label();
            this.lblJobStatus = new System.Windows.Forms.Label();
            this.lblDescHeader = new System.Windows.Forms.Label();
            this.txtJobDescription = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvJobs)).BeginInit();
            this.pnlDetails.SuspendLayout();
            this.SuspendLayout();
            
            // pnlHeader
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.pnlHeader.Controls.Add(this.lblHeaderTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Size = new System.Drawing.Size(950, 50);
            
            // lblHeaderTitle
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeaderTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblHeaderTitle.Location = new System.Drawing.Point(15, 12);
            this.lblHeaderTitle.Text = "🔍 Explore Career Opportunities";

            // splitContainer
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 50);
            this.splitContainer.SplitterDistance = 550;
            this.splitContainer.Size = new System.Drawing.Size(950, 550);

            // Panel 1 (Kaliwa - Search & Table Grid)
            this.splitContainer.Panel1.Controls.Add(this.txtSearch);
            this.splitContainer.Panel1.Controls.Add(this.btnSearch);
            this.splitContainer.Panel1.Controls.Add(this.btnClearSearch);
            this.splitContainer.Panel1.Controls.Add(this.dgvJobs);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(15);

            // txtSearch
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtSearch.Location = new System.Drawing.Point(15, 15);
            this.txtSearch.Size = new System.Drawing.Size(280, 27);

            // btnSearch
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(305, 14);
            this.btnSearch.Size = new System.Drawing.Size(100, 29);
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.BtnSearch_Click);

            // btnClearSearch
            this.btnClearSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnClearSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearSearch.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnClearSearch.ForeColor = System.Drawing.Color.White;
            this.btnClearSearch.Location = new System.Drawing.Point(410, 14);
            this.btnClearSearch.Size = new System.Drawing.Size(80, 29);
            this.btnClearSearch.Text = "Reset";
            this.btnClearSearch.UseVisualStyleBackColor = false;
            this.btnClearSearch.Click += new System.EventHandler(this.BtnClearSearch_Click);

            // dgvJobs
            this.dgvJobs.AllowUserToAddRows = false;
            this.dgvJobs.AllowUserToDeleteRows = false;
            this.dgvJobs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvJobs.BackgroundColor = System.Drawing.Color.White;
            this.dgvJobs.Location = new System.Drawing.Point(15, 60);
            this.dgvJobs.MultiSelect = false;
            this.dgvJobs.ReadOnly = true;
            this.dgvJobs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvJobs.Size = new System.Drawing.Size(520, 475);
            this.dgvJobs.SelectionChanged += new System.EventHandler(this.DgvJobs_SelectionChanged);

            // Panel 2 (Kanan - Job Details Side-panel View)
            this.splitContainer.Panel2.Controls.Add(this.pnlDetails);
            this.splitContainer.Panel2.Padding = new System.Windows.Forms.Padding(15);

            // pnlDetails
            this.pnlDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.pnlDetails.Controls.Add(this.btnApply);
            this.pnlDetails.Controls.Add(this.txtJobDescription);
            this.pnlDetails.Controls.Add(this.lblDescHeader);
            this.pnlDetails.Controls.Add(this.lblJobStatus);
            this.pnlDetails.Controls.Add(this.lblJobTitle);
            this.pnlDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetails.Padding = new System.Windows.Forms.Padding(20);

            // lblJobTitle
            this.lblJobTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblJobTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblJobTitle.Location = new System.Drawing.Point(20, 20);
            this.lblJobTitle.Size = new System.Drawing.Size(320, 50);
            this.lblJobTitle.Text = "Select a Position";

            // lblJobStatus
            this.lblJobStatus.AutoSize = true;
            this.lblJobStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblJobStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblJobStatus.Location = new System.Drawing.Point(20, 75);
            this.lblJobStatus.Text = "Status: --";

            // lblDescHeader
            this.lblDescHeader.AutoSize = true;
            this.lblDescHeader.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblDescHeader.Location = new System.Drawing.Point(20, 115);
            this.lblDescHeader.Text = "Job Description & Requirements:";

            // txtJobDescription
            this.txtJobDescription.BackColor = System.Drawing.Color.White;
            this.txtJobDescription.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtJobDescription.Location = new System.Drawing.Point(23, 140);
            this.txtJobDescription.Multiline = true;
            this.txtJobDescription.ReadOnly = true;
            this.txtJobDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtJobDescription.Size = new System.Drawing.Size(315, 300);

            // btnApply
            this.btnApply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnApply.Enabled = false;
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnApply.ForeColor = System.Drawing.Color.White;
            this.btnApply.Location = new System.Drawing.Point(23, 460);
            this.btnApply.Size = new System.Drawing.Size(315, 45);
            this.btnApply.Text = "💼 Apply for this Job";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);

            // ApplicantJobBoardControl Base Configuration
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.pnlHeader);
            this.Size = new System.Drawing.Size(950, 600);
            this.Load += new System.EventHandler(this.ApplicantJobBoardControl_Load);
            
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvJobs)).EndInit();
            this.pnlDetails.ResumeLayout(false);
            this.pnlDetails.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
