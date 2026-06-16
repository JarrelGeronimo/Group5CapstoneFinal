using System;
using System.Windows.Forms;
using HRAndApplicantSystem.Services;

namespace HRAndApplicantSystem.Forms
{
    public partial class RequirementControl : UserControl
    {
        private readonly RequirementManagementService _requirementService;

        public RequirementControl()
        {
            InitializeComponent();
            _requirementService = new RequirementManagementService();
        }

        private void RequirementControl_Load(object sender, EventArgs e)
        {
            LoadRequirementsGrid();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadRequirementsGrid();
        }

        private void LoadRequirementsGrid()
        {
            try
            {
                var requirements = _requirementService.GetAllRequirements();
                dgvRequirements.DataSource = null;
                dgvRequirements.DataSource = requirements;

                
                if (dgvRequirements.Columns["RequirementTypeID"] != null)
                    dgvRequirements.Columns["RequirementTypeID"].HeaderText = "Type ID";

                if (dgvRequirements.Columns["RequirementName"] != null)
                    dgvRequirements.Columns["RequirementName"].HeaderText = "Requirement Document Title";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching requirements: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string reqName = txtRequirementName.Text.Trim();

            if (string.IsNullOrWhiteSpace(reqName))
            {
                MessageBox.Show("Please enter a valid name for the requirement type.", "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = _requirementService.AddRequirement(reqName);

            if (success)
            {
                MessageBox.Show($"Successfully configured '{reqName}' as an active requirement standard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtRequirementName.Clear();
                LoadRequirementsGrid();
            }
            else
            {
                MessageBox.Show("Failed to record new configuration type. Please check connection and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (dgvRequirements.CurrentRow == null)
            {
                MessageBox.Show("Please select a document requirement configuration to remove from the tracking system.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            dynamic selectedRow = dgvRequirements.CurrentRow.DataBoundItem;
            int reqId = selectedRow.RequirementTypeID;
            string reqName = selectedRow.RequirementName;

            var confirm = MessageBox.Show($"Are you sure you want to completely drop '{reqName}' as a requirement type?\nThis might affect workflow validations.", 
                "Confirm Requirement Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                bool success = _requirementService.RemoveRequirement(reqId);

                if (success)
                {
                    MessageBox.Show("The requirement standard configuration has been successfully removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRequirementsGrid();
                }
                else
                {
                    MessageBox.Show("An error occurred. The item cannot be removed. It might be linked to active applicant uploads.", "System Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
