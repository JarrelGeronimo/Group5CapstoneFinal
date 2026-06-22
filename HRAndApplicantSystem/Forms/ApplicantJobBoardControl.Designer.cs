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
            pnlHeader = new Panel();
            lblHeaderTitle = new Label();
            splitContainer = new SplitContainer();
            txtSearch = new TextBox();
            btnSearch = new Button();
            btnClearSearch = new Button();
            dgvJobs = new DataGridView();
            pnlDetails = new Panel();
            btnApply = new Button();
            txtJobDescription = new TextBox();
            lblDescHeader = new Label();
            lblJobStatus = new Label();
            lblJobTitle = new Label();
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvJobs).BeginInit();
            pnlDetails.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(248, 249, 250);
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(950, 50);
            pnlHeader.TabIndex = 1;
            // 
            // lblHeaderTitle
            // 
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.FromArgb(33, 37, 41);
            lblHeaderTitle.Location = new Point(15, 12);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Size = new Size(389, 32);
            lblHeaderTitle.TabIndex = 0;
            lblHeaderTitle.Text = "🔍 Explore Career Opportunities";
            // 
            // splitContainer
            // 
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Location = new Point(0, 50);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(txtSearch);
            splitContainer.Panel1.Controls.Add(btnSearch);
            splitContainer.Panel1.Controls.Add(btnClearSearch);
            splitContainer.Panel1.Controls.Add(dgvJobs);
            splitContainer.Panel1.Padding = new Padding(15);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(pnlDetails);
            splitContainer.Panel2.Padding = new Padding(15);
            splitContainer.Size = new Size(950, 550);
            splitContainer.SplitterDistance = 766;
            splitContainer.TabIndex = 0;
            // 
            // txtSearch
            // 
            txtSearch.Font = new Font("Segoe UI", 11F);
            txtSearch.Location = new Point(15, 15);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(280, 32);
            txtSearch.TabIndex = 0;
            // 
            // btnSearch
            // 
            btnSearch.BackColor = Color.FromArgb(0, 123, 255);
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSearch.ForeColor = Color.White;
            btnSearch.Location = new Point(305, 14);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(100, 29);
            btnSearch.TabIndex = 1;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = false;
            btnSearch.Click += BtnSearch_Click;
            // 
            // btnClearSearch
            // 
            btnClearSearch.BackColor = Color.FromArgb(108, 117, 125);
            btnClearSearch.FlatStyle = FlatStyle.Flat;
            btnClearSearch.Font = new Font("Segoe UI", 9.5F);
            btnClearSearch.ForeColor = Color.White;
            btnClearSearch.Location = new Point(410, 14);
            btnClearSearch.Name = "btnClearSearch";
            btnClearSearch.Size = new Size(80, 29);
            btnClearSearch.TabIndex = 2;
            btnClearSearch.Text = "Reset";
            btnClearSearch.UseVisualStyleBackColor = false;
            btnClearSearch.Click += BtnClearSearch_Click;
            // 
            // dgvJobs
            // 
            dgvJobs.AllowUserToAddRows = false;
            dgvJobs.AllowUserToDeleteRows = false;
            dgvJobs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvJobs.BackgroundColor = Color.White;
            dgvJobs.ColumnHeadersHeight = 29;
            dgvJobs.Location = new Point(15, 60);
            dgvJobs.MultiSelect = false;
            dgvJobs.Name = "dgvJobs";
            dgvJobs.ReadOnly = true;
            dgvJobs.RowHeadersWidth = 51;
            dgvJobs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvJobs.Size = new Size(520, 475);
            dgvJobs.TabIndex = 3;
            dgvJobs.SelectionChanged += DgvJobs_SelectionChanged;
            // 
            // pnlDetails
            // 
            pnlDetails.BackColor = Color.FromArgb(245, 247, 250);
            pnlDetails.Controls.Add(btnApply);
            pnlDetails.Controls.Add(txtJobDescription);
            pnlDetails.Controls.Add(lblDescHeader);
            pnlDetails.Controls.Add(lblJobStatus);
            pnlDetails.Controls.Add(lblJobTitle);
            pnlDetails.Dock = DockStyle.Fill;
            pnlDetails.Location = new Point(15, 15);
            pnlDetails.Name = "pnlDetails";
            pnlDetails.Padding = new Padding(20);
            pnlDetails.Size = new Size(150, 520);
            pnlDetails.TabIndex = 0;
            // 
            // btnApply
            // 
            btnApply.BackColor = Color.FromArgb(40, 167, 69);
            btnApply.Enabled = false;
            btnApply.FlatStyle = FlatStyle.Flat;
            btnApply.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnApply.ForeColor = Color.White;
            btnApply.Location = new Point(23, 460);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(315, 45);
            btnApply.TabIndex = 0;
            btnApply.Text = "💼 Apply for this Job";
            btnApply.UseVisualStyleBackColor = false;
            btnApply.Click += BtnApply_Click;
            // 
            // txtJobDescription
            // 
            txtJobDescription.BackColor = Color.White;
            txtJobDescription.Font = new Font("Segoe UI", 10F);
            txtJobDescription.Location = new Point(23, 140);
            txtJobDescription.Multiline = true;
            txtJobDescription.Name = "txtJobDescription";
            txtJobDescription.ReadOnly = true;
            txtJobDescription.ScrollBars = ScrollBars.Vertical;
            txtJobDescription.Size = new Size(315, 300);
            txtJobDescription.TabIndex = 1;
            // 
            // lblDescHeader
            // 
            lblDescHeader.AutoSize = true;
            lblDescHeader.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDescHeader.Location = new Point(20, 115);
            lblDescHeader.Name = "lblDescHeader";
            lblDescHeader.Size = new Size(262, 23);
            lblDescHeader.TabIndex = 2;
            lblDescHeader.Text = "Job Description & Requirements:";
            // 
            // lblJobStatus
            // 
            lblJobStatus.AutoSize = true;
            lblJobStatus.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblJobStatus.ForeColor = Color.FromArgb(108, 117, 125);
            lblJobStatus.Location = new Point(20, 75);
            lblJobStatus.Name = "lblJobStatus";
            lblJobStatus.Size = new Size(77, 21);
            lblJobStatus.TabIndex = 3;
            lblJobStatus.Text = "Status: --";
            // 
            // lblJobTitle
            // 
            lblJobTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblJobTitle.ForeColor = Color.FromArgb(33, 37, 41);
            lblJobTitle.Location = new Point(20, 20);
            lblJobTitle.Name = "lblJobTitle";
            lblJobTitle.Size = new Size(320, 50);
            lblJobTitle.TabIndex = 4;
            lblJobTitle.Text = "Select a Position";
            // 
            // ApplicantJobBoardControl
            // 
            BackColor = Color.White;
            Controls.Add(splitContainer);
            Controls.Add(pnlHeader);
            Name = "ApplicantJobBoardControl";
            Size = new Size(950, 600);
            Load += ApplicantJobBoardControl_Load;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel1.PerformLayout();
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvJobs).EndInit();
            pnlDetails.ResumeLayout(false);
            pnlDetails.PerformLayout();
            ResumeLayout(false);
        }
    }
}
