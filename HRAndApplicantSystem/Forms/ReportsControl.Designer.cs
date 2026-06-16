namespace HRAndApplicantSystem.Forms
{
    partial class ReportsControl
    {
        private System.ComponentModel.IContainer components = null;
        
        // Navigation and Header Controls
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblHeaderTitle;
        private System.Windows.Forms.FlowLayoutPanel flpMenu;
        private System.Windows.Forms.Button btnAppMetrics;
        private System.Windows.Forms.Button btnInterviewMetrics;
        private System.Windows.Forms.Button btnTimeToHire;
        private System.Windows.Forms.Button btnHiringDecisions;
        private System.Windows.Forms.Button btnSummary;

        // Dashboard Summary Cards (Top Section)
        private System.Windows.Forms.TableLayoutPanel tlpCards;
        private System.Windows.Forms.Panel card1;
        private System.Windows.Forms.Label lblCard1Title;
        private System.Windows.Forms.Label lblCard1Value;
        private System.Windows.Forms.Panel card2;
        private System.Windows.Forms.Label lblCard2Title;
        private System.Windows.Forms.Label lblCard2Value;
        private System.Windows.Forms.Panel card3;
        private System.Windows.Forms.Label lblCard3Title;
        private System.Windows.Forms.Label lblCard3Value;

        // Detailed Content Panel (Bottom Section)
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Label lblContentTitle;
        private System.Windows.Forms.DataGridView dgvReportDetails;

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
            this.flpMenu = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAppMetrics = new System.Windows.Forms.Button();
            this.btnInterviewMetrics = new System.Windows.Forms.Button();
            this.btnTimeToHire = new System.Windows.Forms.Button();
            this.btnHiringDecisions = new System.Windows.Forms.Button();
            this.btnSummary = new System.Windows.Forms.Button();
            this.tlpCards = new System.Windows.Forms.TableLayoutPanel();
            this.card1 = new System.Windows.Forms.Panel();
            this.lblCard1Title = new System.Windows.Forms.Label();
            this.lblCard1Value = new System.Windows.Forms.Label();
            this.card2 = new System.Windows.Forms.Panel();
            this.lblCard2Title = new System.Windows.Forms.Label();
            this.lblCard2Value = new System.Windows.Forms.Label();
            this.card3 = new System.Windows.Forms.Panel();
            this.lblCard3Title = new System.Windows.Forms.Label();
            this.lblCard3Value = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblContentTitle = new System.Windows.Forms.Label();
            this.dgvReportDetails = new System.Windows.Forms.DataGridView();
            
            this.pnlHeader.SuspendLayout();
            this.flpMenu.SuspendLayout();
            this.tlpCards.SuspendLayout();
            this.card1.SuspendLayout();
            this.card2.SuspendLayout();
            this.card3.SuspendLayout();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReportDetails)).BeginInit();
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
            this.lblHeaderTitle.Text = "📊 Reports & Statistics Dashboard";

            // flpMenu (Horizontal Menu)
            this.flpMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.flpMenu.Controls.Add(this.btnSummary);
            this.flpMenu.Controls.Add(this.btnAppMetrics);
            this.flpMenu.Controls.Add(this.btnInterviewMetrics);
            this.flpMenu.Controls.Add(this.btnTimeToHire);
            this.flpMenu.Controls.Add(this.btnHiringDecisions);
            this.flpMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpMenu.Location = new System.Drawing.Point(0, 50);
            this.flpMenu.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.flpMenu.Size = new System.Drawing.Size(900, 45);

            // Menu Buttons Styling Helper
            ConfigureMenuButton(this.btnSummary, "Complete Summary");
            ConfigureMenuButton(this.btnAppMetrics, "Application Metrics");
            ConfigureMenuButton(this.btnInterviewMetrics, "Interview Metrics");
            ConfigureMenuButton(this.btnTimeToHire, "Time-to-Hire");
            ConfigureMenuButton(this.btnHiringDecisions, "Hiring Decisions");

            this.btnSummary.Click += new System.EventHandler(this.BtnSummary_Click);
            this.btnAppMetrics.Click += new System.EventHandler(this.BtnAppMetrics_Click);
            this.btnInterviewMetrics.Click += new System.EventHandler(this.BtnInterviewMetrics_Click);
            this.btnTimeToHire.Click += new System.EventHandler(this.BtnTimeToHire_Click);
            this.btnHiringDecisions.Click += new System.EventHandler(this.BtnHiringDecisions_Click);

            // tlpCards (Summary Cards Grid)
            this.tlpCards.ColumnCount = 3;
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpCards.Controls.Add(this.card1, 0, 0);
            this.tlpCards.Controls.Add(this.card2, 1, 0);
            this.tlpCards.Controls.Add(this.card3, 2, 0);
            this.tlpCards.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpCards.Location = new System.Drawing.Point(0, 95);
            this.tlpCards.Padding = new System.Windows.Forms.Padding(15);
            this.tlpCards.Size = new System.Drawing.Size(900, 120);

            // Card 1
            this.card1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.card1.Controls.Add(this.lblCard1Value);
            this.card1.Controls.Add(this.lblCard1Title);
            this.card1.Dock = System.Windows.Forms.DockStyle.Fill;
            ConfigureCardLabels(lblCard1Title, lblCard1Value);

            // Card 2
            this.card2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.card2.Controls.Add(this.lblCard2Value);
            this.card2.Controls.Add(this.lblCard2Title);
            this.card2.Dock = System.Windows.Forms.DockStyle.Fill;
            ConfigureCardLabels(lblCard2Title, lblCard2Value);

            // Card 3
            this.card3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23, ((int)(((byte)(162)))), ((int)(((byte)(184)))));
            this.card3.Controls.Add(this.lblCard3Value);
            this.card3.Controls.Add(this.lblCard3Title);
            this.card3.Dock = System.Windows.Forms.DockStyle.Fill;
            ConfigureCardLabels(lblCard3Title, lblCard3Value);

            // pnlContent (Main Container for Grid View)
            this.pnlContent.Controls.Add(this.dgvReportDetails);
            this.pnlContent.Controls.Add(this.lblContentTitle);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 215);
            this.pnlContent.Padding = new System.Windows.Forms.Padding(20);
            this.pnlContent.Size = new System.Drawing.Size(900, 385);

            // lblContentTitle
            this.lblContentTitle.AutoSize = true;
            this.lblContentTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblContentTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(80)))), ((int)(((byte)(87)))));
            this.lblContentTitle.Location = new System.Drawing.Point(20, 15);
            this.lblContentTitle.Text = "Report Breakdown View";

            // dgvReportDetails
            this.dgvReportDetails.AllowUserToAddRows = false;
            this.dgvReportDetails.AllowUserToDeleteRows = false;
            this.dgvReportDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReportDetails.BackgroundColor = System.Drawing.Color.White;
            this.dgvReportDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvReportDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReportDetails.Location = new System.Drawing.Point(20, 45);
            this.dgvReportDetails.Size = new System.Drawing.Size(860, 310);
            this.dgvReportDetails.ReadOnly = true;
            this.dgvReportDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            // ReportsControl Base
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.tlpCards);
            this.Controls.Add(this.flpMenu);
            this.Controls.Add(this.pnlHeader);
            this.Size = new System.Drawing.Size(900, 600);
            this.Load += new System.EventHandler(this.ReportsControl_Load);

            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.flpMenu.ResumeLayout(false);
            this.tlpCards.ResumeLayout(false);
            this.card1.ResumeLayout(false);
            this.card2.ResumeLayout(false);
            this.card3.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReportDetails)).EndInit();
            this.ResumeLayout(false);
        }

        private void ConfigureMenuButton(System.Windows.Forms.Button btn, string text)
        {
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            btn.Size = new System.Drawing.Size(150, 32);
            btn.Text = text;
            btn.BackColor = System.Drawing.Color.White;
            btn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(58)))), ((int)(((byte)(64)))));
            btn.FlatAppearance.BorderSize = 1;
        }

        private void ConfigureCardLabels(System.Windows.Forms.Label title, System.Windows.Forms.Label val)
        {
            title.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            title.ForeColor = System.Drawing.Color.White;
            title.Location = new System.Drawing.Point(12, 10);
            title.Size = new System.Drawing.Size(200, 20);

            val.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            val.ForeColor = System.Drawing.Color.White;
            val.Location = new System.Drawing.Point(10, 35);
            val.Size = new System.Drawing.Size(200, 35);
        }
    }
}
