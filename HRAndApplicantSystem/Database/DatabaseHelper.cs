using System;
using System.Collections.Generic;
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

                    // Accept all roles: Applicant (1), HR (2), HRManager (3), Admin (4)
                    string query = "SELECT [Password] FROM [Users] WHERE [Username] = @username AND ([RoleID] IN (1, 2, 3, 4))";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                            return false;

                        string storedPassword = result.ToString();
                        
                        // Case-sensitive password comparison
                        return storedPassword.Equals(password, StringComparison.Ordinal);
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
                    string query = "INSERT INTO [Users] ([Username], [Password], [RoleID]) VALUES (@username, @password, @roleId)";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@roleId", 1);  // Applicant

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

                    string query = "SELECT COUNT(*) FROM [Users] WHERE [Username] = @username";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

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

        public bool SaveApplicantInfo(string username, HRAndApplicantSystem.Applicant.Applicant applicant)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Verify user is an applicant (RoleID = 1)
                    int roleID = GetUserRoleByUsername(username);
                    if (roleID != 1)
                    {
                        Console.WriteLine("Only applicants can save applicant information.");
                        return false;
                    }

                    // Get UserID from username
                    int userID = GetUserIdByUsername(username);
                    if (userID == -1)
                    {
                        Console.WriteLine("User not found.");
                        return false;
                    }

                    // Check if applicant already exists for this user (by checking if we have an ApplicantID)
                    if (applicant.ApplicantID > 0)
                    {
                        // Update existing applicant info
                        return UpdateApplicantInfo(username, applicant);
                    }

                    // Insert new applicant info with UserID
                    string query = "INSERT INTO [Applicants] ([UserID], [First Name], [Last Name], [ContactNo], [Address], [Education], [Skills]) VALUES (@userID, @firstName, @lastName, @contactNo, @address, @education, @skills)";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userID", userID);
                        cmd.Parameters.AddWithValue("@firstName", applicant.FirstName ?? "");
                        cmd.Parameters.AddWithValue("@lastName", applicant.LastName ?? "");
                        cmd.Parameters.AddWithValue("@contactNo", applicant.ContactNo ?? "");
                        cmd.Parameters.AddWithValue("@address", applicant.Address ?? "");
                        cmd.Parameters.AddWithValue("@education", applicant.Education ?? "");
                        cmd.Parameters.AddWithValue("@skills", applicant.Skills ?? "");

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving applicant info: {ex.Message}");
                return false;
            }
        }

        public HRAndApplicantSystem.Applicant.Applicant GetApplicantByUsername(string username)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Get UserID from username first
                    int userID = GetUserIdByUsername(username);
                    if (userID == -1)
                    {
                        return null;
                    }

                    // Query applicant by UserID
                    string query = "SELECT [ApplicantID], [First Name], [Last Name], [ContactNo], [Address], [Education], [Skills] FROM [Applicants] WHERE [UserID] = @userID";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userID", userID);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new HRAndApplicantSystem.Applicant.Applicant
                                {
                                    ApplicantID = reader["ApplicantID"] != DBNull.Value ? Convert.ToInt32(reader["ApplicantID"]) : 0,
                                    Username = username,
                                    FirstName = reader["First Name"]?.ToString() ?? "",
                                    LastName = reader["Last Name"]?.ToString() ?? "",
                                    ContactNo = reader["ContactNo"]?.ToString() ?? "",
                                    Address = reader["Address"]?.ToString() ?? "",
                                    Education = reader["Education"]?.ToString() ?? "",
                                    Skills = reader["Skills"]?.ToString() ?? ""
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving applicant info: {ex.Message}");
                // Fallback: Try to get by most recent applicant (for backward compatibility)
                // This shouldn't happen if UserID column exists
            }

            return null;
        }

        public bool UpdateApplicantInfo(string username, HRAndApplicantSystem.Applicant.Applicant applicant)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Verify user is an applicant (RoleID = 1)
                    int roleID = GetUserRoleByUsername(username);
                    if (roleID != 1)
                    {
                        Console.WriteLine("Only applicants can update applicant information.");
                        return false;
                    }

                    // Update using ApplicantID as the primary key
                    string query = "UPDATE [Applicants] SET [First Name] = @firstName, [Last Name] = @lastName, [ContactNo] = @contactNo, [Address] = @address, [Education] = @education, [Skills] = @skills WHERE [ApplicantID] = @applicantId";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@firstName", applicant.FirstName ?? "");
                        cmd.Parameters.AddWithValue("@lastName", applicant.LastName ?? "");
                        cmd.Parameters.AddWithValue("@contactNo", applicant.ContactNo ?? "");
                        cmd.Parameters.AddWithValue("@address", applicant.Address ?? "");
                        cmd.Parameters.AddWithValue("@education", applicant.Education ?? "");
                        cmd.Parameters.AddWithValue("@skills", applicant.Skills ?? "");
                        cmd.Parameters.AddWithValue("@applicantId", applicant.ApplicantID);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating applicant info: {ex.Message}");
                return false;
            }
        }

        private int GetUserIdByUsername(string username)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT [UserID] FROM [Users] WHERE [Username] = @username";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user ID: {ex.Message}");
            }

            return -1;
        }

        public int GetUserRoleByUsername(string username)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT [RoleID] FROM [Users] WHERE [Username] = @username";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user role: {ex.Message}");
            }

            return -1;
        }

        public List<HRAndApplicantSystem.Models.JobVacancy> GetAllJobVacancies()
        {
            List<HRAndApplicantSystem.Models.JobVacancy> vacancies = new List<HRAndApplicantSystem.Models.JobVacancy>();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT [JobID], [JobTitle], [JobDetail], [JobSlots], [Status] FROM [JobVacancies] ORDER BY [JobID]";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                vacancies.Add(new HRAndApplicantSystem.Models.JobVacancy
                                {
                                    JobID = Convert.ToInt32(reader["JobID"]),
                                    JobTitle = reader["JobTitle"]?.ToString() ?? "",
                                    JobDetail = reader["JobDetail"]?.ToString() ?? "",
                                    JobSlots = Convert.ToInt32(reader["JobSlots"]),
                                    Status = reader["Status"]?.ToString() ?? ""
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving job vacancies: {ex.Message}");
            }

            return vacancies;
        }

        public bool SubmitJobApplication(int applicantID, int jobID)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Check if applicant has already applied for this job
                    string checkQuery = "SELECT COUNT(*) FROM [Applications] WHERE [ApplicantID] = @applicantID AND [JobID] = @jobID";

                    using (OleDbCommand checkCmd = new OleDbCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@applicantID", applicantID);
                        checkCmd.Parameters.AddWithValue("@jobID", jobID);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            Console.WriteLine("You have already applied for this job.");
                            return false;
                        }
                    }

                    // Insert new application
                    string query = "INSERT INTO [Applications] ([ApplicantID], [JobID], [Status], [DateApplied]) VALUES (@applicantID, @jobID, @status, @dateApplied)";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@applicantID", applicantID);
                        cmd.Parameters.AddWithValue("@jobID", jobID);
                        cmd.Parameters.AddWithValue("@status", "Applied");
                        cmd.Parameters.AddWithValue("@dateApplied", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error submitting application: {ex.Message}");
                return false;
            }
        }

        public List<dynamic> GetApplicantApplications(int applicantID)
        {
            List<dynamic> applications = new List<dynamic>();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT 
                                        a.[ApplicationID], 
                                        a.[Status], 
                                        a.[DateApplied], 
                                        jv.[JobTitle],
                                        jv.[JobID]
                                    FROM [Applications] a
                                    INNER JOIN [JobVacancies] jv ON a.[JobID] = jv.[JobID]
                                    WHERE a.[ApplicantID] = @applicantID
                                    ORDER BY a.[DateApplied] DESC";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@applicantID", applicantID);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dynamic app = new
                                {
                                    ApplicationID = Convert.ToInt32(reader["ApplicationID"]),
                                    Status = reader["Status"]?.ToString() ?? "",
                                    DateApplied = Convert.ToDateTime(reader["DateApplied"]),
                                    JobTitle = reader["JobTitle"]?.ToString() ?? "",
                                    JobID = Convert.ToInt32(reader["JobID"])
                                };
                                applications.Add(app);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving applications: {ex.Message}");
            }

            return applications;
        }
    }
}
