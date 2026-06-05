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
            // Point directly to the source database
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            string dbPath = Path.Combine(projectRoot, "Database", "HRApplicantData.accdb");
            connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";
        }

        public bool ValidateLogin(string username, string password)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Accept both Applicant (1) and HRManager (3) roles - using RoleID instead of text
                    string query = "SELECT COUNT(*) FROM [Users] WHERE [Username] = ? AND [Password] = ? AND ([RoleID] = ? OR [RoleID] = ?)";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.Add("@username", OleDbType.VarChar).Value = username;
                        cmd.Parameters.Add("@password", OleDbType.VarChar).Value = password;

                        cmd.Parameters.Add("@roleId1", OleDbType.Integer).Value = 1;  // Applicant
                        cmd.Parameters.Add("@roleId1", OleDbType.Integer).Value = 2;  // HR
                        cmd.Parameters.Add("@roleId1", OleDbType.Integer).Value = 3;  // HRManager
                        cmd.Parameters.Add("@roleId2", OleDbType.Integer).Value = 4;  // Admin

                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                            return false;

                        int count = Convert.ToInt32(result);
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login validation error: {ex.Message}");
                return false;
            }
        }

        public bool RegisterApplicant(string username, string password)
        {
            // Check if username already exists
            if (UsernameExists(username))
            {
                Console.WriteLine("Username already exists.");
                return false;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Register with RoleID = 1 (Applicant)
                    string query = "INSERT INTO [Users] ([Username], [Password], [RoleID]) VALUES (?, ?, ?)";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.Add("@username", OleDbType.VarChar).Value = username;
                        cmd.Parameters.Add("@password", OleDbType.VarChar).Value = password;
                        cmd.Parameters.Add("@roleId", OleDbType.Integer).Value = 1;  // Applicant

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Account created successfully!");
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering applicant: {ex.Message}");
                return false;
            }
        }

        private bool UsernameExists(string username)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM [Users] WHERE [Username] = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.Add("@username", OleDbType.VarChar).Value = username;

                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                            return false;

                        int count = Convert.ToInt32(result);
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking username: {ex.Message}");
                return false;
            }
        }
    }
}
