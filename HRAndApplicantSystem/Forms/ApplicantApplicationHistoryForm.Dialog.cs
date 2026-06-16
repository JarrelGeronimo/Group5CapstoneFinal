using System.Windows.Forms;

namespace HRAndApplicantSystem.UI
{
    public static class Dialog
    {
        public static void Success(string message)
        {
            MessageBox.Show(
                message,
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public static void Error(string message)
        {
            MessageBox.Show(
                message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static void Warning(string message)
        {
            MessageBox.Show(
                message,
                "Warning",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        public static bool Confirm(string message)
        {
            return MessageBox.Show(
                message,
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)
                == DialogResult.Yes;
        }
    }
}
