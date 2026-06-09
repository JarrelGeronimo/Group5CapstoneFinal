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
            this.SuspendLayout();

            // DataGridView
            var jobsDataGridView = new System.Windows.Forms.DataGridView();
            jobsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            jobsDataGridView.BackgroundColor = System.Drawing.Color.White;
            jobsDataGridView.ReadOnly = true;
            jobsDataGridView.AllowUserToAddRows = false;
            jobsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.Controls.Add(jobsDataGridView);

            // Top Panel
            var topPanel = new System.Windows.Forms.Panel();
            topPanel.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            topPanel.Height = 90;
            topPanel.Padding = new System.Windows.Forms.Padding(10);
            topPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(topPanel);

            this.ResumeLayout(false);
        }
    }
}
