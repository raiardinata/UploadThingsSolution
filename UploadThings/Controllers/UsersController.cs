using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UploadThings.Data;
using UploadThings.Models;
using UploadThings.UnitofWork;

namespace UploadThings.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MSSQLContext _context;

        public UsersController(IUnitOfWork unitOfWork, MSSQLContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = "User Input fail to validate.";
            return View(user);
        }

        [HttpGet("GetUsernames")]
        public async Task<IActionResult> GetUsernames(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Ok(new List<string>()); // Return an empty list
            }

            var usernames = await _context.Users
                .Where(u => u.Name.StartsWith(query))
                .Select(u => u.Name)
                .ToListAsync();

            return Ok(usernames); // Return list of usernames
        }

        [HttpGet("GetUsernames")]
        public async Task<IActionResult> CheckUsernameExists(string username)
        {
            bool exists = await _context.Users.AnyAsync(u => u.Name == username);
            return Json(exists);
        }
    }
}
