using MySqlConnector;

namespace UploadThings.Models.DatabaseChecker
{
    public class MariaDBDatabaseChecker : IDatabaseChecker
    {
        private readonly string _connectionString;
        private readonly string databaseName = "ProductDB";
        public MariaDBDatabaseChecker(string connectionString)
        {
            _connectionString = connectionString;
        }
        public Exception CheckAndCreateDatabase()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS {databaseName};";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                                        CREATE TABLE IF NOT EXISTS ProductDB.Products 
                                        (Id INT AUTO_INCREMENT PRIMARY KEY, ProductName VARCHAR(255), Price DECIMAL(18,2), ProductImagePath VARCHAR(255), TypeofProduct VARCHAR(255));";
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    return new Exception("null");
                }
            }
            catch (Exception ex)
            {
                return new Exception($"Failed when checking MariaDB, detail : {ex}");
            }
        }

    }
}
