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
            string dbPath = "";
            
            // Try multiple strategies to find the database
            // Strategy 1: Relative to the assembly location (works in VS and VS Code)
            string assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
            dbPath = Path.Combine(assemblyDir, "..", "..", "..", "Database", "HRApplicantData.accdb");
            dbPath = Path.GetFullPath(dbPath);
            
            if (!File.Exists(dbPath))
            {
                // Strategy 2: Try from current working directory
                dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "HRApplicantData.accdb");
            }
            
            if (!File.Exists(dbPath))
            {
                // Strategy 3: Try one more level up
                dbPath = Path.Combine(assemblyDir, "..", "..", "..", "..", "Database", "HRApplicantData.accdb");
                dbPath = Path.GetFullPath(dbPath);
            }
            
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
                        Console.WriteLine($"Error: UserID not found for username: {username}");
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
                                var applicant = new Applicant
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
                                System.Diagnostics.Debug.WriteLine($"GetApplicantByUsername - Username: {username}, UserID: {userID}, ApplicantID: {applicant.ApplicantID}");
                                return applicant;
                            }
                            else
                            {
                                Console.WriteLine($"Error: Applicant profile not found for UserID: {userID}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving applicant info: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
                // Fallback: Try to get by most recent applicant (for backward compatibility)
                // This shouldn't happen if UserID column exists
            }

            return null;
        }

        public Applicant GetApplicantByID(int applicantID)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT [ApplicantID], [First Name], [Last Name], [ContactNo], [Address], [Education], [Skills] FROM [Applicants] WHERE [ApplicantID] = @applicantID";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@applicantID", applicantID);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Applicant
                                {
                                    ApplicantID = reader["ApplicantID"] != DBNull.Value ? Convert.ToInt32(reader["ApplicantID"]) : 0,
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

        public int GetUserIDByUsername(string username)
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

        public string GetUsernameByUserID(int userID)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT [Username] FROM [Users] WHERE [UserID] = @userID";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userID", userID);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting username: {ex.Message}");
            }

            return "Unknown";
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

        public string GetRoleNameByUsername(string username)
        {
            try
            {
                int roleID = GetUserRoleByUsername(username);
                
                return roleID switch
                {
                    RoleConstants.APPLICANT => "Applicant",
                    RoleConstants.HR => "HR Staff",
                    RoleConstants.HR_MANAGER => "HR Manager",
                    RoleConstants.ADMIN => "Admin",
                    _ => "Unknown"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting role name: {ex.Message}");
            }

            return "Unknown";
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

        public JobVacancy GetJobVacancyByID(int jobID)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT [JobID], [JobTitle], [JobDetail], [Status] FROM [JobVacancies] WHERE [JobID] = @jobID";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@jobID", jobID);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new JobVacancy
                                {
                                    JobID = Convert.ToInt32(reader["JobID"]),
                                    JobTitle = reader["JobTitle"]?.ToString() ?? "",
                                    JobDetail = reader["JobDetail"]?.ToString() ?? "",
                                    Status = reader["Status"]?.ToString() ?? ""
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving job vacancy: {ex.Message}");
            }

            return null;
        }

        public bool HasApplicantAppliedForJob(int applicantID, int jobID)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Check if applicant has a non-Draft application for this job
                    string checkQuery = "SELECT COUNT(*) FROM [Applications] WHERE [ApplicantID] = @applicantID AND [JobID] = @jobID AND [Status] <> 'Draft'";

                    using (OleDbCommand checkCmd = new OleDbCommand(checkQuery, conn))
                    {
                        OleDbParameter applicantParam = new OleDbParameter("@applicantID", OleDbType.Integer);
                        applicantParam.Value = applicantID;
                        checkCmd.Parameters.Add(applicantParam);

                        OleDbParameter jobParam = new OleDbParameter("@jobID", OleDbType.Integer);
                        jobParam.Value = jobID;
                        checkCmd.Parameters.Add(jobParam);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking application status: {ex.Message}");
                return false;
            }
        }

        public int CreateDraftApplication(int applicantID, int jobID)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Check if a draft already exists for this job
                    string checkQuery = "SELECT [ApplicationID] FROM [Applications] WHERE [ApplicantID] = @applicantID AND [JobID] = @jobID AND [Status] = 'Draft'";

                    using (OleDbCommand checkCmd = new OleDbCommand(checkQuery, conn))
                    {
                        OleDbParameter applicantParam = new OleDbParameter("@applicantID", OleDbType.Integer);
                        applicantParam.Value = applicantID;
                        checkCmd.Parameters.Add(applicantParam);

                        OleDbParameter jobParam = new OleDbParameter("@jobID", OleDbType.Integer);
                        jobParam.Value = jobID;
                        checkCmd.Parameters.Add(jobParam);

                        var existingDraft = checkCmd.ExecuteScalar();
                        if (existingDraft != null)
                        {
                            // Return existing draft ID
                            return Convert.ToInt32(existingDraft);
                        }
                    }

                    // Create new draft application
                    string insertQuery = "INSERT INTO [Applications] ([ApplicantID], [JobID], [Status], [DateApplied]) VALUES (@applicantID, @jobID, @status, @dateApplied)";

                    using (OleDbCommand cmd = new OleDbCommand(insertQuery, conn))
                    {
                        OleDbParameter applicantInsertParam = new OleDbParameter("@applicantID", OleDbType.Integer);
                        applicantInsertParam.Value = applicantID;
                        cmd.Parameters.Add(applicantInsertParam);

                        OleDbParameter jobInsertParam = new OleDbParameter("@jobID", OleDbType.Integer);
                        jobInsertParam.Value = jobID;
                        cmd.Parameters.Add(jobInsertParam);

                        OleDbParameter statusParam = new OleDbParameter("@status", OleDbType.VarWChar);
                        statusParam.Value = "Draft";
                        cmd.Parameters.Add(statusParam);

                        OleDbParameter dateParam = new OleDbParameter("@dateApplied", OleDbType.Date);
                        dateParam.Value = DateTime.Now;
                        cmd.Parameters.Add(dateParam);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // Get the inserted ApplicationID
                            string getIdQuery = "SELECT [ApplicationID] FROM [Applications] WHERE [ApplicantID] = @applicantID AND [JobID] = @jobID AND [Status] = 'Draft'";
                            using (OleDbCommand getIdCmd = new OleDbCommand(getIdQuery, conn))
                            {
                                getIdCmd.Parameters.Add(new OleDbParameter("@applicantID", applicantID));
                                getIdCmd.Parameters.Add(new OleDbParameter("@jobID", jobID));
                                var result = getIdCmd.ExecuteScalar();
                                return result != null ? Convert.ToInt32(result) : -1;
                            }
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating draft application: {ex.Message}");
                return -1;
            }
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
                        statusParam.Value = "Submitted";
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
                                        a.[ApplicantID],
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
                                    ApplicantID = Convert.ToInt32(reader["ApplicantID"]),
                                    ApplicationStatus = reader["Status"]?.ToString() ?? "",
                                    DateApplied = Convert.ToDateTime(reader["DateApplied"]),
                                    JobTitle = reader["JobTitle"]?.ToString() ?? "",
                                    JobID = Convert.ToInt32(reader["JobID"])
                                };
                                applications.Add(app);
                            }
                        }
                    }
                }

                // Debug: Show applicant ID and count of applications found
                System.Diagnostics.Debug.WriteLine($"GetApplicantApplications - ApplicantID: {applicantID}, Applications Found: {applications.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving applications for ApplicantID {applicantID}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
            }

            return applications;
        }

        public List<dynamic> GetJobRequirements(int jobID)
        {
            List<dynamic> requirements = new List<dynamic>();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Get all available requirement types for any job
                    string query = @"SELECT 
                                        [RequirementTypeID], 
                                        [RequirementName]
                                    FROM [RequirementTypes]
                                    ORDER BY [RequirementName]";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dynamic req = new
                                {
                                    RequirementTypeID = Convert.ToInt32(reader["RequirementTypeID"]),
                                    RequirementName = reader["RequirementName"]?.ToString() ?? ""
                                };
                                requirements.Add(req);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving job requirements: {ex.Message}");
            }

            return requirements;
        }

        public bool SubmitApplicantDocument(int applicantID, int jobID, int requirementTypeID, string remarks, string documentStatus = "Submitted")
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Check if document already exists for this job
                    string checkQuery = "SELECT [DocumentID] FROM [ApplicantDocuments] WHERE [ApplicantID] = @applicantID AND [JobID] = @jobID AND [RequirementTypeID] = @requirementTypeID";

                    using (OleDbCommand checkCmd = new OleDbCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@applicantID", applicantID);
                        checkCmd.Parameters.AddWithValue("@jobID", jobID);
                        checkCmd.Parameters.AddWithValue("@requirementTypeID", requirementTypeID);

                        object result = checkCmd.ExecuteScalar();
                        
                        if (result != null && result != DBNull.Value)
                        {
                            // Update existing document
                            string updateQuery = "UPDATE [ApplicantDocuments] SET [DocumentStatus] = @status, [Remarks] = @remarks WHERE [ApplicantID] = @applicantID AND [JobID] = @jobID AND [RequirementTypeID] = @requirementTypeID";

                            using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@status", documentStatus);
                                updateCmd.Parameters.AddWithValue("@remarks", remarks);
                                updateCmd.Parameters.AddWithValue("@applicantID", applicantID);
                                updateCmd.Parameters.AddWithValue("@jobID", jobID);
                                updateCmd.Parameters.AddWithValue("@requirementTypeID", requirementTypeID);

                                int rowsAffected = updateCmd.ExecuteNonQuery();
                                return rowsAffected > 0;
                            }
                        }
                    }

                    // Insert new document
                    string insertQuery = "INSERT INTO [ApplicantDocuments] ([ApplicantID], [JobID], [RequirementTypeID], [DocumentStatus], [Remarks]) VALUES (@applicantID, @jobID, @requirementTypeID, @status, @remarks)";

                    using (OleDbCommand insertCmd = new OleDbCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@applicantID", applicantID);
                        insertCmd.Parameters.AddWithValue("@jobID", jobID);
                        insertCmd.Parameters.AddWithValue("@requirementTypeID", requirementTypeID);
                        insertCmd.Parameters.AddWithValue("@status", documentStatus);
                        insertCmd.Parameters.AddWithValue("@remarks", remarks);

                        int rowsAffected = insertCmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error submitting applicant document: {ex.Message}");
                return false;
            }
        }

        public List<dynamic> GetApplicantDocuments(int applicantID, int jobID)
        {
            List<dynamic> documents = new List<dynamic>();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT 
                                        ad.[DocumentID],
                                        ad.[RequirementTypeID],
                                        rt.[RequirementName],
                                        ad.[DocumentStatus],
                                        ad.[Remarks]
                                    FROM [ApplicantDocuments] ad
                                    INNER JOIN [RequirementTypes] rt ON ad.[RequirementTypeID] = rt.[RequirementTypeID]
                                    WHERE ad.[ApplicantID] = @applicantID AND ad.[JobID] = @jobID
                                    ORDER BY rt.[RequirementName]";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@applicantID", applicantID);
                        cmd.Parameters.AddWithValue("@jobID", jobID);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dynamic doc = new
                                {
                                    DocumentID = Convert.ToInt32(reader["DocumentID"]),
                                    RequirementTypeID = Convert.ToInt32(reader["RequirementTypeID"]),
                                    RequirementName = reader["RequirementName"]?.ToString() ?? "",
                                    DocumentStatus = reader["DocumentStatus"]?.ToString() ?? "",
                                    Remarks = reader["Remarks"]?.ToString() ?? ""
                                };
                                documents.Add(doc);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving applicant documents: {ex.Message}");
            }

            return documents;
        }

        public List<dynamic> GetPendingApplicationsForScreening()
        {
            List<dynamic> applications = new List<dynamic>();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT [Applications].[ApplicationID], [Applications].[ApplicantID], [Applications].[JobID], 
                                           [Applications].[Status], [Applications].[DateApplied], [Applicants].[First Name], 
                                           [Applicants].[Last Name], [Applicants].[ContactNo], [JobVacancies].[JobTitle]
                                    FROM ([Applications] INNER JOIN [Applicants] ON [Applications].[ApplicantID] = [Applicants].[ApplicantID])
                                    INNER JOIN [JobVacancies] ON [Applications].[JobID] = [JobVacancies].[JobID]
                                    WHERE [Applications].[Status] = 'Submitted' OR [Applications].[Status] = 'Under Review'
                                    ORDER BY [Applications].[DateApplied]";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dynamic app = new
                                {
                                    ApplicationID = Convert.ToInt32(reader[0]),
                                    ApplicantID = Convert.ToInt32(reader[1]),
                                    JobID = Convert.ToInt32(reader[2]),
                                    Status = reader[3]?.ToString() ?? "",
                                    DateApplied = Convert.ToDateTime(reader[4]),
                                    FirstName = reader[5]?.ToString() ?? "",
                                    LastName = reader[6]?.ToString() ?? "",
                                    ContactNo = reader[7]?.ToString() ?? "",
                                    JobTitle = reader[8]?.ToString() ?? ""
                                };
                                applications.Add(app);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving pending applications: {ex.Message}");
            }

            return applications;
        }

        public dynamic GetApplicationDetailsForScreening(int applicationID)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT [Applications].[ApplicationID], [Applications].[ApplicantID], [Applications].[JobID],
                                           [Applications].[Status], [Applications].[DateApplied], [Applicants].[First Name],
                                           [Applicants].[Last Name], [Applicants].[ContactNo], [Applicants].[Address],
                                           [Applicants].[Education], [Applicants].[Skills], [JobVacancies].[JobTitle],
                                           [JobVacancies].[JobDetail]
                                    FROM ([Applications] INNER JOIN [Applicants] ON [Applications].[ApplicantID] = [Applicants].[ApplicantID])
                                    INNER JOIN [JobVacancies] ON [Applications].[JobID] = [JobVacancies].[JobID]
                                    WHERE [Applications].[ApplicationID] = @applicationID";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@applicationID", applicationID);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new
                                {
                                    ApplicationID = Convert.ToInt32(reader[0]),
                                    ApplicantID = Convert.ToInt32(reader[1]),
                                    JobID = Convert.ToInt32(reader[2]),
                                    Status = reader[3]?.ToString() ?? "",
                                    DateApplied = Convert.ToDateTime(reader[4]),
                                    FirstName = reader[5]?.ToString() ?? "",
                                    LastName = reader[6]?.ToString() ?? "",
                                    ContactNo = reader[7]?.ToString() ?? "",
                                    Address = reader[8]?.ToString() ?? "",
                                    Education = reader[9]?.ToString() ?? "",
                                    Skills = reader[10]?.ToString() ?? "",
                                    JobTitle = reader[11]?.ToString() ?? "",
                                    JobDetail = reader[12]?.ToString() ?? ""
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving application details: {ex.Message}");
            }

            return null;
        }

        public bool ScreenApplication(int applicationID, string result, string remarks, string hrUsername)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Determine new status based on screening result
                    string newStatus = result == "Qualified" ? "Shortlisted" : "Rejected";

                    // Update application status
                    string updateQuery = "UPDATE [Applications] SET [Status] = @newStatus WHERE [ApplicationID] = @applicationID";

                    using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@newStatus", newStatus);
                        updateCmd.Parameters.AddWithValue("@applicationID", applicationID);

                        int rowsAffected = updateCmd.ExecuteNonQuery();
                        if (rowsAffected == 0) return false;
                    }

                    // Record screening result
                    string screeningQuery = "INSERT INTO [ScreeningResults] ([ApplicationID], [Result], [Remarks], [ScreenedBy], [DateScreened]) VALUES (@appID, @result, @remarks, @screenedBy, @dateScreened)";

                    using (OleDbCommand screenCmd = new OleDbCommand(screeningQuery, conn))
                    {
                        screenCmd.Parameters.Add(new OleDbParameter("@appID", applicationID));
                        screenCmd.Parameters.Add(new OleDbParameter("@result", result));
                        screenCmd.Parameters.Add(new OleDbParameter("@remarks", remarks));
                        screenCmd.Parameters.Add(new OleDbParameter("@screenedBy", hrUsername));
                        
                        OleDbParameter dateParam = new OleDbParameter("@dateScreened", OleDbType.Date);
                        dateParam.Value = DateTime.Now;
                        screenCmd.Parameters.Add(dateParam);

                        screenCmd.ExecuteNonQuery();
                    }

                    // Record status history
                    RecordStatusChange(applicationID, newStatus, $"Screened by HR: {result}", hrUsername);

                    // Log audit trail
                    string screeningRole = GetRoleNameByUsername(hrUsername);
                    LogAuditTrail(screeningRole, hrUsername, $"Screened Application #{applicationID} - Result: {result}");

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error screening application: {ex.Message}");
                return false;
            }
        }

        public bool UpdateApplicationStatus(int applicationID, string newStatus, string remarks, string changedBy)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = "UPDATE [Applications] SET [Status] = @newStatus WHERE [ApplicationID] = @applicationID";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@newStatus", newStatus);
                        cmd.Parameters.AddWithValue("@applicationID", applicationID);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        
                        if (rowsAffected > 0)
                        {
                            RecordStatusChange(applicationID, newStatus, remarks, changedBy);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating application status: {ex.Message}");
            }

            return false;
        }

        public bool DeleteApplication(int applicationID)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = "DELETE FROM [Applications] WHERE [ApplicationID] = @applicationID";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@applicationID", applicationID);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting application: {ex.Message}");
                return false;
            }
        }

        public bool RecordStatusChange(int applicationID, string status, string remarks, string changedBy)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = "INSERT INTO [ApplicationStatusHistory] ([ApplicationID], [Status], [Remarks], [DateChanged], [ChangedBy]) VALUES (@appID, @status, @remarks, @dateChanged, @changedBy)";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OleDbParameter("@appID", applicationID));
                        cmd.Parameters.Add(new OleDbParameter("@status", status));
                        cmd.Parameters.Add(new OleDbParameter("@remarks", remarks));
                        
                        OleDbParameter dateParam = new OleDbParameter("@dateChanged", OleDbType.Date);
                        dateParam.Value = DateTime.Now;
                        cmd.Parameters.Add(dateParam);
                        
                        cmd.Parameters.Add(new OleDbParameter("@changedBy", changedBy));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error recording status change: {ex.Message}");
            }

            return false;
        }

        public bool LogAuditTrail(string userType, string username, string action)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Get UserID from username
                    int userID = GetUserIDByUsername(username);

                    string query = "INSERT INTO [AuditTrail] ([UserType], [UserID], [Action], [ActionDate]) VALUES (@userType, @userID, @action, @actionDate)";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OleDbParameter("@userType", userType));
                        cmd.Parameters.Add(new OleDbParameter("@userID", userID > 0 ? userID : (object)DBNull.Value));
                        cmd.Parameters.Add(new OleDbParameter("@action", action));
                        
                        OleDbParameter dateParam = new OleDbParameter("@actionDate", OleDbType.Date);
                        dateParam.Value = DateTime.Now;
                        cmd.Parameters.Add(dateParam);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging audit trail: {ex.Message}");
            }

            return false;
        }

        // Legacy overload for backward compatibility
        public bool LogAuditTrail(string userType, string action)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = "INSERT INTO [AuditTrail] ([UserType], [Action], [ActionDate]) VALUES (@userType, @action, @actionDate)";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OleDbParameter("@userType", userType));
                        cmd.Parameters.Add(new OleDbParameter("@action", action));
                        
                        OleDbParameter dateParam = new OleDbParameter("@actionDate", OleDbType.Date);
                        dateParam.Value = DateTime.Now;
                        cmd.Parameters.Add(dateParam);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging audit trail: {ex.Message}");
            }

            return false;
        }

        public List<dynamic> GetAuditTrail(int limit = 50)
        {
            List<dynamic> auditLogs = new List<dynamic>();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = $@"SELECT TOP {limit} [AuditID], [UserType], [UserID], [Action], [ActionDate]
                                     FROM [AuditTrail]
                                     ORDER BY [ActionDate] DESC";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int userID = reader["UserID"] != DBNull.Value ? Convert.ToInt32(reader["UserID"]) : 0;
                                string username = userID > 0 ? GetUsernameByUserID(userID) : "Unknown";

                                auditLogs.Add(new
                                {
                                    AuditID = reader["AuditID"],
                                    UserType = reader["UserType"],
                                    Username = username,
                                    Action = reader["Action"],
                                    ActionDate = reader["ActionDate"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving audit trail: {ex.Message}");
            }

            return auditLogs;
        }

        public List<dynamic> GetApplicationsByStatus(string status)
        {
            List<dynamic> applications = new List<dynamic>();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT [Applications].[ApplicationID], [Applications].[ApplicantID], [Applications].[JobID],
                                           [Applications].[Status], [Applications].[DateApplied], [Applicants].[First Name],
                                           [Applicants].[Last Name], [JobVacancies].[JobTitle]
                                    FROM ([Applications] INNER JOIN [Applicants] ON [Applications].[ApplicantID] = [Applicants].[ApplicantID])
                                    INNER JOIN [JobVacancies] ON [Applications].[JobID] = [JobVacancies].[JobID]
                                    WHERE [Applications].[Status] = @status
                                    ORDER BY [Applications].[DateApplied]";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", status);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dynamic app = new
                                {
                                    ApplicationID = Convert.ToInt32(reader[0]),
                                    ApplicantID = Convert.ToInt32(reader[1]),
                                    JobID = Convert.ToInt32(reader[2]),
                                    Status = reader[3]?.ToString() ?? "",
                                    DateApplied = Convert.ToDateTime(reader[4]),
                                    FirstName = reader[5]?.ToString() ?? "",
                                    LastName = reader[6]?.ToString() ?? "",
                                    JobTitle = reader[7]?.ToString() ?? ""
                                };
                                applications.Add(app);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving applications by status: {ex.Message}");
            }

            return applications;
        }

        public List<dynamic> GetApplicationsByJob(int jobID)
        {
            List<dynamic> applications = new List<dynamic>();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT [Applications].[ApplicationID], [Applications].[ApplicantID], [Applications].[JobID],
                                           [Applications].[Status], [Applications].[DateApplied], [Applicants].[First Name],
                                           [Applicants].[Last Name], [JobVacancies].[JobTitle]
                                    FROM ([Applications] INNER JOIN [Applicants] ON [Applications].[ApplicantID] = [Applicants].[ApplicantID])
                                    INNER JOIN [JobVacancies] ON [Applications].[JobID] = [JobVacancies].[JobID]
                                    WHERE [Applications].[JobID] = @jobID
                                    ORDER BY [Applications].[DateApplied]";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@jobID", jobID);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dynamic app = new
                                {
                                    ApplicationID = Convert.ToInt32(reader[0]),
                                    ApplicantID = Convert.ToInt32(reader[1]),
                                    JobID = Convert.ToInt32(reader[2]),
                                    Status = reader[3]?.ToString() ?? "",
                                    DateApplied = Convert.ToDateTime(reader[4]),
                                    FirstName = reader[5]?.ToString() ?? "",
                                    LastName = reader[6]?.ToString() ?? "",
                                    JobTitle = reader[7]?.ToString() ?? ""
                                };
                                applications.Add(app);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving applications by job: {ex.Message}");
            }

            return applications;
        }
            // Schedule Interview 
        public bool ScheduleInterview(int applicationID, DateTime interviewDateTime,string interviewer, string mode, string location, string scheduledBy)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Insert into InterviewSchedules
                    string insertQuery = @"INSERT INTO [InterviewSchedules] 
                        ([ApplicationID], [InterviewDate], [InterviewTime], [Interviewer], [Location], [Status])
                        VALUES (@appID, @date, @time, @interviewer, @location, @status)";

                    using (OleDbCommand cmd = new OleDbCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@appID", applicationID);
                        cmd.Parameters.AddWithValue("@date", interviewDateTime.Date);
                        cmd.Parameters.AddWithValue("@time", interviewDateTime.TimeOfDay);
                        cmd.Parameters.AddWithValue("@interviewer", interviewer);
                        cmd.Parameters.AddWithValue("@location", location);
                        cmd.Parameters.AddWithValue("@status", "Scheduled");
                        cmd.ExecuteNonQuery();
                    }

                    // Update application status to Interview Scheduled
                    string updateQuery = "UPDATE [Applications] SET [Status] = 'Interview Scheduled' WHERE [ApplicationID] = @appID";
                    using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@appID", applicationID);
                        updateCmd.ExecuteNonQuery();
                    }

                    // Record status history
                    RecordStatusChange(applicationID, "Interview Scheduled",
                        $"Interview scheduled on {interviewDateTime:MMMM dd, yyyy HH:mm}", scheduledBy);

                    // Log audit trail
                    string interviewRole = GetRoleNameByUsername(scheduledBy);
                    LogAuditTrail(interviewRole, scheduledBy, $"Scheduled Interview for Application #{applicationID} on {interviewDateTime:MMMM dd, yyyy HH:mm}");

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scheduling interview: {ex.Message}");
                return false;
            }
        }

        public dynamic GetInterviewSchedule(int applicationID)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT [ScheduleID], [ApplicationID], [InterviewDate], [InterviewTime], 
                                           [Interviewer], [Location], [Status]
                                    FROM [InterviewSchedules]
                                    WHERE [ApplicationID] = @appID";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@appID", applicationID);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new
                                {
                                    ScheduleID = reader["ScheduleID"],
                                    ApplicationID = reader["ApplicationID"],
                                    InterviewDate = reader["InterviewDate"],
                                    InterviewTime = reader["InterviewTime"],
                                    Interviewer = reader["Interviewer"],
                                    Location = reader["Location"],
                                    Status = reader["Status"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving interview schedule: {ex.Message}");
            }

            return null;
        }

         // Evaluate Interview 
        public bool EvaluateInterview(int applicationID, int score, string result,
            string remarks, string newStatus, string evaluatedBy)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Insert into InterviewEvaluations
                    string insertQuery = @"INSERT INTO [InterviewEvaluations]
                        ([ApplicationID], [Score], [Result], [Remarks], [DateEvaluated])
                        VALUES (@appID, @score, @result, @remarks, @dateEvaluated)";

                    using (OleDbCommand cmd = new OleDbCommand(insertQuery, conn))
                    {
                        cmd.Parameters.Add(new OleDbParameter("@appID", applicationID));
                        cmd.Parameters.Add(new OleDbParameter("@score", score));
                        cmd.Parameters.Add(new OleDbParameter("@result", result));
                        cmd.Parameters.Add(new OleDbParameter("@remarks", remarks));
                        
                        OleDbParameter dateParam = new OleDbParameter("@dateEvaluated", OleDbType.Date);
                        dateParam.Value = DateTime.Now;
                        cmd.Parameters.Add(dateParam);
                        
                        cmd.ExecuteNonQuery();
                    }

                    // Update interview schedule status to Completed
                    string updateSchedule = "UPDATE [InterviewSchedules] SET [Status] = 'Completed' WHERE [ApplicationID] = @appID";
                    using (OleDbCommand schedCmd = new OleDbCommand(updateSchedule, conn))
                    {
                        schedCmd.Parameters.AddWithValue("@appID", applicationID);
                        schedCmd.ExecuteNonQuery();
                    }

                    // Update application status
                    string updateApp = "UPDATE [Applications] SET [Status] = @newStatus WHERE [ApplicationID] = @appID";
                    using (OleDbCommand appCmd = new OleDbCommand(updateApp, conn))
                    {
                        appCmd.Parameters.AddWithValue("@newStatus", newStatus);
                        appCmd.Parameters.AddWithValue("@appID", applicationID);
                        appCmd.ExecuteNonQuery();
                    }

                    // Record status history
                    RecordStatusChange(applicationID, newStatus,
                        $"Interview evaluated: {result} (Score: {score}/100). {remarks}", evaluatedBy);

                    // Log audit trail
                    string evaluationRole = GetRoleNameByUsername(evaluatedBy);
                    LogAuditTrail(evaluationRole, evaluatedBy, $"Evaluated Interview for Application #{applicationID} - Result: {result} (Score: {score}/100)");

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error evaluating interview: {ex.Message}");
                return false;
            }
        }

        // ── Final Hiring Decision ─────────────────────────────────
        public bool MakeHiringDecision(int applicationID, string decision,
            string remarks, string decidedBy)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Insert into HiringDecisions
                    string insertQuery = @"INSERT INTO [HiringDecisions]
                        ([ApplicationID], [Decision], [Remarks], [DecisionBy], [DecisionDate])
                        VALUES (@appID, @decision, @remarks, @decidedBy, @dateDecided)";

                    using (OleDbCommand cmd = new OleDbCommand(insertQuery, conn))
                    {
                        cmd.Parameters.Add(new OleDbParameter("@appID", applicationID));
                        cmd.Parameters.Add(new OleDbParameter("@decision", decision));
                        cmd.Parameters.Add(new OleDbParameter("@remarks", remarks));
                        cmd.Parameters.Add(new OleDbParameter("@decidedBy", decidedBy));
                        
                        OleDbParameter dateParam = new OleDbParameter("@dateDecided", OleDbType.Date);
                        dateParam.Value = DateTime.Now;
                        cmd.Parameters.Add(dateParam);
                        
                        cmd.ExecuteNonQuery();
                    }

                    // Update application status
                    string updateApp = "UPDATE [Applications] SET [Status] = @decision WHERE [ApplicationID] = @appID";
                    using (OleDbCommand appCmd = new OleDbCommand(updateApp, conn))
                    {
                        appCmd.Parameters.AddWithValue("@decision", decision);
                        appCmd.Parameters.AddWithValue("@appID", applicationID);
                        appCmd.ExecuteNonQuery();
                    }

                    // Record status history
                    RecordStatusChange(applicationID, decision,
                        $"Final decision: {decision}. {remarks}", decidedBy);

                    // Log audit trail
                    string decisionRole = GetRoleNameByUsername(decidedBy);
                    LogAuditTrail(decisionRole, decidedBy, $"Made Hiring Decision for Application #{applicationID} - Decision: {decision}");

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error recording hiring decision: {ex.Message}");
                return false;
            }
        }
        // ── JOB VACANCY MANAGEMENT METHODS ───────────────────────
        public bool CreateJobVacancy(JobVacancy job)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO [JobVacancies] ([JobTitle], [JobDetail], [Status]) VALUES (@title, @detail, @status)";
                    
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", job.JobTitle ?? "");
                        cmd.Parameters.AddWithValue("@detail", job.JobDetail ?? "");
                        cmd.Parameters.AddWithValue("@status", job.Status ?? "Open");
                        
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating job vacancy: {ex.Message}");
                return false;
            }
        }

        public bool UpdateJobVacancy(JobVacancy job)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE [JobVacancies] SET [JobTitle] = @title, [JobDetail] = @detail, [Status] = @status WHERE [JobID] = @id";
                    
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", job.JobTitle ?? "");
                        cmd.Parameters.AddWithValue("@detail", job.JobDetail ?? "");
                        cmd.Parameters.AddWithValue("@status", job.Status ?? "Open");
                        cmd.Parameters.AddWithValue("@id", job.JobID);
                        
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating job vacancy: {ex.Message}");
                return false;
            }
        }

        public bool DeleteJobVacancy(int jobID)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM [JobVacancies] WHERE [JobID] = @id";
                    
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", jobID);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting job vacancy: {ex.Message}");
                return false;
           }
        }

        public bool ChangeUserPassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                // First validate that the old password is correct
                if (!ValidateLogin(username, oldPassword))
                {
                    return false;
                }

                // Password is correct, now update it
                return UpdatePasswordHash(username, newPassword);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing password: {ex.Message}");
                return false;
            }
        }

        public bool ChangeUsername(string oldUsername, string newUsername)
        {
            try
            {
                // Check if new username already exists
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM [Users] WHERE [Username] = @username";
                    using (OleDbCommand cmd = new OleDbCommand(checkQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", newUsername);
                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            return false; // Username already exists
                        }
                    }

                    // Username doesn't exist, proceed with update
                    string updateQuery = "UPDATE [Users] SET [Username] = @newUsername WHERE [Username] = @oldUsername";
                    using (OleDbCommand cmd = new OleDbCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@newUsername", newUsername);
                        cmd.Parameters.AddWithValue("@oldUsername", oldUsername);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing username: {ex.Message}");
                return false;
            }
        }
    }
}
