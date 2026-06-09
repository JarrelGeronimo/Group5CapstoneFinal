using HRAndApplicantSystem.Database;
using HRAndApplicantSystem.Infrastructure.Repositories;
using HRAndApplicantSystem.Models;

namespace HRAndApplicantSystem.Forms
{
    /// <summary>
    /// Applicant List Form - Display all applicants in DataGridView
    /// 
    /// ARCHITECTURE: UI Layer Only
    /// - Shows applicants in a DataGridView
    /// - Delegates data retrieval to ApplicantRepository
    /// - Allows filtering, sorting, and selection
    /// - Opens ApplicantDetailForm for editing
    /// 
    /// Under Development - Stub for Phase 3
    /// </summary>
    public partial class ApplicantListForm : Form
    {
        private readonly DatabaseHelper _db;
        private readonly IApplicantRepository _applicantRepository;

        public ApplicantListForm(DatabaseHelper db)
        {
            InitializeComponent();
            _db = db;
            _applicantRepository = new ApplicantRepository(db);
            this.Text = "Applicant List";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(1000, 600);
        }

        private void ApplicantListForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadApplicants();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading applicants: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadApplicants()
        {
            // TODO: Implement DataGridView loading from repository
            MessageBox.Show("Feature under development", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Name = "ApplicantListForm";
            this.Load += new System.EventHandler(this.ApplicantListForm_Load);
            this.ResumeLayout(false);
        }
    }
}
