using Microsoft.AspNetCore.Mvc;
using UploadThings.Models;
using UploadThings.UnitofWork;

namespace UploadThings.Controllers
{
    public class ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile ProductImage)
        {
            ModelState.Remove("ProductImagePath");
            if (ProductImage != null && ProductImage.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", ProductImage.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProductImage.CopyToAsync(stream);
                }
                product.ProductImagePath = "/images/" + ProductImage.FileName;
            }

            if (ModelState.IsValid)
            {
                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = "User Input fail to validate.";
            return View(product);
        }
    }

}
