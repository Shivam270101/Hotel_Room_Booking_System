using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models
{
    public enum BookingStatus
    {
        [Display(Name = "Pending")]
        Pending = 0,

        [Display(Name = "Confirmed")]
        Confirmed = 1,

        [Display(Name = "Checked In")]
        CheckedIn = 2,

        [Display(Name = "Checked Out")]
        CheckedOut = 3,

        [Display(Name = "Cancelled")]
        Cancelled = 4
    }

    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Booking Reference")]
        public string BookingReference { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Room")]
        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }

        [StringLength(450)]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string GuestFirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string GuestLastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        [Display(Name = "Email")]
        public string GuestEmail { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone")]
        public string GuestPhone { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-In Date")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-Out Date")]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Display(Name = "Number of Guests")]
        [Range(1, 10)]
        public int NumberOfGuests { get; set; }

        [Display(Name = "Number of Nights")]
        public int NumberOfNights { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Price Per Night")]
        public decimal PricePerNight { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Display(Name = "Status")]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [Required]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        [Display(Name = "Special Requests")]
        public string? SpecialRequests { get; set; }

        [StringLength(500)]
        [Display(Name = "Admin Notes")]
        public string? AdminNotes { get; set; }

        // Computed properties
        [NotMapped]
        public string GuestFullName => $"{GuestFirstName} {GuestLastName}";

        [NotMapped]
        public string StatusBadgeClass => Status switch
        {
            BookingStatus.Pending => "bg-warning text-dark",
            BookingStatus.Confirmed => "bg-success",
            BookingStatus.CheckedIn => "bg-info",
            BookingStatus.CheckedOut => "bg-secondary",
            BookingStatus.Cancelled => "bg-danger",
            _ => "bg-secondary"
        };

        [NotMapped]
        public bool CanCancel => Status == BookingStatus.Pending || Status == BookingStatus.Confirmed;

        [NotMapped]
        public bool CanCheckIn => Status == BookingStatus.Confirmed && CheckInDate.Date <= DateTime.Today;

        [NotMapped]
        public bool CanCheckOut => Status == BookingStatus.CheckedIn;

        // Generate a booking reference
        public static string GenerateReference()
        {
            var datePart = DateTime.Now.ToString("yyyyMMdd");
            var randomPart = Guid.NewGuid().ToString("N")[..6].ToUpper();
            return $"HB-{datePart}-{randomPart}";
        }
    }
}
