using UploadThings.Models.DatabaseChecker;

namespace UploadThings.Models.Factories
{
    public class MariaDBDatabaseCheckerFactory : DatabaseCheckerFactory
    {
        public override IDatabaseChecker CreateDatabaseChecker(string connectionString)
        {
            return new MariaDBDatabaseChecker(connectionString);
        }
    }
}
