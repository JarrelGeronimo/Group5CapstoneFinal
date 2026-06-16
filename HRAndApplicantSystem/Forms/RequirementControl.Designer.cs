namespace HRAndApplicantSystem.Forms
{
    partial class RequirementControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblHeaderTitle;
        private System.Windows.Forms.GroupBox gbAddRequirement;
        private System.Windows.Forms.TextBox txtRequirementName;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label lblInputLabel;
        private System.Windows.Forms.DataGridView dgvRequirements;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnRefresh;

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
            this.gbAddRequirement = new System.Windows.Forms.GroupBox();
            this.lblInputLabel = new System.Windows.Forms.Label();
            this.txtRequirementName = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dgvRequirements = new System.Windows.Forms.DataGridView();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            
            this.pnlHeader.SuspendLayout();
            this.gbAddRequirement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequirements)).BeginInit();
            this.SuspendLayout();
            
            // pnlHeader
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.pnlHeader.Controls.Add(this.lblHeaderTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Size = new System.Drawing.Size(900, 50);
            
            // lblHeaderTitle
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeaderTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblHeaderTitle.Location = new System.Drawing.Point(15, 12);
            this.lblHeaderTitle.Text = "📋 Document Requirement Type Management";

            // gbAddRequirement
            this.gbAddRequirement.Controls.Add(this.btnAdd);
            this.gbAddRequirement.Controls.Add(this.txtRequirementName);
            this.gbAddRequirement.Controls.Add(this.lblInputLabel);
            this.gbAddRequirement.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.gbAddRequirement.Location = new System.Drawing.Point(20, 70);
            this.gbAddRequirement.Size = new System.Drawing.Size(860, 90);
            this.gbAddRequirement.Text = "Create New Requirement Configuration";

            // lblInputLabel
            this.lblInputLabel.AutoSize = true;
            this.lblInputLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.lblInputLabel.Location = new System.Drawing.Point(15, 42);
            this.lblInputLabel.Text = "Requirement Name (e.g., 'NBI Clearance', 'Medical Certificate'):";

            // txtRequirementName
            this.txtRequirementName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            this.txtRequirementName.Location = new System.Drawing.Point(400, 36);
            this.txtRequirementName.Size = new System.Drawing.Size(320, 27);

            // btnAdd
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(735, 35);
            this.btnAdd.Size = new System.Drawing.Size(110, 30);
            this.btnAdd.Text = "➕ Add Type";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);

            // dgvRequirements
            this.dgvRequirements.AllowUserToAddRows = false;
            this.dgvRequirements.AllowUserToDeleteRows = false;
            this.dgvRequirements.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRequirements.BackgroundColor = System.Drawing.Color.White;
            this.dgvRequirements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRequirements.Location = new System.Drawing.Point(20, 180);
            this.dgvRequirements.MultiSelect = false;
            this.dgvRequirements.ReadOnly = true;
            this.dgvRequirements.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRequirements.Size = new System.Drawing.Size(860, 350);

            // btnRemove
            this.btnRemove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRemove.ForeColor = System.Drawing.Color.White;
            this.btnRemove.Location = new System.Drawing.Point(700, 545);
            this.btnRemove.Size = new System.Drawing.Size(180, 38);
            this.btnRemove.Text = "🗑️ Remove Selected";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.BtnRemove_Click);

            // btnRefresh
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(20, 545);
            this.btnRefresh.Size = new System.Drawing.Size(110, 38);
            this.btnRefresh.Text = "🔄 Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);

            // RequirementControl Base Setup
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.dgvRequirements);
            this.Controls.Add(this.gbAddRequirement);
            this.Controls.Add(this.pnlHeader);
            this.Size = new System.Drawing.Size(900, 600);
            this.Load += new System.EventHandler(this.RequirementControl_Load);
            
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.gbAddRequirement.ResumeLayout(false);
            this.gbAddRequirement.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequirements)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
