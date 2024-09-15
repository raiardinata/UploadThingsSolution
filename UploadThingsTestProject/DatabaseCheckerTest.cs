using Microsoft.Extensions.Configuration;
using UploadThings.Models.Factories;
using UploadThings.Services.Factories;

namespace UploadThingsTestProject
{
    public class DatabaseCheckerTest
    {
        private IConfiguration? _configuration;
        private JsonOverideFactory? _jsonOverideFactory;
        private MSSQLDatabaseCheckerFactory? _mssqlFactory;
        private MariaDBDatabaseCheckerFactory? _mariadbFactory;
        private PostgresDatabaseCheckerFactory? _postgresFactory;

        [SetUp]
        public void Setup()
        {
            JsonOverideFactory jsonOverideFactory = new JsonOverideFactory();
            MSSQLDatabaseCheckerFactory mssqlFactory = new MSSQLDatabaseCheckerFactory();
            MariaDBDatabaseCheckerFactory mariadbFactory = new MariaDBDatabaseCheckerFactory();
            PostgresDatabaseCheckerFactory postgresFactory = new PostgresDatabaseCheckerFactory();

            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                .Build();

            _jsonOverideFactory = jsonOverideFactory;
            _mssqlFactory = mssqlFactory;
            _mariadbFactory = mariadbFactory;
            _postgresFactory = postgresFactory;
        }

        [TearDown]
        public void TearDown()
        {
            _configuration = null;
            _jsonOverideFactory = null;
            _mssqlFactory = null;
            _mariadbFactory = null;
            _postgresFactory = null;
        }

        [Test]
        // This test will check all database setup(mssql, mariadb, postgres). If the database setup are not exist, it will create base database and table for each database server.
        public void TestADatabaseCheckerSuccess()
        {
            var mssqlChecker = _mssqlFactory.CreateDatabaseChecker(_configuration.GetConnectionString("MSSQLConnection"));
            Exception mssqlCheckerValid = mssqlChecker.CheckAndCreateDatabase();
            if (mssqlCheckerValid.Message != "null")
            {
                Assert.Fail(mssqlCheckerValid.Message);
            }

            var mariadbChecker = _mariadbFactory.CreateDatabaseChecker(_configuration.GetConnectionString("MariaDBConnection"));
            Exception mariadbCheckerValid = mariadbChecker.CheckAndCreateDatabase();
            if (mariadbCheckerValid.Message != "null")
            {
                Assert.Fail(mariadbCheckerValid.Message);
            }

            var postgresChecker = _postgresFactory.CreateDatabaseChecker(_configuration.GetConnectionString("PostgresGeneralConnection"));
            Exception postgresCheckerValid = postgresChecker.CheckAndCreateDatabase();
            if (postgresCheckerValid.Message != "null")
            {
                Assert.Fail(postgresCheckerValid.Message);
            }

            postgresChecker = _postgresFactory.CreateDatabaseChecker(_configuration.GetConnectionString("PostgresTransactionConnection"));
            postgresCheckerValid = postgresChecker.CheckAndCreateDatabase();
            if (postgresCheckerValid.Message != "null")
            {
                Assert.Fail(postgresCheckerValid.Message);
            }

            var jsonOveride = _jsonOverideFactory.UpdateAppSetting("DatabaseSetup", "Initiate", "1", "appsettings.json");
            Exception jsonOverideValid = jsonOveride.UpdateAppSetting();
            if (jsonOverideValid.Message != "null")
            {
                Assert.Fail(jsonOverideValid.Message);
            }

            Assert.Pass();
        }

        [Test]
        public void TestBDatabaseCheckerFailed()
        {
            var mssqlChecker = _mssqlFactory.CreateDatabaseChecker(_configuration.GetConnectionString("PostgresTransactionConnection"));
            Exception mssqlCheckerValid = mssqlChecker.CheckAndCreateDatabase();
            if (mssqlCheckerValid.Message != "null")
            {
                Assert.Pass(mssqlCheckerValid.Message);
            }

            var mariadbChecker = _mariadbFactory.CreateDatabaseChecker(_configuration.GetConnectionString("MSSQLConnection"));
            Exception mariadbCheckerValid = mariadbChecker.CheckAndCreateDatabase();
            if (mariadbCheckerValid.Message != "null")
            {
                Assert.Pass(mariadbCheckerValid.Message);
            }

            var postgresChecker = _postgresFactory.CreateDatabaseChecker(_configuration.GetConnectionString("MariaDBConnection"));
            Exception postgresCheckerValid = postgresChecker.CheckAndCreateDatabase();
            if (postgresCheckerValid.Message != "null")
            {
                Assert.Pass(postgresCheckerValid.Message);
            }

            postgresChecker = _postgresFactory.CreateDatabaseChecker(_configuration.GetConnectionString("MariaDBConnection"));
            postgresCheckerValid = postgresChecker.CheckAndCreateDatabase();
            if (postgresCheckerValid.Message != "null")
            {
                Assert.Pass(postgresCheckerValid.Message);
            }

            var jsonOveride = _jsonOverideFactory.UpdateAppSetting("DatabaseSetup", "Initiate", "1", "appsettings.json");
            Exception jsonOverideValid = jsonOveride.UpdateAppSetting();
            if (jsonOverideValid.Message != "null")
            {
                Assert.Pass(jsonOverideValid.Message);
            }

            Assert.Fail("It suppose to be pass when there is an error.");
        }
    }
}