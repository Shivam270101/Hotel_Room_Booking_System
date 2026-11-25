using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;

namespace HotelBookingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Get featured rooms (one of each type, active only)
            var featuredRooms = await _context.Rooms
                .Where(r => r.IsActive)
                .GroupBy(r => r.Type)
                .Select(g => g.OrderByDescending(r => r.PricePerNight).First())
                .Take(6)
                .ToListAsync();

            ViewBag.FeaturedRooms = featuredRooms;
            ViewBag.TotalRooms = await _context.Rooms.CountAsync(r => r.IsActive);

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
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
