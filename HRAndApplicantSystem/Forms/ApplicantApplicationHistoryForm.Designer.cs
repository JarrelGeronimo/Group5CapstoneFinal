namespace HRAndApplicantSystem.Forms
{
    partial class ApplicantApplicationHistoryForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView lvApplications;

        private System.Windows.Forms.ColumnHeader colJobTitle;
        private System.Windows.Forms.ColumnHeader colStatus;
        private System.Windows.Forms.ColumnHeader colDateApplied;

        private System.Windows.Forms.FlowLayoutPanel flpTimeline;

        private System.Windows.Forms.Label lblTitle;

        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();

            this.splitContainer1 =
                new System.Windows.Forms.SplitContainer();

            this.lvApplications =
                new System.Windows.Forms.ListView();

            this.colJobTitle =
                new System.Windows.Forms.ColumnHeader();

            this.colStatus =
                new System.Windows.Forms.ColumnHeader();

            this.colDateApplied =
                new System.Windows.Forms.ColumnHeader();

            this.flpTimeline =
                new System.Windows.Forms.FlowLayoutPanel();

            this.btnRefresh =
                new System.Windows.Forms.Button();

            this.btnClose =
                new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)
                (this.splitContainer1)).BeginInit();

            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();

            this.SuspendLayout();

            // Title

            this.lblTitle.AutoSize = true;
            this.lblTitle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    16F,
                    System.Drawing.FontStyle.Bold);

            this.lblTitle.Location =
                new System.Drawing.Point(20, 15);

            this.lblTitle.Text =
                "MY APPLICATION HISTORY";

            // SplitContainer

            this.splitContainer1.Location =
                new System.Drawing.Point(20, 60);

            this.splitContainer1.Size =
                new System.Drawing.Size(1050, 550);

            this.splitContainer1.SplitterDistance = 350;

            // ListView

            this.lvApplications.Columns.AddRange(
                new System.Windows.Forms.ColumnHeader[]
                {
                    this.colJobTitle,
                    this.colStatus,
                    this.colDateApplied
                });

            this.lvApplications.Dock =
                System.Windows.Forms.DockStyle.Fill;

            this.lvApplications.FullRowSelect = true;
            this.lvApplications.GridLines = true;
            this.lvApplications.HideSelection = false;
            this.lvApplications.MultiSelect = false;
            this.lvApplications.View =
                System.Windows.Forms.View.Details;

            this.lvApplications.SelectedIndexChanged +=
                new System.EventHandler(
                    this.lvApplications_SelectedIndexChanged);

            this.colJobTitle.Text = "Job Title";
            this.colJobTitle.Width = 150;

            this.colStatus.Text = "Status";
            this.colStatus.Width = 100;

            this.colDateApplied.Text = "Applied";
            this.colDateApplied.Width = 80;

            // Timeline Panel

            this.flpTimeline.Dock =
                System.Windows.Forms.DockStyle.Fill;

            this.flpTimeline.AutoScroll = true;
            this.flpTimeline.FlowDirection =
                System.Windows.Forms.FlowDirection.TopDown;

            this.flpTimeline.WrapContents = false;

            // Buttons

            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Location =
                new System.Drawing.Point(820, 620);

            this.btnRefresh.Size =
                new System.Drawing.Size(100, 35);

            this.btnRefresh.Click +=
                new System.EventHandler(
                    this.btnRefresh_Click);

            this.btnClose.Text = "Close";

            this.btnClose.Location =
                new System.Drawing.Point(940, 620);

            this.btnClose.Size =
                new System.Drawing.Size(100, 35);

            this.btnClose.Click +=
                new System.EventHandler(
                    this.btnClose_Click);

            // Panels

            this.splitContainer1.Panel1.Controls.Add(
                this.lvApplications);

            this.splitContainer1.Panel2.Controls.Add(
                this.flpTimeline);

            // Form

            this.ClientSize =
                new System.Drawing.Size(1100, 680);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClose);

            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;

            this.Text =
                "Application History";

            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);

            ((System.ComponentModel.ISupportInitialize)
                (this.splitContainer1)).EndInit();

            this.splitContainer1.ResumeLayout(false);

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
