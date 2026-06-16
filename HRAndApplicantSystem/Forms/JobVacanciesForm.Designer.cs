namespace HRAndApplicantSystem.Forms
{
    partial class JobVacanciesForm
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
            jobsDataGridView = new DataGridView();
            topPanel = new Panel();
            searchTextBox = new TextBox();
            statusLabel = new Label();
            applyButton = new Button();
            viewDetailsButton = new Button();
            refreshButton = new Button();
            closeButton = new Button();
            Label searchLabel = new Label();
            
            ((System.ComponentModel.ISupportInitialize)jobsDataGridView).BeginInit();
            topPanel.SuspendLayout();
            SuspendLayout();
            
            // 
            // topPanel
            // 
            topPanel.BackColor = Color.FromArgb(240, 240, 240);
            topPanel.BorderStyle = BorderStyle.FixedSingle;
            topPanel.Controls.Add(closeButton);
            topPanel.Controls.Add(refreshButton);
            topPanel.Controls.Add(viewDetailsButton);
            topPanel.Controls.Add(applyButton);
            topPanel.Controls.Add(statusLabel);
            topPanel.Controls.Add(searchTextBox);
            topPanel.Controls.Add(searchLabel);
            topPanel.Dock = DockStyle.Top;
            topPanel.Location = new Point(0, 0);
            topPanel.Name = "topPanel";
            topPanel.Padding = new Padding(10);
            topPanel.Size = new Size(1200, 100);
            topPanel.TabIndex = 1;
            
            // 
            // searchLabel
            // 
            searchLabel.AutoSize = true;
            searchLabel.Location = new Point(10, 15);
            searchLabel.Name = "searchLabel";
            searchLabel.Size = new Size(47, 15);
            searchLabel.TabIndex = 0;
            searchLabel.Text = "Search:";
            
            // 
            // searchTextBox
            // 
            searchTextBox.Location = new Point(63, 12);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new Size(300, 23);
            searchTextBox.TabIndex = 1;
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(10, 45);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(70, 15);
            statusLabel.TabIndex = 2;
            statusLabel.Text = "Total Active Jobs: 0";
            
            // 
            // applyButton
            // 
            applyButton.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
            applyButton.ForeColor = System.Drawing.Color.White;
            applyButton.Location = new Point(400, 40);
            applyButton.Name = "applyButton";
            applyButton.Size = new Size(100, 35);
            applyButton.TabIndex = 3;
            applyButton.Text = "Apply";
            applyButton.UseVisualStyleBackColor = false;
            applyButton.Click += ApplyButton_Click;
            
            // 
            // viewDetailsButton
            // 
            viewDetailsButton.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
            viewDetailsButton.ForeColor = System.Drawing.Color.White;
            viewDetailsButton.Location = new Point(510, 40);
            viewDetailsButton.Name = "viewDetailsButton";
            viewDetailsButton.Size = new Size(100, 35);
            viewDetailsButton.TabIndex = 4;
            viewDetailsButton.Text = "View Details";
            viewDetailsButton.UseVisualStyleBackColor = false;
            viewDetailsButton.Click += ViewDetailsButton_Click;
            
            // 
            // refreshButton
            // 
            refreshButton.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
            refreshButton.ForeColor = System.Drawing.Color.White;
            refreshButton.Location = new Point(620, 40);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(100, 35);
            refreshButton.TabIndex = 5;
            refreshButton.Text = "Refresh";
            refreshButton.UseVisualStyleBackColor = false;
            refreshButton.Click += RefreshButton_Click;
            
            // 
            // closeButton
            // 
            closeButton.BackColor = System.Drawing.Color.FromArgb(220, 20, 60);
            closeButton.ForeColor = System.Drawing.Color.White;
            closeButton.Location = new Point(730, 40);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(100, 35);
            closeButton.TabIndex = 6;
            closeButton.Text = "Close";
            closeButton.UseVisualStyleBackColor = false;
            closeButton.Click += CloseButton_Click;
            
            // 
            // jobsDataGridView
            // 
            jobsDataGridView.AllowUserToAddRows = false;
            jobsDataGridView.BackgroundColor = Color.White;
            jobsDataGridView.ColumnHeadersHeight = 29;
            jobsDataGridView.Dock = DockStyle.Fill;
            jobsDataGridView.Location = new Point(0, 100);
            jobsDataGridView.Name = "jobsDataGridView";
            jobsDataGridView.ReadOnly = true;
            jobsDataGridView.RowHeadersWidth = 51;
            jobsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            jobsDataGridView.Size = new Size(1200, 500);
            jobsDataGridView.TabIndex = 0;
            
            // 
            // JobVacanciesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 600);
            Controls.Add(jobsDataGridView);
            Controls.Add(topPanel);
            Name = "JobVacanciesForm";
            Text = "Browse Job Vacancies";
            Load += JobVacanciesForm_Load;
            ((System.ComponentModel.ISupportInitialize)jobsDataGridView).EndInit();
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            ResumeLayout(false);
        }

        private Panel topPanel;
    }
}
