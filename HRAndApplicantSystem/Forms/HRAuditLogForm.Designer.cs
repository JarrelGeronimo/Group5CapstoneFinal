namespace HRAndApplicantSystem.Forms
{
    partial class HRAuditLogForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HRAuditLogForm));
            this.panelTop = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelFilters = new System.Windows.Forms.Panel();
            this.labelUserType = new System.Windows.Forms.Label();
            this.comboBoxUserType = new System.Windows.Forms.ComboBox();
            this.labelAction = new System.Windows.Forms.Label();
            this.textBoxActionKeyword = new System.Windows.Forms.TextBox();
            this.labelDateFrom = new System.Windows.Forms.Label();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.labelDateTo = new System.Windows.Forms.Label();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.buttonFilter = new System.Windows.Forms.Button();
            this.buttonClearFilters = new System.Windows.Forms.Button();
            this.dataGridViewAuditLogs = new System.Windows.Forms.DataGridView();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.labelStatus = new System.Windows.Forms.Label();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            this.panelFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAuditLogs)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.SteelBlue;
            this.panelTop.Controls.Add(this.labelTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1000, 50);
            this.panelTop.TabIndex = 0;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(10, 10);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(200, 30);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "📋 Audit Trail Logs";
            // 
            // panelFilters
            // 
            this.panelFilters.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelFilters.Controls.Add(this.labelUserType);
            this.panelFilters.Controls.Add(this.comboBoxUserType);
            this.panelFilters.Controls.Add(this.labelAction);
            this.panelFilters.Controls.Add(this.textBoxActionKeyword);
            this.panelFilters.Controls.Add(this.labelDateFrom);
            this.panelFilters.Controls.Add(this.dateTimePickerFrom);
            this.panelFilters.Controls.Add(this.labelDateTo);
            this.panelFilters.Controls.Add(this.dateTimePickerTo);
            this.panelFilters.Controls.Add(this.buttonFilter);
            this.panelFilters.Controls.Add(this.buttonClearFilters);
            this.panelFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilters.Location = new System.Drawing.Point(0, 50);
            this.panelFilters.Name = "panelFilters";
            this.panelFilters.Padding = new System.Windows.Forms.Padding(10);
            this.panelFilters.Size = new System.Drawing.Size(1000, 100);
            this.panelFilters.TabIndex = 1;
            // 
            // labelUserType
            // 
            this.labelUserType.AutoSize = true;
            this.labelUserType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelUserType.Location = new System.Drawing.Point(10, 15);
            this.labelUserType.Name = "labelUserType";
            this.labelUserType.Size = new System.Drawing.Size(60, 15);
            this.labelUserType.TabIndex = 0;
            this.labelUserType.Text = "User Type:";
            // 
            // comboBoxUserType
            // 
            this.comboBoxUserType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUserType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboBoxUserType.FormattingEnabled = true;
            this.comboBoxUserType.Items.AddRange(new object[] {
            "All",
            "Applicant",
            "HR Staff",
            "HR Manager",
            "Administrator"});
            this.comboBoxUserType.Location = new System.Drawing.Point(70, 12);
            this.comboBoxUserType.Name = "comboBoxUserType";
            this.comboBoxUserType.Size = new System.Drawing.Size(120, 23);
            this.comboBoxUserType.TabIndex = 1;
            this.comboBoxUserType.SelectedIndex = 0;
            // 
            // labelAction
            // 
            this.labelAction.AutoSize = true;
            this.labelAction.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelAction.Location = new System.Drawing.Point(200, 15);
            this.labelAction.Name = "labelAction";
            this.labelAction.Size = new System.Drawing.Size(53, 15);
            this.labelAction.TabIndex = 2;
            this.labelAction.Text = "Action:";
            // 
            // textBoxActionKeyword
            // 
            this.textBoxActionKeyword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxActionKeyword.Location = new System.Drawing.Point(255, 12);
            this.textBoxActionKeyword.Name = "textBoxActionKeyword";
            this.textBoxActionKeyword.Size = new System.Drawing.Size(120, 23);
            this.textBoxActionKeyword.TabIndex = 3;
            this.textBoxActionKeyword.PlaceholderText = "Search keyword";
            // 
            // labelDateFrom
            // 
            this.labelDateFrom.AutoSize = true;
            this.labelDateFrom.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelDateFrom.Location = new System.Drawing.Point(385, 15);
            this.labelDateFrom.Name = "labelDateFrom";
            this.labelDateFrom.Size = new System.Drawing.Size(35, 15);
            this.labelDateFrom.TabIndex = 4;
            this.labelDateFrom.Text = "From:";
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dateTimePickerFrom.Location = new System.Drawing.Point(420, 12);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(100, 23);
            this.dateTimePickerFrom.TabIndex = 5;
            // 
            // labelDateTo
            // 
            this.labelDateTo.AutoSize = true;
            this.labelDateTo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelDateTo.Location = new System.Drawing.Point(520, 15);
            this.labelDateTo.Name = "labelDateTo";
            this.labelDateTo.Size = new System.Drawing.Size(20, 15);
            this.labelDateTo.TabIndex = 6;
            this.labelDateTo.Text = "To:";
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dateTimePickerTo.Location = new System.Drawing.Point(545, 12);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.Size = new System.Drawing.Size(100, 23);
            this.dateTimePickerTo.TabIndex = 7;
            // 
            // buttonFilter
            // 
            this.buttonFilter.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.buttonFilter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonFilter.ForeColor = System.Drawing.Color.White;
            this.buttonFilter.Location = new System.Drawing.Point(655, 12);
            this.buttonFilter.Name = "buttonFilter";
            this.buttonFilter.Size = new System.Drawing.Size(75, 23);
            this.buttonFilter.TabIndex = 8;
            this.buttonFilter.Text = "🔍 Filter";
            this.buttonFilter.UseVisualStyleBackColor = false;
            this.buttonFilter.Click += new System.EventHandler(this.ButtonFilter_Click);
            // 
            // buttonClearFilters
            // 
            this.buttonClearFilters.BackColor = System.Drawing.Color.Silver;
            this.buttonClearFilters.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.buttonClearFilters.Location = new System.Drawing.Point(735, 12);
            this.buttonClearFilters.Name = "buttonClearFilters";
            this.buttonClearFilters.Size = new System.Drawing.Size(75, 23);
            this.buttonClearFilters.TabIndex = 9;
            this.buttonClearFilters.Text = "Clear";
            this.buttonClearFilters.UseVisualStyleBackColor = false;
            this.buttonClearFilters.Click += new System.EventHandler(this.ButtonClearFilters_Click);
            // 
            // dataGridViewAuditLogs
            // 
            this.dataGridViewAuditLogs.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewAuditLogs.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridViewAuditLogs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAuditLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAuditLogs.Location = new System.Drawing.Point(0, 150);
            this.dataGridViewAuditLogs.Name = "dataGridViewAuditLogs";
            this.dataGridViewAuditLogs.ReadOnly = true;
            this.dataGridViewAuditLogs.RowHeadersWidth = 51;
            this.dataGridViewAuditLogs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewAuditLogs.Size = new System.Drawing.Size(1000, 350);
            this.dataGridViewAuditLogs.TabIndex = 2;
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelBottom.Controls.Add(this.labelStatus);
            this.panelBottom.Controls.Add(this.buttonRefresh);
            this.panelBottom.Controls.Add(this.buttonExport);
            this.panelBottom.Controls.Add(this.buttonClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 500);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1000, 50);
            this.panelBottom.TabIndex = 3;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelStatus.Location = new System.Drawing.Point(10, 15);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(120, 15);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "Total Records: 0";
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonRefresh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonRefresh.ForeColor = System.Drawing.Color.White;
            this.buttonRefresh.Location = new System.Drawing.Point(750, 10);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 30);
            this.buttonRefresh.TabIndex = 1;
            this.buttonRefresh.Text = "🔄 Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = false;
            this.buttonRefresh.Click += new System.EventHandler(this.ButtonRefresh_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(139)))), ((int)(((byte)(34)))));
            this.buttonExport.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonExport.ForeColor = System.Drawing.Color.White;
            this.buttonExport.Location = new System.Drawing.Point(830, 10);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(75, 30);
            this.buttonExport.TabIndex = 2;
            this.buttonExport.Text = "📥 Export";
            this.buttonExport.UseVisualStyleBackColor = false;
            this.buttonExport.Click += new System.EventHandler(this.ButtonExport_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.BackColor = System.Drawing.Color.Crimson;
            this.buttonClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonClose.ForeColor = System.Drawing.Color.White;
            this.buttonClose.Location = new System.Drawing.Point(910, 10);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 30);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "✕ Close";
            this.buttonClose.UseVisualStyleBackColor = false;
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // HRAuditLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1000, 550);
            this.Controls.Add(this.dataGridViewAuditLogs);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelFilters);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.Name = "HRAuditLogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Audit Trail Logs - HR Manager/Admin Only";
            this.Load += new System.EventHandler(this.HRAuditLogForm_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelFilters.ResumeLayout(false);
            this.panelFilters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAuditLogs)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Panel panelFilters;
        private System.Windows.Forms.Label labelUserType;
        private System.Windows.Forms.ComboBox comboBoxUserType;
        private System.Windows.Forms.Label labelAction;
        private System.Windows.Forms.TextBox textBoxActionKeyword;
        private System.Windows.Forms.Label labelDateFrom;
        private System.Windows.Forms.DateTimePicker dateTimePickerFrom;
        private System.Windows.Forms.Label labelDateTo;
        private System.Windows.Forms.DateTimePicker dateTimePickerTo;
        private System.Windows.Forms.Button buttonFilter;
        private System.Windows.Forms.Button buttonClearFilters;
        private System.Windows.Forms.DataGridView dataGridViewAuditLogs;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button buttonClose;
    }
}
