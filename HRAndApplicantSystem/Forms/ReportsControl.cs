using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HRAndApplicantSystem.Services;

namespace HRAndApplicantSystem.Forms
{
    public partial class ReportsControl : UserControl
    {
        private readonly ReportsService _reportsService;

        public ReportsControl()
        {
            InitializeComponent();
            _reportsService = new ReportsService();
        }

        private void ReportsControl_Load(object sender, EventArgs e)
        {
            // Default na bubuksan ang pangkalahatang summary kapag nag-load ang window
            LoadCompleteSummaryView();
        }

        private void BtnSummary_Click(object sender, EventArgs e) => LoadCompleteSummaryView();
        private void BtnAppMetrics_Click(object sender, EventArgs e) => LoadApplicationMetricsView();
        private void BtnInterviewMetrics_Click(object sender, EventArgs e) => LoadInterviewMetricsView();
        private void BtnTimeToHire_Click(object sender, EventArgs e) => LoadTimeToHireView();
        private void BtnHiringDecisions_Click(object sender, EventArgs e) => LoadHiringDecisionsView();

        private void LoadCompleteSummaryView()
        {
            lblContentTitle.Text = "📋 Complete Metrics Summary Overview";
            
            var appData = _reportsService.GetApplicationMetricsData();
            var interviewData = _reportsService.GetInterviewMetricsData();
            var hireData = _reportsService.GetTimeToHireMetricsData();
            var decisionData = _reportsService.GetHiringDecisionMetricsData();

            // Setup Top Cards
            SetCardData("Total Applications", appData?.TotalApplications.ToString() ?? "0",
                        "Interview Pass Rate", interviewData != null ? $"{interviewData.PassRate:F1}%" : "0%",
                        "Avg Time-to-Hire", hireData != null ? $"{hireData.AverageDaysToHire:F1} Days" : "0 Days");

            // Gumawa ng custom clean list para sa summary view row list grid
            var summaryList = new List<object>();
            if (appData != null) summaryList.Add(new { MetricGroup = "Applications", Description = "Total Volume Received", Value = appData.TotalApplications.ToString() });
            if (interviewData != null) summaryList.Add(new { MetricGroup = "Interviews", Description = "Total Evaluated (Pass Rate)", Value = $"{interviewData.TotalInterviews} ({interviewData.PassRate:F1}%)" });
            if (hireData != null) summaryList.Add(new { MetricGroup = "Recruitment Speed", Description = "Average Days to Close Hire", Value = $"{hireData.AverageDaysToHire:F1} Days" });
            if (decisionData != null) summaryList.Add(new { MetricGroup = "Hiring Output", Description = "Company Job Offer Acceptance Rate", Value = $"{decisionData.OfferRate:F1}%" });

            dgvReportDetails.DataSource = null;
            dgvReportDetails.DataSource = summaryList;
            
            if (dgvReportDetails.Columns["MetricGroup"] != null) dgvReportDetails.Columns["MetricGroup"].HeaderText = "Metric Category";
        }

        private void LoadApplicationMetricsView()
        {
            lblContentTitle.Text = "📁 Detailed Application Metrics & Funnel Status";
            var data = _reportsService.GetApplicationMetricsData();

            SetCardData("Total Applications", data?.TotalApplications.ToString() ?? "0",
                        "Active Funnel Groups", data?.StatusItems.Count.ToString() ?? "0",
                        "Status", "Operational");

            dgvReportDetails.DataSource = null;
            if (data != null)
            {
                dgvReportDetails.DataSource = data.StatusItems;
                ConfigureGridHeaders("Application Status", "Total Count", "Percentage Share");
            }
        }

        private void LoadInterviewMetricsView()
        {
            lblContentTitle.Text = "🗣️ Detailed Interview Pass / Fail Breakdown Log";
            var data = _reportsService.GetInterviewMetricsData();

            SetCardData("Total Interviews", data?.TotalInterviews.ToString() ?? "0",
                        "Passed Count", $"{data?.PassCount} ({data?.PassRate:F1}%)",
                        "Failed Count", $"{data?.FailCount} ({data?.FailRate:F1}%)");

            dgvReportDetails.DataSource = null;
            if (data != null)
            {
                dgvReportDetails.DataSource = data.DetailedResults;
                ConfigureGridHeaders("Evaluation Score / Outcome", "Total Applicants");
            }
        }

        private void LoadTimeToHireView()
        {
            lblContentTitle.Text = "⏱️ Time-To-Hire Operational Cycle Speed Metrics";
            var data = _reportsService.GetTimeToHireMetricsData();

            SetCardData("Tracked Applications", data?.TotalApplications.ToString() ?? "0",
                        "Average Speed", $"{data?.AverageDaysToHire:F1} Days",
                        "Median Duration", $"{data?.MedianDaysToHire:F1} Days");

            var statsList = new List<object>();
            if (data != null)
            {
                statsList.Add(new { Record = "Fastest Hiring Loop (Minimum)", Value = $"{data.MinDaysToHire} Days" });
                statsList.Add(new { Record = "Slowest Hiring Loop (Maximum)", Value = $"{data.MaxDaysToHire} Days" });
                statsList.Add(new { Record = "Average Operations Pace", Value = $"{data.AverageDaysToHire:F2} Days" });
            }

            dgvReportDetails.DataSource = null;
            dgvReportDetails.DataSource = statsList;
            ConfigureGridHeaders("Recruitment Process Benchmark", "Duration (Days)");
        }

        private void LoadHiringDecisionsView()
        {
            lblContentTitle.Text = "🎯 Executive Hiring Decisions Approval Rates";
            var data = _reportsService.GetHiringDecisionMetricsData();

            SetCardData("Total Executive Decisions", data?.TotalDecisions.ToString() ?? "0",
                        "Job Offers Issued", $"{data?.OfferedCount} ({data?.OfferRate:F1}%)",
                        "Rejection Decisions", $"{data?.RejectedCount} ({data?.RejectionRate:F1}%)");

            var decisionList = new List<object>();
            if (data != null)
            {
                decisionList.Add(new { DecisionType = "Approved / Offered for Employment", Total = data.OfferedCount, Ratio = $"{data.OfferRate:F1}%" });
                decisionList.Add(new { DecisionType = "Declined / Marked as Rejected", Total = data.RejectedCount, Ratio = $"{data.RejectionRate:F1}%" });
            }

            dgvReportDetails.DataSource = null;
            dgvReportDetails.DataSource = decisionList;
            ConfigureGridHeaders("Final Action Outcome", "Total Count", "Percentage Ratio");
        }

        // Helper Method para mabilisang palitan ang text ng top summary cards
        private void SetCardData(string c1T, string c1V, string c2T, string c2V, string c3T, string c3V)
        {
            lblCard1Title.Text = c1T; lblCard1Value.Text = c1V;
            lblCard2Title.Text = c2T; lblCard2Value.Text = c2V;
            lblCard3Title.Text = c3T; lblCard3Value.Text = c3V;
        }

        // Helper Method para awtomatikong lagyan ng magandang Header Title ang DataGridView
        private void ConfigureGridHeaders(string col1, string col2, string col3 = null)
        {
            if (dgvReportDetails.Columns.Count > 0) dgvReportDetails.Columns[0].HeaderText = col1;
            if (dgvReportDetails.Columns.Count > 1) dgvReportDetails.Columns[1].HeaderText = col2;
            if (col3 != null && dgvReportDetails.Columns.Count > 2) dgvReportDetails.Columns[2].HeaderText = col3;
        }
    }
}
