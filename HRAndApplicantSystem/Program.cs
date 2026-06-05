using System;
using HRApplicantSystem.Database;

class Program
{
    static void Main()
    {
        try
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                Console.WriteLine("Database connected successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Connection failed: " + ex.Message);
        }
    }
}