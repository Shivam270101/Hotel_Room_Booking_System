using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models
{
    public enum RoomType
    {
        [Display(Name = "Standard")]
        Standard = 0,

        [Display(Name = "Deluxe")]
        Deluxe = 1,

        [Display(Name = "Suite")]
        Suite = 2,

        [Display(Name = "Family")]
        Family = 3,

        [Display(Name = "Presidential Suite")]
        Presidential = 4
    }

    public enum BedType
    {
        [Display(Name = "Single")]
        Single = 0,

        [Display(Name = "Double")]
        Double = 1,

        [Display(Name = "Queen")]
        Queen = 2,

        [Display(Name = "King")]
        King = 3,

        [Display(Name = "Twin")]
        Twin = 4
    }

    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Room Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Room Type")]
        public RoomType Type { get; set; }

        [Required]
        [Display(Name = "Bed Type")]
        public BedType BedType { get; set; }

        [StringLength(1000)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Price Per Night")]
        [Range(0.01, 99999.99)]
        public decimal PricePerNight { get; set; }

        [Required]
        [Display(Name = "Max Occupancy")]
        [Range(1, 10)]
        public int MaxOccupancy { get; set; }

        [Display(Name = "Floor")]
        [Range(1, 50)]
        public int FloorNumber { get; set; }

        [Display(Name = "Room Size (sq ft)")]
        [Range(100, 5000)]
        public int SizeInSqFt { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [StringLength(1000)]
        [Display(Name = "Amenities")]
        public string? Amenities { get; set; }

        // Navigation property
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        // Helper to get amenities as a list
        [NotMapped]
        public List<string> AmenitiesList =>
            string.IsNullOrWhiteSpace(Amenities)
                ? new List<string>()
                : Amenities.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        [NotMapped]
        public string TypeDisplayName => Type switch
        {
            RoomType.Standard => "Standard",
            RoomType.Deluxe => "Deluxe",
            RoomType.Suite => "Suite",
            RoomType.Family => "Family",
            RoomType.Presidential => "Presidential Suite",
            _ => Type.ToString()
        };

        [NotMapped]
        public string BedTypeDisplayName => BedType switch
        {
            BedType.Single => "Single Bed",
            BedType.Double => "Double Bed",
            BedType.Queen => "Queen Bed",
            BedType.King => "King Bed",
            BedType.Twin => "Twin Beds",
            _ => BedType.ToString()
        };
    }
}
