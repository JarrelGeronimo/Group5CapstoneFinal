using System;
using System.Data.OleDb;
using System.IO;

namespace HRApplicantSystem.Database
{
    public class DatabaseHelper
    {
        private static string dbPath = Path.Combine(AppContext.BaseDirectory, "Database", "HRApplicantData.accdb");

        private static string connString =
            $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};";

        public static OleDbConnection GetConnection()
        {
            return new OleDbConnection(connString);
        }
    }
}