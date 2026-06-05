using System;
using System.Data.OleDb;

public class DatabaseHelper
{
    private static string connectionString =
        @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=HRDatabase.accdb;";

    public static OleDbConnection GetConnection()
    {
        return new OleDbConnection(connectionString);
    }
}