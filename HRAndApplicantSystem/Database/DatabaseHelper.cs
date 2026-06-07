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
                                    WHERE [Applications].[Status] = 'Submitted'
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
                        screenCmd.Parameters.AddWithValue("@appID", applicationID);
                        screenCmd.Parameters.AddWithValue("@result", result);
                        screenCmd.Parameters.AddWithValue("@remarks", remarks);
                        screenCmd.Parameters.AddWithValue("@screenedBy", hrUsername);
                        screenCmd.Parameters.AddWithValue("@dateScreened", DateTime.Now);

                        screenCmd.ExecuteNonQuery();
                    }

                    // Record status history
                    RecordStatusChange(applicationID, "Under Review", $"Screened by HR: {result}", hrUsername);

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
                        cmd.Parameters.AddWithValue("@appID", applicationID);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@remarks", remarks);
                        cmd.Parameters.AddWithValue("@dateChanged", DateTime.Now);
                        cmd.Parameters.AddWithValue("@changedBy", changedBy);

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
    }
}
