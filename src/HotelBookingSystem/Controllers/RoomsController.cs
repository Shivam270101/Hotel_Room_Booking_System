using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModels;

namespace HotelBookingSystem.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(ApplicationDbContext context, ILogger<RoomsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Rooms
        public async Task<IActionResult> Index(
            DateTime? checkInDate,
            DateTime? checkOutDate,
            RoomType? roomType,
            int? numberOfGuests,
            decimal? maxPrice,
            string? sortBy)
        {
            var viewModel = new RoomSearchViewModel
            {
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                RoomType = roomType,
                NumberOfGuests = numberOfGuests,
                MaxPrice = maxPrice,
                SortBy = sortBy
            };

            var query = _context.Rooms.Where(r => r.IsActive).AsQueryable();

            // Filter by room type
            if (roomType.HasValue)
            {
                query = query.Where(r => r.Type == roomType.Value);
            }

            // Filter by max occupancy
            if (numberOfGuests.HasValue && numberOfGuests.Value > 0)
            {
                query = query.Where(r => r.MaxOccupancy >= numberOfGuests.Value);
            }

            // Filter by max price
            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                query = query.Where(r => r.PricePerNight <= maxPrice.Value);
            }

            var rooms = await query.ToListAsync();

            // Calculate availability for each room
            int? nights = null;
            if (checkInDate.HasValue && checkOutDate.HasValue && checkOutDate > checkInDate)
            {
                nights = (checkOutDate.Value - checkInDate.Value).Days;
            }

            var availableRooms = new List<RoomAvailabilityViewModel>();

            foreach (var room in rooms)
            {
                bool isAvailable = true;

                if (checkInDate.HasValue && checkOutDate.HasValue)
                {
                    // Check for overlapping bookings
                    isAvailable = !await _context.Bookings
                        .AnyAsync(b => b.RoomId == room.Id
                            && b.Status != BookingStatus.Cancelled
                            && b.Status != BookingStatus.CheckedOut
                            && b.CheckInDate < checkOutDate.Value
                            && b.CheckOutDate > checkInDate.Value);
                }

                availableRooms.Add(new RoomAvailabilityViewModel
                {
                    Room = room,
                    IsAvailable = isAvailable,
                    Nights = nights,
                    TotalPrice = nights.HasValue ? nights.Value * room.PricePerNight : null
                });
            }

            // Filter out unavailable rooms when dates are specified
            if (checkInDate.HasValue && checkOutDate.HasValue)
            {
                availableRooms = availableRooms.Where(r => r.IsAvailable).ToList();
            }

            // Sort
            availableRooms = sortBy switch
            {
                "price_asc" => availableRooms.OrderBy(r => r.Room.PricePerNight).ToList(),
                "price_desc" => availableRooms.OrderByDescending(r => r.Room.PricePerNight).ToList(),
                "name" => availableRooms.OrderBy(r => r.Room.Name).ToList(),
                "type" => availableRooms.OrderBy(r => r.Room.Type).ToList(),
                "size" => availableRooms.OrderByDescending(r => r.Room.SizeInSqFt).ToList(),
                _ => availableRooms.OrderBy(r => r.Room.PricePerNight).ToList()
            };

            viewModel.AvailableRooms = availableRooms;
            viewModel.TotalRooms = availableRooms.Count;

            return View(viewModel);
        }

        // GET: /Rooms/Details/5
        public async Task<IActionResult> Details(int? id, DateTime? checkIn, DateTime? checkOut)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);

            if (room == null)
            {
                return NotFound();
            }

            // Check availability if dates provided
            bool isAvailable = true;
            if (checkIn.HasValue && checkOut.HasValue)
            {
                isAvailable = !await _context.Bookings
                    .AnyAsync(b => b.RoomId == room.Id
                        && b.Status != BookingStatus.Cancelled
                        && b.Status != BookingStatus.CheckedOut
                        && b.CheckInDate < checkOut.Value
                        && b.CheckOutDate > checkIn.Value);
            }

            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;
            ViewBag.IsAvailable = isAvailable;
            ViewBag.Nights = checkIn.HasValue && checkOut.HasValue
                ? (checkOut.Value - checkIn.Value).Days
                : (int?)null;

            return View(room);
        }
    }
}
