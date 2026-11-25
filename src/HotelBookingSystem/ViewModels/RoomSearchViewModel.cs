using System.ComponentModel.DataAnnotations;
using HotelBookingSystem.Models;

namespace HotelBookingSystem.ViewModels
{
    public class RoomSearchViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Check-In Date")]
        public DateTime? CheckInDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Check-Out Date")]
        public DateTime? CheckOutDate { get; set; }

        [Display(Name = "Room Type")]
        public RoomType? RoomType { get; set; }

        [Display(Name = "Guests")]
        [Range(1, 10)]
        public int? NumberOfGuests { get; set; }

        [Display(Name = "Max Price Per Night")]
        [Range(0, 99999)]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "Sort By")]
        public string? SortBy { get; set; }

        // Results
        public List<RoomAvailabilityViewModel> AvailableRooms { get; set; } = new();

        public int TotalRooms { get; set; }
    }

    public class RoomAvailabilityViewModel
    {
        public Room Room { get; set; } = null!;
        public bool IsAvailable { get; set; }
        public int? Nights { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
