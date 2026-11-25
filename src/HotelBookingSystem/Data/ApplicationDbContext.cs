using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Models;

namespace HotelBookingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Room configuration
            builder.Entity<Room>(entity =>
            {
                entity.HasIndex(r => r.RoomNumber).IsUnique();
                entity.Property(r => r.PricePerNight).HasColumnType("decimal(10,2)");
            });

            // Booking configuration
            builder.Entity<Booking>(entity =>
            {
                entity.HasIndex(b => b.BookingReference).IsUnique();
                entity.Property(b => b.PricePerNight).HasColumnType("decimal(10,2)");
                entity.Property(b => b.TotalPrice).HasColumnType("decimal(10,2)");

                entity.HasOne(b => b.Room)
                    .WithMany(r => r.Bookings)
                    .HasForeignKey(b => b.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.User)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
            });
        }
    }
}
