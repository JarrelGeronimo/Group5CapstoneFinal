using System;
using System.IO;
using System.Data.OleDb;

namespace HRAndApplicantSystem.Database
{
    public class DatabaseHelper
    {
        private readonly string connectionString;

        public DatabaseHelper()
        {
            string dbPath = Path.Combine(AppContext.BaseDirectory, "Database", "HRApplicantData.accdb");
            connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";
        }

        public bool ValidateLogin(string username, string password)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM Users WHERE Username = ? AND Password = ? AND Role = ?";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", username);
                    cmd.Parameters.AddWithValue("?", password);
                    cmd.Parameters.AddWithValue("?", "HRManager");

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                        return false;

                    int count = Convert.ToInt32(result);
                    return count > 0;
                }
            }
        }
    }
}
