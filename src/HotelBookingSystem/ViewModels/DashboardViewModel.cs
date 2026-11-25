using HotelBookingSystem.Models;

namespace HotelBookingSystem.ViewModels
{
    public class DashboardViewModel
    {
        // Summary stats
        public int TotalRooms { get; set; }
        public int ActiveRooms { get; set; }
        public int TotalBookings { get; set; }
        public int TodayCheckIns { get; set; }
        public int TodayCheckOuts { get; set; }
        public int CurrentlyOccupied { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public double OccupancyRate { get; set; }

        // Booking status breakdown
        public int PendingBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CheckedInBookings { get; set; }
        public int CancelledBookings { get; set; }

        // Recent data
        public List<Booking> RecentBookings { get; set; } = new();
        public List<Booking> UpcomingCheckIns { get; set; } = new();
    }

    public class AdminRoomViewModel
    {
        public List<Room> Rooms { get; set; } = new();
        public string? SearchQuery { get; set; }
        public RoomType? FilterType { get; set; }
        public bool? FilterActive { get; set; }
    }

    public class AdminBookingListViewModel
    {
        public List<Booking> Bookings { get; set; } = new();
        public string? SearchQuery { get; set; }
        public BookingStatus? FilterStatus { get; set; }
        public DateTime? FilterDate { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 15;
    }
}
