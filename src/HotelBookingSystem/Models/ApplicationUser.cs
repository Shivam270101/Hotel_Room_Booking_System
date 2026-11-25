using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HotelBookingSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(500)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        // Computed property
        public string FullName => $"{FirstName} {LastName}";
    }
}
