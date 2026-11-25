using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure database is created
        context.Database.Migrate();

        // Seed roles
        string[] roles = { "Admin", "Guest" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed admin user
        var adminEmail = "admin@grandhorizon.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Hotel",
                LastName = "Admin",
                EmailConfirmed = true,
                PhoneNumber = "555-0100",
                RegistrationDate = DateTime.Now
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed a guest user
        var guestEmail = "guest@example.com";
        if (await userManager.FindByEmailAsync(guestEmail) == null)
        {
            var guestUser = new ApplicationUser
            {
                UserName = guestEmail,
                Email = guestEmail,
                FirstName = "John",
                LastName = "Smith",
                EmailConfirmed = true,
                PhoneNumber = "555-0200",
                RegistrationDate = DateTime.Now
            };
            var result = await userManager.CreateAsync(guestUser, "Guest@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(guestUser, "Guest");
            }
        }

        // Seed rooms
        if (!context.Rooms.Any())
        {
            var rooms = new List<Room>
            {
                new Room
                {
                    RoomNumber = "101",
                    Name = "Comfort Standard",
                    Type = RoomType.Standard,
                    BedType = BedType.Queen,
                    Description = "A cozy standard room with modern amenities, perfect for solo travelers or couples. Features a comfortable queen bed, work desk, and city views.",
                    PricePerNight = 99.99m,
                    MaxOccupancy = 2,
                    FloorNumber = 1,
                    SizeInSqFt = 280,
                    IsActive = true,
                    ImageUrl = "/images/rooms/standard-1.jpg",
                    Amenities = "Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe"
                },
                new Room
                {
                    RoomNumber = "102",
                    Name = "Classic Standard",
                    Type = RoomType.Standard,
                    BedType = BedType.Twin,
                    Description = "A well-appointed standard room with twin beds, ideal for friends or colleagues traveling together. Includes all essential amenities.",
                    PricePerNight = 89.99m,
                    MaxOccupancy = 2,
                    FloorNumber = 1,
                    SizeInSqFt = 270,
                    IsActive = true,
                    ImageUrl = "/images/rooms/standard-2.jpg",
                    Amenities = "Wi-Fi,TV,Air Conditioning,Room Service,Safe"
                },
                new Room
                {
                    RoomNumber = "201",
                    Name = "Deluxe Garden View",
                    Type = RoomType.Deluxe,
                    BedType = BedType.King,
                    Description = "Spacious deluxe room with a luxurious king bed and beautiful garden views. Features premium furnishings, a sitting area, and marble bathroom.",
                    PricePerNight = 179.99m,
                    MaxOccupancy = 2,
                    FloorNumber = 2,
                    SizeInSqFt = 400,
                    IsActive = true,
                    ImageUrl = "/images/rooms/deluxe-1.jpg",
                    Amenities = "Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe,Bathrobe,Coffee Machine,Garden View"
                },
                new Room
                {
                    RoomNumber = "202",
                    Name = "Deluxe City View",
                    Type = RoomType.Deluxe,
                    BedType = BedType.King,
                    Description = "Elegantly designed deluxe room overlooking the city skyline. Enjoy premium comfort with a king-sized bed and upgraded amenities.",
                    PricePerNight = 199.99m,
                    MaxOccupancy = 2,
                    FloorNumber = 2,
                    SizeInSqFt = 420,
                    IsActive = true,
                    ImageUrl = "/images/rooms/deluxe-2.jpg",
                    Amenities = "Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe,Bathrobe,Coffee Machine,City View,Balcony"
                },
                new Room
                {
                    RoomNumber = "301",
                    Name = "Executive Suite",
                    Type = RoomType.Suite,
                    BedType = BedType.King,
                    Description = "Luxurious executive suite featuring a separate living area, dining space, and bedroom with king bed. Perfect for extended stays or business travelers.",
                    PricePerNight = 299.99m,
                    MaxOccupancy = 3,
                    FloorNumber = 3,
                    SizeInSqFt = 650,
                    IsActive = true,
                    ImageUrl = "/images/rooms/suite-1.jpg",
                    Amenities = "Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe,Bathrobe,Coffee Machine,City View,Living Room,Work Desk,Jacuzzi"
                },
                new Room
                {
                    RoomNumber = "302",
                    Name = "Honeymoon Suite",
                    Type = RoomType.Suite,
                    BedType = BedType.King,
                    Description = "Romantic suite designed for couples, featuring a four-poster king bed, private balcony, and in-room jacuzzi with panoramic views.",
                    PricePerNight = 349.99m,
                    MaxOccupancy = 2,
                    FloorNumber = 3,
                    SizeInSqFt = 580,
                    IsActive = true,
                    ImageUrl = "/images/rooms/suite-2.jpg",
                    Amenities = "Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe,Bathrobe,Coffee Machine,Panoramic View,Balcony,Jacuzzi,Champagne"
                },
                new Room
                {
                    RoomNumber = "401",
                    Name = "Family Room",
                    Type = RoomType.Family,
                    BedType = BedType.Double,
                    Description = "Spacious family room with connecting beds, designed for families with children. Includes a play area and child-friendly amenities.",
                    PricePerNight = 249.99m,
                    MaxOccupancy = 5,
                    FloorNumber = 4,
                    SizeInSqFt = 550,
                    IsActive = true,
                    ImageUrl = "/images/rooms/family-1.jpg",
                    Amenities = "Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe,Kids Area,Extra Beds,Garden View"
                },
                new Room
                {
                    RoomNumber = "402",
                    Name = "Family Deluxe",
                    Type = RoomType.Family,
                    BedType = BedType.Queen,
                    Description = "Premium family accommodation with separate sleeping areas for parents and children, plus a kitchenette for added convenience.",
                    PricePerNight = 279.99m,
                    MaxOccupancy = 6,
                    FloorNumber = 4,
                    SizeInSqFt = 620,
                    IsActive = true,
                    ImageUrl = "/images/rooms/family-2.jpg",
                    Amenities = "Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe,Kids Area,Extra Beds,Kitchenette,Balcony"
                },
                new Room
                {
                    RoomNumber = "501",
                    Name = "Presidential Suite",
                    Type = RoomType.Presidential,
                    BedType = BedType.King,
                    Description = "The crown jewel of Grand Horizon Hotel. This lavish presidential suite spans the entire floor with panoramic views, private dining room, butler service, and the finest luxury amenities.",
                    PricePerNight = 599.99m,
                    MaxOccupancy = 4,
                    FloorNumber = 5,
                    SizeInSqFt = 1200,
                    IsActive = true,
                    ImageUrl = "/images/rooms/presidential-1.jpg",
                    Amenities = "Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe,Bathrobe,Coffee Machine,Panoramic View,Living Room,Dining Room,Jacuzzi,Butler Service,Private Terrace,Grand Piano"
                },
                new Room
                {
                    RoomNumber = "103",
                    Name = "Economy Single",
                    Type = RoomType.Standard,
                    BedType = BedType.Single,
                    Description = "Budget-friendly single room perfect for business travelers. Clean, comfortable, and equipped with all essentials.",
                    PricePerNight = 69.99m,
                    MaxOccupancy = 1,
                    FloorNumber = 1,
                    SizeInSqFt = 200,
                    IsActive = true,
                    ImageUrl = "/images/rooms/standard-3.jpg",
                    Amenities = "Wi-Fi,TV,Air Conditioning,Work Desk,Safe"
                }
            };

            context.Rooms.AddRange(rooms);
            await context.SaveChangesAsync();

            // Seed sample bookings
            var guestUser = await userManager.FindByEmailAsync(guestEmail);
            var bookings = new List<Booking>
            {
                new Booking
                {
                    BookingReference = Booking.GenerateReference(),
                    RoomId = rooms[0].Id,
                    UserId = guestUser?.Id,
                    GuestFirstName = "John",
                    GuestLastName = "Smith",
                    GuestEmail = "john.smith@example.com",
                    GuestPhone = "555-0201",
                    CheckInDate = DateTime.Today.AddDays(5),
                    CheckOutDate = DateTime.Today.AddDays(8),
                    NumberOfGuests = 2,
                    NumberOfNights = 3,
                    PricePerNight = rooms[0].PricePerNight,
                    TotalPrice = rooms[0].PricePerNight * 3,
                    Status = BookingStatus.Confirmed,
                    BookingDate = DateTime.Now.AddDays(-2),
                    SpecialRequests = "Late check-in please, arriving around 10 PM"
                },
                new Booking
                {
                    BookingReference = Booking.GenerateReference(),
                    RoomId = rooms[2].Id,
                    GuestFirstName = "Sarah",
                    GuestLastName = "Johnson",
                    GuestEmail = "sarah.j@example.com",
                    GuestPhone = "555-0202",
                    CheckInDate = DateTime.Today.AddDays(1),
                    CheckOutDate = DateTime.Today.AddDays(4),
                    NumberOfGuests = 2,
                    NumberOfNights = 3,
                    PricePerNight = rooms[2].PricePerNight,
                    TotalPrice = rooms[2].PricePerNight * 3,
                    Status = BookingStatus.Confirmed,
                    BookingDate = DateTime.Now.AddDays(-5)
                },
                new Booking
                {
                    BookingReference = Booking.GenerateReference(),
                    RoomId = rooms[4].Id,
                    GuestFirstName = "Michael",
                    GuestLastName = "Chen",
                    GuestEmail = "m.chen@example.com",
                    GuestPhone = "555-0203",
                    CheckInDate = DateTime.Today,
                    CheckOutDate = DateTime.Today.AddDays(2),
                    NumberOfGuests = 2,
                    NumberOfNights = 2,
                    PricePerNight = rooms[4].PricePerNight,
                    TotalPrice = rooms[4].PricePerNight * 2,
                    Status = BookingStatus.CheckedIn,
                    BookingDate = DateTime.Now.AddDays(-7)
                },
                new Booking
                {
                    BookingReference = Booking.GenerateReference(),
                    RoomId = rooms[6].Id,
                    GuestFirstName = "Emily",
                    GuestLastName = "Davis",
                    GuestEmail = "emily.d@example.com",
                    GuestPhone = "555-0204",
                    CheckInDate = DateTime.Today.AddDays(-3),
                    CheckOutDate = DateTime.Today.AddDays(-1),
                    NumberOfGuests = 4,
                    NumberOfNights = 2,
                    PricePerNight = rooms[6].PricePerNight,
                    TotalPrice = rooms[6].PricePerNight * 2,
                    Status = BookingStatus.CheckedOut,
                    BookingDate = DateTime.Now.AddDays(-10)
                },
                new Booking
                {
                    BookingReference = Booking.GenerateReference(),
                    RoomId = rooms[8].Id,
                    GuestFirstName = "Robert",
                    GuestLastName = "Wilson",
                    GuestEmail = "r.wilson@example.com",
                    GuestPhone = "555-0205",
                    CheckInDate = DateTime.Today.AddDays(10),
                    CheckOutDate = DateTime.Today.AddDays(14),
                    NumberOfGuests = 2,
                    NumberOfNights = 4,
                    PricePerNight = rooms[8].PricePerNight,
                    TotalPrice = rooms[8].PricePerNight * 4,
                    Status = BookingStatus.Pending,
                    BookingDate = DateTime.Now.AddDays(-1),
                    SpecialRequests = "Anniversary celebration - champagne and flowers in room"
                }
            };

            context.Bookings.AddRange(bookings);
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
