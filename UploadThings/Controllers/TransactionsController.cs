using Microsoft.AspNetCore.Mvc;
using UploadThings.Models;
using UploadThings.UnitofWork;

namespace UploadThings.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var transactions = await _unitOfWork.Transactions.GetAllAsync();
            return View(transactions);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Transactions.AddAsync(transaction);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = "Transaction Input fail to validate.";
            return View(transaction);
        }
    }
}
