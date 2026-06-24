using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HRAndApplicantSystem.Forms
{
    public partial class HRAuditLogForm : Form
    {
        private readonly AuditLogService _auditService;
        private readonly int _currentUserRole;
        private readonly string _currentUsername;
        private List<AuditTrail> _allAuditLogs;
        private List<AuditTrail> _filteredAuditLogs;

        /// <summary>
        /// Constructor - requires user role validation
        /// Only Admin(4) and HR Manager(3) can access audit logs
        /// </summary>
        public HRAuditLogForm(int userRoleID, string username = "")
        {
            InitializeComponent();
            _currentUserRole = userRoleID;
            _currentUsername = username;
            _auditService = new AuditLogService();
            _allAuditLogs = new List<AuditTrail>();
            _filteredAuditLogs = new List<AuditTrail>();
        }

        private void HRAuditLogForm_Load(object sender, EventArgs e)
        {
            // Validate access - only Admin and HR Manager can view audit logs
            if (!_auditService.ValidateAuditLogAccess(_currentUserRole))
            {
                MessageBox.Show(
                    "Access Denied. Only Admin and HR Manager can view audit trail logs.",
                    "Authorization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
                this.Close();
                return;
            }

            // Initialize DateTimePickers
            dateTimePickerFrom.Value = DateTime.Now.AddMonths(-1);
            dateTimePickerTo.Value = DateTime.Now;

            // Setup DataGridView columns
            SetupDataGridViewColumns();

            // Load initial audit logs
            LoadAllAuditLogs();
        }

        /// <summary>
        /// Configures DataGridView columns for audit log display
        /// </summary>
        private void SetupDataGridViewColumns()
        {
            dataGridViewAuditLogs.Columns.Clear();

            // AuditID column
            DataGridViewTextBoxColumn colAuditID = new DataGridViewTextBoxColumn
            {
                Name = "AuditID",
                HeaderText = "ID",
                DataPropertyName = "AuditID",
                Width = 60,
                ReadOnly = true
            };
            dataGridViewAuditLogs.Columns.Add(colAuditID);

            // UserType column
            DataGridViewTextBoxColumn colUserType = new DataGridViewTextBoxColumn
            {
                Name = "UserType",
                HeaderText = "User Type",
                DataPropertyName = "UserType",
                Width = 100,
                ReadOnly = true
            };
            dataGridViewAuditLogs.Columns.Add(colUserType);

            // Username column
            DataGridViewTextBoxColumn colUsername = new DataGridViewTextBoxColumn
            {
                Name = "Username",
                HeaderText = "Username",
                DataPropertyName = "Username",
                Width = 120,
                ReadOnly = true
            };
            dataGridViewAuditLogs.Columns.Add(colUsername);

            // Action column
            DataGridViewTextBoxColumn colAction = new DataGridViewTextBoxColumn
            {
                Name = "Action",
                HeaderText = "Action",
                DataPropertyName = "Action",
                Width = 250,
                ReadOnly = true
            };
            dataGridViewAuditLogs.Columns.Add(colAction);

            // ActionDate column
            DataGridViewTextBoxColumn colActionDate = new DataGridViewTextBoxColumn
            {
                Name = "ActionDate",
                HeaderText = "Date & Time",
                DataPropertyName = "ActionDate",
                Width = 150,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm:ss" }
            };
            dataGridViewAuditLogs.Columns.Add(colActionDate);

            // UserID column (hidden)
            DataGridViewTextBoxColumn colUserID = new DataGridViewTextBoxColumn
            {
                Name = "UserID",
                HeaderText = "User ID",
                DataPropertyName = "UserID",
                Width = 0,
                ReadOnly = true,
                Visible = false
            };
            dataGridViewAuditLogs.Columns.Add(colUserID);
        }

        /// <summary>
        /// Loads all audit logs from the database
        /// </summary>
        private void LoadAllAuditLogs()
        {
            try
            {
                _allAuditLogs = _auditService.GetAllAuditLogs(500);
                System.Diagnostics.Debug.WriteLine($"HRAuditLogForm: Loaded {_allAuditLogs.Count} audit logs");
                
                _filteredAuditLogs = new List<AuditTrail>(_allAuditLogs);
                BindDataGridView(_filteredAuditLogs);
                UpdateStatusLabel();
                
                if (_allAuditLogs.Count == 0)
                {
                    MessageBox.Show(
                        "No audit logs found in the database. Check if audit logging is enabled.",
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HRAuditLogForm Error: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show(
                    $"Error loading audit logs: {ex.Message}",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Applies filters based on user input
        /// </summary>
        private void ButtonFilter_Click(object sender, EventArgs e)
        {
            try
            {
                _filteredAuditLogs = new List<AuditTrail>(_allAuditLogs);

                // Filter by User Type
                string selectedUserType = comboBoxUserType.SelectedItem?.ToString() ?? "All";
                if (selectedUserType != "All")
                {
                    _filteredAuditLogs = _filteredAuditLogs
                        .Where(log => log.UserType.Equals(selectedUserType, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Filter by Action keyword
                string actionKeyword = textBoxActionKeyword.Text.Trim();
                if (!string.IsNullOrEmpty(actionKeyword))
                {
                    _filteredAuditLogs = _filteredAuditLogs
                        .Where(log => log.Action.Contains(actionKeyword, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Filter by Date Range
                DateTime dateFrom = dateTimePickerFrom.Value.Date;
                DateTime dateTo = dateTimePickerTo.Value.Date.AddDays(1); // Include entire end date
                _filteredAuditLogs = _filteredAuditLogs
                    .Where(log => log.ActionDate >= dateFrom && log.ActionDate < dateTo)
                    .ToList();

                BindDataGridView(_filteredAuditLogs);
                UpdateStatusLabel();
                MessageBox.Show($"Filter applied. Found {_filteredAuditLogs.Count} matching records.", "Filter Complete");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error applying filters: {ex.Message}",
                    "Filter Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Clears all filters and reloads all audit logs
        /// </summary>
        private void ButtonClearFilters_Click(object sender, EventArgs e)
        {
            try
            {
                comboBoxUserType.SelectedIndex = 0; // All
                textBoxActionKeyword.Clear();
                dateTimePickerFrom.Value = DateTime.Now.AddMonths(-1);
                dateTimePickerTo.Value = DateTime.Now;

                _filteredAuditLogs = new List<AuditTrail>(_allAuditLogs);
                BindDataGridView(_filteredAuditLogs);
                UpdateStatusLabel();
                MessageBox.Show("All filters cleared.", "Filters Cleared");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error clearing filters: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Refreshes data from the database
        /// </summary>
        private void ButtonRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadAllAuditLogs();
                MessageBox.Show("Audit logs refreshed from database.", "Refresh Complete");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error refreshing data: {ex.Message}",
                    "Refresh Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Exports current filtered audit logs to CSV file
        /// </summary>
        private void ButtonExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (_filteredAuditLogs.Count == 0)
                {
                    MessageBox.Show("No audit logs to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv",
                    FileName = $"AuditTrail_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCSV(saveDialog.FileName, _filteredAuditLogs);
                    MessageBox.Show($"Audit logs exported successfully to:\n{saveDialog.FileName}", "Export Complete");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error exporting audit logs: {ex.Message}",
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Exports audit logs to CSV file
        /// </summary>
        private void ExportToCSV(string filePath, List<AuditTrail> logs)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Write header
                writer.WriteLine("\"AuditID\",\"UserType\",\"UserID\",\"Username\",\"Action\",\"ActionDate\"");

                // Write data rows
                foreach (var log in logs)
                {
                    string escapedAction = log.Action.Replace("\"", "\"\"");
                    writer.WriteLine($"\"{log.AuditID}\",\"{log.UserType}\",\"{log.UserID}\",\"{log.Username}\",\"{escapedAction}\",\"{log.ActionDate:yyyy-MM-dd HH:mm:ss}\"");
                }
            }
        }

        /// <summary>
        /// Closes the form
        /// </summary>
        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Binds audit log data to DataGridView
        /// </summary>
        private void BindDataGridView(List<AuditTrail> logs)
        {
            dataGridViewAuditLogs.DataSource = logs.OrderByDescending(l => l.ActionDate).ToList();
        }

        /// <summary>
        /// Updates the status label with record count
        /// </summary>
        private void UpdateStatusLabel()
        {
            labelStatus.Text = $"Total Records: {_filteredAuditLogs.Count} | All Records: {_allAuditLogs.Count}";
        }
    }
}
