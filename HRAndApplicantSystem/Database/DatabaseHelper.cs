using System;
using System.Collections.Generic;
using System.IO;
using System.Data.OleDb;
using HRAndApplicantSystem.Models;
using HRAndApplicantSystem.Utilities;

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

                    // Accept all valid roles
                    string query = "SELECT [Password] FROM [Users] WHERE [Username] = @username AND ([RoleID] IN (" + 
                        RoleConstants.APPLICANT + ", " + 
                        RoleConstants.HR + ", " + 
                        RoleConstants.HR_MANAGER + ", " + 
                        RoleConstants.ADMIN + "))";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                            return false;

                        string storedPassword = result.ToString();
                        
                        // Try to verify as a hashed password first
                        if (PasswordHasher.VerifyPassword(password, storedPassword))
                            return true;

                        // Fallback: Check if stored password is plain text (backward compatibility with existing accounts)
                        if (storedPassword.Equals(password, StringComparison.Ordinal))
                        {
                            // Hash the password and update the database for future logins
                            UpdatePasswordHash(username, password);
                            return true;
                        }

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login validation error: {ex.Message}");
                return false;
            }
        }

        private bool UpdatePasswordHash(string username, string plainPassword)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string hashedPassword = PasswordHasher.HashPassword(plainPassword);
                    string query = "UPDATE [Users] SET [Password] = @newPassword WHERE [Username] = @username";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@newPassword", hashedPassword);
                        cmd.Parameters.AddWithValue("@username", username);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating password hash: {ex.Message}");
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

                    // Hash the password before storing
                    string hashedPassword = PasswordHasher.HashPassword(password);

                    // Register with RoleID = 1 (Applicant)
                    string query = "INSERT INTO [Users] ([Username], [Password], [RoleID]) VALUES (@username, @password, @roleId)";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);
                        cmd.Parameters.AddWithValue("@roleId", (int)UserRole.Applicant);

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

        public bool SaveApplicantInfo(string username, Applicant applicant)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Verify user is an applicant (RoleID = 1)
                    int roleID = GetUserRoleByUsername(username);
                    if (roleID != RoleConstants.APPLICANT)
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

        public Applicant GetApplicantByUsername(string username)
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
                                return new Applicant
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

        public bool UpdateApplicantInfo(string username, Applicant applicant)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Verify user is an applicant (RoleID = 1)
                    int roleID = GetUserRoleByUsername(username);
                    if (roleID != RoleConstants.APPLICANT)
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

                    string query = "SELECT [JobID], [JobTitle], [JobDetail], [Status] FROM [JobVacancies] ORDER BY [JobID]";

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
                        OleDbParameter applicantParam = new OleDbParameter("@applicantID", OleDbType.Integer);
                        applicantParam.Value = applicantID;
                        checkCmd.Parameters.Add(applicantParam);

                        OleDbParameter jobParam = new OleDbParameter("@jobID", OleDbType.Integer);
                        jobParam.Value = jobID;
                        checkCmd.Parameters.Add(jobParam);

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
                        OleDbParameter applicantInsertParam = new OleDbParameter("@applicantID", OleDbType.Integer);
                        applicantInsertParam.Value = applicantID;
                        cmd.Parameters.Add(applicantInsertParam);

                        OleDbParameter jobInsertParam = new OleDbParameter("@jobID", OleDbType.Integer);
                        jobInsertParam.Value = jobID;
                        cmd.Parameters.Add(jobInsertParam);

                        OleDbParameter statusParam = new OleDbParameter("@status", OleDbType.VarWChar);
                        statusParam.Value = "Applied";
                        cmd.Parameters.Add(statusParam);
                        
                        OleDbParameter dateParam = new OleDbParameter("@dateApplied", OleDbType.Date);
                        dateParam.Value = DateTime.Now;
                        cmd.Parameters.Add(dateParam);

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
