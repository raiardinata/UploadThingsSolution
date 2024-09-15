using UploadThings.Models.DatabaseChecker;

namespace UploadThings.Models.Factories
{
    public class MSSQLDatabaseCheckerFactory : DatabaseCheckerFactory
    {
        public override IDatabaseChecker CreateDatabaseChecker(string connectionString)
        {
            return new MSSQLDatabaseChecker(connectionString);
        }
    }
}
