using ePizzaHub.Services.Interfaces;
using ePizzaHub.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace ePizzaHub.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IItemService _itemService;
        IMemoryCache _cache;
        public HomeController(ILogger<HomeController> logger, IItemService itemService, IMemoryCache cache)
        {
            _logger = logger;
            _itemService = itemService;
            _cache = cache;
        }

        public IActionResult Index()
        {
            string key = "catalog";
            var items = _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpiration = DateTime.Now.AddHours(12);
                return _itemService.GetAll();
            });

            // Shuffle (random order)
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