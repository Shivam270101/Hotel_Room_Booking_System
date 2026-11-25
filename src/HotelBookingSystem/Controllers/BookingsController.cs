using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModels;

namespace HotelBookingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<BookingsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: /Bookings/Create?roomId=1&checkIn=...&checkOut=...
        public async Task<IActionResult> Create(int roomId, DateTime? checkIn, DateTime? checkOut)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null || !room.IsActive)
            {
                return NotFound();
            }

            var viewModel = new BookingViewModel
            {
                RoomId = roomId,
                Room = room,
                CheckInDate = checkIn ?? DateTime.Today.AddDays(1),
                CheckOutDate = checkOut ?? DateTime.Today.AddDays(2),
                NumberOfGuests = 1
            };

            // Pre-fill user info if logged in
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    viewModel.GuestFirstName = user.FirstName;
                    viewModel.GuestLastName = user.LastName;
                    viewModel.GuestEmail = user.Email ?? string.Empty;
                    viewModel.GuestPhone = user.PhoneNumber ?? string.Empty;
                }
            }

            return View(viewModel);
        }

        // POST: /Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel viewModel)
        {
            var room = await _context.Rooms.FindAsync(viewModel.RoomId);
            if (room == null || !room.IsActive)
            {
                return NotFound();
            }

            viewModel.Room = room;

            // Validate dates
            if (viewModel.CheckInDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("CheckInDate", "Check-in date cannot be in the past.");
            }

            if (viewModel.CheckOutDate.Date <= viewModel.CheckInDate.Date)
            {
                ModelState.AddModelError("CheckOutDate", "Check-out date must be after check-in date.");
            }

            var nights = (viewModel.CheckOutDate.Date - viewModel.CheckInDate.Date).Days;
            if (nights > 30)
            {
                ModelState.AddModelError("CheckOutDate", "Maximum stay is 30 nights.");
            }

            if (viewModel.NumberOfGuests > room.MaxOccupancy)
            {
                ModelState.AddModelError("NumberOfGuests",
                    $"This room accommodates a maximum of {room.MaxOccupancy} guests.");
            }

            // Check availability (no overlapping bookings)
            var hasConflict = await _context.Bookings
                .AnyAsync(b => b.RoomId == viewModel.RoomId
                    && b.Status != BookingStatus.Cancelled
                    && b.Status != BookingStatus.CheckedOut
                    && b.CheckInDate < viewModel.CheckOutDate.Date
                    && b.CheckOutDate > viewModel.CheckInDate.Date);

            if (hasConflict)
            {
                ModelState.AddModelError("", "This room is not available for the selected dates. Please choose different dates.");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Create booking
            var booking = new Booking
            {
                BookingReference = Booking.GenerateReference(),
                RoomId = viewModel.RoomId,
                GuestFirstName = viewModel.GuestFirstName,
                GuestLastName = viewModel.GuestLastName,
                GuestEmail = viewModel.GuestEmail,
                GuestPhone = viewModel.GuestPhone,
                CheckInDate = viewModel.CheckInDate.Date,
                CheckOutDate = viewModel.CheckOutDate.Date,
                NumberOfGuests = viewModel.NumberOfGuests,
                NumberOfNights = nights,
                PricePerNight = room.PricePerNight,
                TotalPrice = nights * room.PricePerNight,
                Status = BookingStatus.Confirmed,
                BookingDate = DateTime.Now,
                SpecialRequests = viewModel.SpecialRequests
            };

            // Link to user if logged in
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    booking.UserId = user.Id;
                }
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Booking created: {Reference} for Room {RoomNumber}",
                booking.BookingReference, room.RoomNumber);

            return RedirectToAction(nameof(Confirmation), new { id = booking.Id });
        }

        // GET: /Bookings/Confirmation/5
        public async Task<IActionResult> Confirmation(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Only allow viewing own booking (or if admin)
            if (User.Identity?.IsAuthenticated == true && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && booking.UserId != null && booking.UserId != user.Id)
                {
                    return Forbid();
                }
            }

            var viewModel = new BookingConfirmationViewModel
            {
                Booking = booking,
                Room = booking.Room!
            };

            return View(viewModel);
        }

        // GET: /Bookings/MyBookings
        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // POST: /Bookings/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            // Verify ownership or admin
            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && (user == null || booking.UserId != user.Id))
            {
                return Forbid();
            }

            if (!booking.CanCancel)
            {
                TempData["Error"] = "This booking cannot be cancelled.";
                return RedirectToAction(nameof(MyBookings));
            }

            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Booking {booking.BookingReference} has been cancelled.";

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Bookings", "Admin");
            }

            return RedirectToAction(nameof(MyBookings));
        }

        // GET: /Bookings/Lookup
        public IActionResult Lookup()
        {
            return View();
        }

        // POST: /Bookings/Lookup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lookup(string reference, string email)
        {
            if (string.IsNullOrWhiteSpace(reference) || string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("", "Please enter both booking reference and email.");
                return View();
            }

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b =>
                    b.BookingReference == reference.Trim().ToUpper()
                    && b.GuestEmail.ToLower() == email.Trim().ToLower());

            if (booking == null)
            {
                ModelState.AddModelError("", "No booking found with the provided reference and email.");
                return View();
            }

            return RedirectToAction(nameof(Confirmation), new { id = booking.Id });
        }
    }
}
