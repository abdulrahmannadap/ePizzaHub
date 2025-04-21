using ePizzaHub.Core.Database;
using ePizzaHub.Core.Database.Entities;
using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Policy;

namespace ePizzaHub.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : BaseController
    {

        IMemoryCache _cache;
        private IItemService _itemService;
        IPaymentService _paymentService;
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController(IMemoryCache cache, IItemService itemService, ILogger<HomeController> logger, IPaymentService paymentService, AppDbContext context = null)
        {
            _cache = cache;
            _itemService = itemService;
            _logger = logger;
            _paymentService = paymentService;
            _context = context;
        }
        public IActionResult Index()
        {
            string key = "catalog";
            var items = _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpiration = DateTime.Now.AddHours(12);
                return _itemService.GetAll();
            });

            //  Shuffle (random order)
            var random = new Random();
            var shuffledItems = items.OrderBy(x => random.Next()).ToList();

            // Carousel images
            var layoutImageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "LayoutImage");
            var imageFiles = Directory.GetFiles(layoutImageFolder)
                                      .Select(Path.GetFileName)
                                      .ToList();

            ViewBag.CarouselImages = imageFiles;

            // Log error just for testing
            try
            {
                int x = 0, y = 4;
                int z = y / x;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return View(shuffledItems);
        }


        [HttpGet]
        public IActionResult TrackOrder()
        {
            var payments = _paymentService.GetAll();
            return View(payments);
        }

        [HttpGet]
        public IActionResult ItemList()
        {
            var item = _context.Items.Include(w=>w.Category).Include(w=>w.ItemType).OrderByDescending(w=>w.Id).ToList();

            return View(item);
        }


        [HttpGet]
        public IActionResult NewItem()
        {
           var categories = _context.Categories.ToList();
            var itemType = _context.ItemTypes.ToList();
            ViewBag.Categories = categories.Select(w => new SelectListItem { Text = w.Name, Value = w.Id.ToString() });
            ViewBag.ItemTypes = itemType.Select(w => new SelectListItem { Text = w.Name, Value = w.Id.ToString() });

            return View();
        }
        [HttpPost]
        public IActionResult Create(Item item, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Create "uploads" folder if not exists
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate a unique filename
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }

                    // Save filename to the model (adjust path if needed)
                    item.ImageUrl = "/images/" + uniqueFileName;
                }

                _context.Items.Add(item);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            // Reload ViewBags in case of validation error
            var categories = _context.Categories.ToList();
            var itemType = _context.ItemTypes.ToList();
            ViewBag.Categories = categories.Select(w => new SelectListItem { Text = w.Name, Value = w.Id.ToString() });
            ViewBag.ItemTypes = itemType.Select(w => new SelectListItem { Text = w.Name, Value = w.Id.ToString() });

            return View(item);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _context.Items.Include(w=>w.Category).Include(w=>w.ItemType).FirstOrDefault(w=>w.Id ==id);
            if (item == null)
            {
                return NotFound();
            }

            var categories = _context.Categories.ToList();
            var itemTypes = _context.ItemTypes.ToList();
            ViewBag.Categories = categories.Select(w => new SelectListItem { Text = w.Name, Value = w.Id.ToString() });
            ViewBag.ItemTypes = itemTypes.Select(w => new SelectListItem { Text = w.Name, Value = w.Id.ToString() });

            return View(item);
        }
        [HttpPost]
        public IActionResult Edit(int id, Item item, IFormFile ImageFile)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var existingItem = _context.Items.AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (existingItem == null)
                {
                    return NotFound();
                }

                // Handle image upload
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }

                    // Optional: delete old image file if you want

                    item.ImageUrl = "/images/" + uniqueFileName;
                }
                else
                {
                    // Preserve existing image if no new image uploaded
                    item.ImageUrl = existingItem.ImageUrl;
                }

                _context.Update(item);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Reload ViewBags in case of error
            var categories = _context.Categories.ToList();
            var itemTypes = _context.ItemTypes.ToList();
            ViewBag.Categories = categories.Select(w => new SelectListItem { Text = w.Name, Value = w.Id.ToString() });
            ViewBag.ItemTypes = itemTypes.Select(w => new SelectListItem { Text = w.Name, Value = w.Id.ToString() });

            return View(item);
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var item = _context.Items
                .Include(i => i.Category)
                .Include(i => i.ItemType)
                .FirstOrDefault(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var item = _context.Items.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            // Remove related CartItems first
            var relatedCartItems = _context.CartItems.Where(c => c.ItemId == id).ToList();
            _context.CartItems.RemoveRange(relatedCartItems);

            // Optionally delete image
            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Items.Remove(item);
            _context.SaveChanges();

            return RedirectToAction(nameof(ItemList));
        }



        [HttpGet]
        public IActionResult UserList()
        {
            var userList = _context.Users.ToList();
            return View(userList); 
        }

        [HttpGet]
        public IActionResult UserEdit(int id)
        {
            var user = _context.Users.Include(u => u.Roles).FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new SignUpModel
            {
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password = user.Password, // Optionally leave blank if not editable
                Roles = user.Roles.FirstOrDefault()?.Name!// If single role, else use multi-select logic
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult UserEdit(int id, SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Include(u => u.Roles).FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Name = model.Name;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                // If you want to allow updating the password
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    user.Password = model.Password; // You can hash if needed
                }

                // Update role - assuming single role
                user.Roles.Clear();
                var role = _context.Roles.FirstOrDefault(r => r.Name == model.Roles);
                if (role != null)
                {
                    user.Roles.Add(role);
                }

                _context.SaveChanges();

                return RedirectToAction("UserList");
            }

            return View(model);
        }



    }
}
