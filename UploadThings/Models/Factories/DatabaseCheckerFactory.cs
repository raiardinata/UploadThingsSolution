namespace UploadThings.Models.Factories
{
    public abstract class DatabaseCheckerFactory
    {
        public abstract IDatabaseChecker CreateDatabaseChecker(string connectionString);
    }
}
