using UploadThings.Models.DatabaseChecker;

namespace UploadThings.Models.Factories
{
    public class PostgresDatabaseCheckerFactory : DatabaseCheckerFactory
    {
        public override IDatabaseChecker CreateDatabaseChecker(string connectionString)
        {
            return new PostgresDatabaseChecker(connectionString);
        }
    }
}
