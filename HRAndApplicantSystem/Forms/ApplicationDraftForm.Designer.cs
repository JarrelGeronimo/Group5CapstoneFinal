namespace HRAndApplicantSystem.Forms
{
    partial class ApplicationDraftForm
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
            components = new System.ComponentModel.Container();
            
            SuspendLayout();
            
            // 
            // ApplicationDraftForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            AutoScrollMinSize = new System.Drawing.Size(900, 800);
            BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            ClientSize = new System.Drawing.Size(900, 700);
            Name = "ApplicationDraftForm";
            Text = "Apply for Job";
            StartPosition = FormStartPosition.CenterParent;
            
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
