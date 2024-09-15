using Microsoft.Data.SqlClient;


namespace UploadThings.Models.DatabaseChecker
{
    public class MSSQLDatabaseChecker : IDatabaseChecker
    {
        private readonly string _connectionString;
        private readonly string databaseName = "UserDB";
        public MSSQLDatabaseChecker(string connectionString)
        {
            _connectionString = connectionString;
        }
        public Exception CheckAndCreateDatabase()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"IF DB_ID('UserDB') IS NULL CREATE Database {databaseName};";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF NOT EXISTS
                                    (
                                        SELECT 
                                            * 
                                        FROM 
                                            UserDB.dbo.sysobjects 
                                        WHERE 
                                            name = 'Users' 
                                            AND xtype = 'U'
                                    ) CREATE TABLE UserDB.dbo.Users(Id INT IDENTITY(1,1) PRIMARY KEY, Name VARCHAR(255), Email VARCHAR(255));";
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    return new Exception("null");
                }
            }
            catch (Exception ex)
            {
                return new Exception($"Failed when checking MSSQL, detail : {ex}");
            }
        }
    }
}
