using ePizzaHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ePizzaHub.Web.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : BaseController
    {
        IMemoryCache _cache;
        private IItemService _itemService;
        IPaymentService _paymentService;
        private readonly ILogger<HomeController> _logger;
        public HomeController(IMemoryCache cache, IItemService itemService, ILogger<HomeController> logger, IPaymentService paymentService)
        {
            _cache = cache;
            _itemService = itemService;
            _logger = logger;
            _paymentService = paymentService;
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

    }
}
