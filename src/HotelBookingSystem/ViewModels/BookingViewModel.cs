using System.ComponentModel.DataAnnotations;
using HotelBookingSystem.Models;

namespace HotelBookingSystem.ViewModels
{
    public class BookingViewModel
    {
        // Room info (read-only display)
        public int RoomId { get; set; }
        public Room? Room { get; set; }

        // Date range
        [Required(ErrorMessage = "Check-in date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-In Date")]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-out date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-Out Date")]
        public DateTime CheckOutDate { get; set; }

        // Guest info
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string GuestFirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string GuestLastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(256)]
        [Display(Name = "Email Address")]
        public string GuestEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string GuestPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Number of guests is required")]
        [Range(1, 10, ErrorMessage = "Number of guests must be between 1 and 10")]
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; } = 1;

        [StringLength(500)]
        [Display(Name = "Special Requests")]
        public string? SpecialRequests { get; set; }

        // Computed display fields
        public int NumberOfNights => (CheckOutDate - CheckInDate).Days;
        public decimal PricePerNight => Room?.PricePerNight ?? 0;
        public decimal TotalPrice => NumberOfNights > 0 ? NumberOfNights * PricePerNight : 0;
    }

    public class BookingConfirmationViewModel
    {
        public Booking Booking { get; set; } = null!;
        public Room Room { get; set; } = null!;
    }
}
