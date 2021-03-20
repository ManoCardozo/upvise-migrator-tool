using System;
using System.Configuration;
using System.Data.SqlClient;
using com.upvise.client;

namespace UpviseMigratorTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Checking Upvise Connection...");
            var canConnectToUpvise = CanConnectToUpvise();
            PrintConnectonStatus(canConnectToUpvise);
            Console.WriteLine();

            Console.WriteLine("Checking Database Connection...");
            var canConnectToDatabase = CanConnectToDatabase();
            PrintConnectonStatus(canConnectToDatabase);
            Console.WriteLine();

            NotifyEndOfProgram();
        }

        private static bool CanConnectToUpvise()
        {
            try
            {
                var upviseLogin = ConfigurationManager.AppSettings["upviseLogin"];
                var upvisePassword = ConfigurationManager.AppSettings["upvisePassword"];

                var result = Query.login(upviseLogin, upvisePassword);

                return result != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool CanConnectToDatabase()
        {
            try
            {
                string datasource = ConfigurationManager.AppSettings["datasource"];
                string database = ConfigurationManager.AppSettings["database"];
                string connectionString = $"Data Source={datasource};Initial Catalog={database};Integrated Security=true";

                var connection = new SqlConnection(connectionString);

                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT TOP 1 * FROM Temp_DocumentsExport", connection))
                {
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var documentId = Convert.ToInt32(reader["DocumentID"].ToString());
                        if (documentId == null)
                        {
                            throw new Exception("Unexpected error: Unable to read document Id.");
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void PrintConnectonStatus(bool canConnect)
        {
            var status = canConnect ? "OK" : "NOT OK";
            var message = $"- Connection Status: {status}";

            Console.ForegroundColor = canConnect
                ? ConsoleColor.Green
                : ConsoleColor.Red;

            Console.WriteLine("----------------------------------");
            Console.WriteLine(message);
            Console.WriteLine("----------------------------------");
            Console.ResetColor();
        }

        private static void NotifyEndOfProgram()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
