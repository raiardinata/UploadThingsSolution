using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UploadThings.Models;
using UploadThings.Models.Factories;
using UploadThings.Services.Factories;

namespace UploadThings.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly JsonOverideFactory _jsonOverideFactory;
        private readonly MSSQLDatabaseCheckerFactory _mssqlFactory;
        private readonly MariaDBDatabaseCheckerFactory _mariadbFactory;
        private readonly PostgresDatabaseCheckerFactory _postgresFactory;

        public HomeController(
            ILogger<HomeController> logger,
            IConfiguration configuration,
            JsonOverideFactory jsonOverideFactory,
            MSSQLDatabaseCheckerFactory mssqlFactory,
            MariaDBDatabaseCheckerFactory mariadbFactory,
            PostgresDatabaseCheckerFactory postgresFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _configurationRoot = (IConfigurationRoot)configuration;
            _jsonOverideFactory = jsonOverideFactory;
            _mssqlFactory = mssqlFactory;
            _mariadbFactory = mariadbFactory;
            _postgresFactory = postgresFactory;
        }

        public IActionResult Index()
        {
            ViewBag.Message = TempData["Message"];
            if (_configuration["DatabaseSetup:Initiate"] != "1")
            {
                var mssqlChecker = _mssqlFactory.CreateDatabaseChecker(_configuration.GetConnectionString("MSSQLConnection"));
                Exception mssqlCheckerValid = mssqlChecker.CheckAndCreateDatabase();
                if (mssqlCheckerValid.Message != "null")
                {
                    TempData["Message"] += "MSSQL Database Checkup Failed! | ";
                }

                var mariadbChecker = _mariadbFactory.CreateDatabaseChecker(_configuration.GetConnectionString("MariaDBConnection"));
                Exception mariadbCheckerValid = mariadbChecker.CheckAndCreateDatabase();
                if (mariadbCheckerValid.Message != "null")
                {
                    TempData["Message"] += "MariaDB Database Checkup Failed! | ";
                }

                var postgresChecker = _postgresFactory.CreateDatabaseChecker(_configuration.GetConnectionString("PostgresGeneralConnection"));
                Exception postgresCheckerValid = postgresChecker.CheckAndCreateDatabase();
                if (postgresCheckerValid.Message != "null")
                {
                    TempData["Message"] += "Postgres Database Checkup Failed! | ";
                }

                postgresChecker = _postgresFactory.CreateDatabaseChecker(_configuration.GetConnectionString("PostgresTransactionConnection"));
                postgresCheckerValid = postgresChecker.CheckAndCreateDatabase();
                if (postgresCheckerValid.Message != "null")
                {
                    TempData["Message"] += "Postgres Database Checkup Failed! | ";
                }

                var jsonOveride = _jsonOverideFactory.UpdateAppSetting("DatabaseSetup", "Initiate", "1", "appsettings.json");
                Exception jsonOverideValid = jsonOveride.UpdateAppSetting();
                if (jsonOverideValid.Message != "null")
                {
                    TempData["Message"] += "jsonOveride Failed! | ";
                }

                _configurationRoot.Reload();
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
