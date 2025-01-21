using MySqlConnector;

namespace UploadThings.Models.DatabaseChecker
{
    public class MariaDBDatabaseChecker(string connectionString) : IDatabaseChecker
    {
        private readonly string _connectionString = connectionString;
        private readonly string databaseName = "ProductDB";

        public Exception CheckAndCreateDatabase()
        {
            try
            {
                using MySqlConnection connection = new(_connectionString);
                connection.Open();
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS {databaseName};";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"
                                        CREATE TABLE IF NOT EXISTS ProductDB.Products 
                                        (Id INT AUTO_INCREMENT PRIMARY KEY, ProductName VARCHAR(255), Price DECIMAL(18,2), ProductImagePath VARCHAR(255), TypeofProduct VARCHAR(255));";
                cmd.ExecuteNonQuery();
                connection.Close();
                return new Exception("null");
            }
            catch (Exception ex)
            {
                return new Exception($"Failed when checking MariaDB, detail : {ex.Message}");
            }
        }

    }
}
