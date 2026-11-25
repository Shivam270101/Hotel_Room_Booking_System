using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using HotelBookingSystem.ViewModels;

namespace HotelBookingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            var allBookings = await _context.Bookings.Include(b => b.Room).ToListAsync();
            var allRooms = await _context.Rooms.ToListAsync();

            var viewModel = new DashboardViewModel
            {
                TotalRooms = allRooms.Count,
                ActiveRooms = allRooms.Count(r => r.IsActive),
                TotalBookings = allBookings.Count,
                TodayCheckIns = allBookings.Count(b =>
                    b.CheckInDate.Date == today && b.Status == BookingStatus.Confirmed),
                TodayCheckOuts = allBookings.Count(b =>
                    b.CheckOutDate.Date == today && b.Status == BookingStatus.CheckedIn),
                CurrentlyOccupied = allBookings.Count(b => b.Status == BookingStatus.CheckedIn),

                TotalRevenue = allBookings
                    .Where(b => b.Status != BookingStatus.Cancelled)
                    .Sum(b => b.TotalPrice),
                MonthlyRevenue = allBookings
                    .Where(b => b.Status != BookingStatus.Cancelled
                        && b.BookingDate >= firstDayOfMonth)
                    .Sum(b => b.TotalPrice),

                PendingBookings = allBookings.Count(b => b.Status == BookingStatus.Pending),
                ConfirmedBookings = allBookings.Count(b => b.Status == BookingStatus.Confirmed),
                CheckedInBookings = allBookings.Count(b => b.Status == BookingStatus.CheckedIn),
                CancelledBookings = allBookings.Count(b => b.Status == BookingStatus.Cancelled),

                RecentBookings = allBookings
                    .OrderByDescending(b => b.BookingDate)
                    .Take(10)
                    .ToList(),

                UpcomingCheckIns = allBookings
                    .Where(b => b.CheckInDate.Date >= today
                        && (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending))
                    .OrderBy(b => b.CheckInDate)
                    .Take(5)
                    .ToList()
            };

            // Calculate occupancy rate
            if (viewModel.ActiveRooms > 0)
            {
                viewModel.OccupancyRate = Math.Round(
                    (double)viewModel.CurrentlyOccupied / viewModel.ActiveRooms * 100, 1);
            }

            return View(viewModel);
        }

        // GET: /Admin/Rooms
        public async Task<IActionResult> Rooms(string? search, RoomType? type, bool? active)
        {
            var query = _context.Rooms.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r =>
                    r.RoomNumber.Contains(search) ||
                    r.Name.Contains(search) ||
                    r.Description.Contains(search));
            }

            if (type.HasValue)
            {
                query = query.Where(r => r.Type == type.Value);
            }

            if (active.HasValue)
            {
                query = query.Where(r => r.IsActive == active.Value);
            }

            var rooms = await query.OrderBy(r => r.RoomNumber).ToListAsync();

            var viewModel = new AdminRoomViewModel
            {
                Rooms = rooms,
                SearchQuery = search,
                FilterType = type,
                FilterActive = active
            };

            return View(viewModel);
        }

        // GET: /Admin/CreateRoom
        public IActionResult CreateRoom()
        {
            return View(new Room { IsActive = true, FloorNumber = 1, MaxOccupancy = 2, SizeInSqFt = 300 });
        }

        // POST: /Admin/CreateRoom
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoom(Room room)
        {
            // Check for duplicate room number
            if (await _context.Rooms.AnyAsync(r => r.RoomNumber == room.RoomNumber))
            {
                ModelState.AddModelError("RoomNumber", "A room with this number already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Room created: {RoomNumber} - {Name}", room.RoomNumber, room.Name);
                TempData["Success"] = $"Room {room.RoomNumber} has been created successfully.";
                return RedirectToAction(nameof(Rooms));
            }

            return View(room);
        }

        // GET: /Admin/EditRoom/5
        public async Task<IActionResult> EditRoom(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            return View(room);
        }

        // POST: /Admin/EditRoom/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoom(int id, Room room)
        {
            if (id != room.Id) return NotFound();

            // Check for duplicate room number (excluding current room)
            if (await _context.Rooms.AnyAsync(r => r.RoomNumber == room.RoomNumber && r.Id != id))
            {
                ModelState.AddModelError("RoomNumber", "A room with this number already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Room updated: {RoomNumber} - {Name}", room.RoomNumber, room.Name);
                    TempData["Success"] = $"Room {room.RoomNumber} has been updated successfully.";
                    return RedirectToAction(nameof(Rooms));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Rooms.AnyAsync(r => r.Id == id))
                        return NotFound();
                    throw;
                }
            }

            return View(room);
        }

        // POST: /Admin/DeleteRoom/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null) return NotFound();

            // Check for active bookings
            var activeBookings = room.Bookings.Count(b =>
                b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.CheckedOut);

            if (activeBookings > 0)
            {
                TempData["Error"] = $"Cannot delete room {room.RoomNumber} - it has {activeBookings} active booking(s). Deactivate it instead.";
                return RedirectToAction(nameof(Rooms));
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Room deleted: {RoomNumber}", room.RoomNumber);
            TempData["Success"] = $"Room {room.RoomNumber} has been deleted.";
            return RedirectToAction(nameof(Rooms));
        }

        // POST: /Admin/ToggleRoom/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            room.IsActive = !room.IsActive;
            await _context.SaveChangesAsync();

            var status = room.IsActive ? "activated" : "deactivated";
            TempData["Success"] = $"Room {room.RoomNumber} has been {status}.";
            return RedirectToAction(nameof(Rooms));
        }

        // GET: /Admin/Bookings
        public async Task<IActionResult> Bookings(
            string? search,
            BookingStatus? status,
            DateTime? date,
            int page = 1)
        {
            var pageSize = 15;
            var query = _context.Bookings.Include(b => b.Room).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b =>
                    b.BookingReference.Contains(search) ||
                    b.GuestFirstName.Contains(search) ||
                    b.GuestLastName.Contains(search) ||
                    b.GuestEmail.Contains(search));
            }

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            if (date.HasValue)
            {
                query = query.Where(b =>
                    b.CheckInDate.Date == date.Value.Date ||
                    b.CheckOutDate.Date == date.Value.Date);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var bookings = await query
                .OrderByDescending(b => b.BookingDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new AdminBookingListViewModel
            {
                Bookings = bookings,
                SearchQuery = search,
                FilterStatus = status,
                FilterDate = date,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        // GET: /Admin/BookingDetails/5
        public async Task<IActionResult> BookingDetails(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: /Admin/UpdateBookingStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBookingStatus(int id, BookingStatus newStatus, string? adminNotes)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            var oldStatus = booking.Status;
            booking.Status = newStatus;

            if (!string.IsNullOrWhiteSpace(adminNotes))
            {
                booking.AdminNotes = adminNotes;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Booking {Reference} status changed from {OldStatus} to {NewStatus}",
                booking.BookingReference, oldStatus, newStatus);

            TempData["Success"] = $"Booking {booking.BookingReference} status updated to {newStatus}.";
            return RedirectToAction(nameof(BookingDetails), new { id });
        }
    }
}
