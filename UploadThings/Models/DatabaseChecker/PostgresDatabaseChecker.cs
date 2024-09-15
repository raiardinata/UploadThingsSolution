using Npgsql;

namespace UploadThings.Models.DatabaseChecker
{
    public class PostgresDatabaseChecker : IDatabaseChecker
    {
        private readonly string _connectionString;
        private readonly string databaseName = "TransactionDB";

        public PostgresDatabaseChecker(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Exception CheckAndCreateDatabase()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();

                    if (_connectionString == "Host=127.0.0.1;Port=5432;Username=postgres;Password=r;")
                    {
                        cmd.CommandText = $"SELECT 1 FROM pg_database WHERE datname = '{databaseName}';";
                        var exists = cmd.ExecuteScalar();
                        if (exists != null)
                        {
                            return new Exception("null");
                        }
                        else
                        {
                            // If it doesn't exist, create the database
                            var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\";", connection);
                            createCmd.ExecuteNonQuery();
                            Console.WriteLine($"Database {databaseName} created successfully.");
                        }
                    }
                    else
                    {
                        cmd.CommandText = $"SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'transaction') AS table_existence;";
                        Boolean exists = (bool)cmd.ExecuteScalar();
                        if (exists)
                        {
                            return new Exception("null");
                        }
                        else
                        {
                            // Create Transaction table if it doesn't exist
                            cmd.CommandText = @$"
                                        CREATE TABLE IF NOT EXISTS Transactions 
                                        (Id SERIAL PRIMARY KEY, UserName VARCHAR(255), ProductName VARCHAR(255), Quantity DECIMAL(18,2), TotalPrice DECIMAL(18,2), UnitPrice DECIMAL(18,2), Date TIMESTAMP)";
                            cmd.ExecuteNonQuery();
                        }
                    }

                    connection.Close();
                    return new Exception("null");
                }
            }
            catch (Exception ex)
            {
                return new Exception($"Failed when checking Postgres, detail : {ex}");
            }
        }
    }
}
