# Hotel Room Booking System

A full-featured Hotel Room Booking System built with **ASP.NET Core 8.0 MVC**, **C#**, and **SQL Server**. This application allows guests to search and book hotel rooms online, while providing hotel staff with comprehensive management tools.

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-purple)
![C#](https://img.shields.io/badge/C%23-12-green)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-blue)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-blueviolet)

---

## Table of Contents

- [Features](#features)
- [Design Decisions](#design-decisions)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Setup & Installation](#setup--installation)
- [Database](#database)
- [Demo Accounts](#demo-accounts)
- [Screenshots](#screenshots)
- [Business Rules](#business-rules)
- [Future Enhancements](#future-enhancements)

---

## Features

### Guest Features
- **Room Search & Filtering**: Search available rooms by check-in/check-out dates, room type, number of guests, and price range
- **Room Details**: View detailed room information including amenities, features, bed type, floor, and pricing
- **Online Booking**: Book rooms with a streamlined booking form with real-time price calculation
- **Booking Confirmation**: Receive a unique booking reference with full reservation details
- **Booking Lookup**: Find existing bookings using reference number and email
- **My Bookings**: Registered users can view and manage all their bookings
- **Booking Cancellation**: Cancel pending or confirmed bookings

### Admin / Staff Features
- **Dashboard**: Real-time overview with key metrics — total rooms, bookings, occupancy rate, revenue, today's check-ins/check-outs
- **Room Management**: Full CRUD operations for rooms — add, edit, activate/deactivate, delete
- **Booking Management**: View all bookings with search, filter by status/date, pagination
- **Booking Details**: Detailed view of any booking with ability to update status (Pending → Confirmed → Checked In → Checked Out) and add admin notes
- **Status Workflow**: Manage the full booking lifecycle

### General Features
- **Responsive Design**: Clean, modern UI that works on desktop, tablet, and mobile
- **Authentication**: Role-based access with ASP.NET Core Identity (Admin & Guest roles)
- **Data Validation**: Server-side and client-side validation on all forms
- **Toast Notifications**: Success/error feedback for user actions
- **Seed Data**: Pre-populated with 10 sample rooms and 5 sample bookings

---

## Design Decisions

### Features Chosen and Why

1. **Role-Based Access (Admin + Guest)**: A hotel needs clear separation between staff operations and guest interactions. Admins manage rooms and bookings; guests search and book. I chose not to add more granular roles (receptionist, manager) to keep it focused within the time constraint.

2. **Guest Booking Without Mandatory Registration**: Many hotel booking sites let you book without creating an account. I implemented this to reduce friction — guests can book immediately, but optionally register to track their bookings. The booking form pre-fills data for logged-in users.

3. **Room Availability by Date Range**: The core booking logic checks for overlapping date ranges against existing active bookings (not cancelled/checked-out). This prevents double-booking, which is the most critical business rule.

4. **Booking Reference System**: Each booking gets a unique reference (e.g., `HB-20261125-A3F2B1`) for easy lookup. Guests can find their booking using just the reference + email, even without an account.

5. **Admin Dashboard with Metrics**: Staff need a quick overview. The dashboard shows today's check-ins/check-outs, occupancy rate, revenue, booking status breakdown, and recent/upcoming bookings — all calculated from real data.

6. **Booking Status Workflow**: I implemented a 5-state workflow (Pending → Confirmed → Checked In → Checked Out, or Cancelled) to model real hotel operations. Status transitions are validated (e.g., can't check in before the check-in date).

7. **Room Amenities as Comma-Separated String**: Rather than a separate many-to-many table, I stored amenities as a comma-separated string with a helper property to split them. This simplifies the schema while still being displayable and filterable. For a production system, a proper junction table would be better.

8. **Bootstrap 5 with Custom Gold Theme**: I chose Bootstrap for rapid responsive development and created a custom "luxury hotel" aesthetic with gold accent colors, Playfair Display headings, and smooth animations.

### Business Rules Applied

- Check-in date must be today or in the future
- Check-out date must be after check-in date
- Maximum stay is 30 nights
- Number of guests cannot exceed room's maximum occupancy
- No overlapping bookings for the same room (double-booking prevention)
- Rooms with active bookings cannot be deleted (only deactivated)
- Room numbers must be unique
- Each booking gets a unique auto-generated reference
- Only Pending and Confirmed bookings can be cancelled
- Free cancellation policy (24 hours before check-in)
- Check-in time: 2:00 PM, Check-out time: 11:00 AM
- Payment is at the hotel (no online payment in this version)

### What I Would Add With More Time

1. **Online Payment Integration**: Stripe or PayPal integration for online payments
2. **Email Notifications**: Booking confirmation, reminder, and cancellation emails
3. **Room Image Uploads**: File upload system for actual room photos
4. **Reviews & Ratings**: Guest reviews and star ratings for rooms
5. **Pricing Engine**: Seasonal pricing, weekend surcharges, early-bird discounts
6. **Calendar View**: Visual calendar for admin to see room availability at a glance
7. **Reports & Analytics**: Revenue reports, occupancy trends, booking source analytics
8. **Multi-language Support**: Internationalization for global guests
9. **API Endpoints**: RESTful API for mobile app or third-party integration
10. **Unit & Integration Tests**: xUnit tests for business logic and controllers
11. **Audit Trail**: Log all changes to bookings and rooms with timestamps and user info
12. **Pagination & Caching**: Performance optimization for large datasets

---

## Technology Stack

| Component | Technology |
|-----------|-----------|
| **Framework** | ASP.NET Core 8.0 MVC |
| **Language** | C# 12 |
| **Database** | SQL Server (LocalDB) |
| **ORM** | Entity Framework Core 8.0 |
| **Authentication** | ASP.NET Core Identity |
| **UI Framework** | Bootstrap 5.3 |
| **Icons** | Font Awesome 6.5 |
| **Fonts** | Google Fonts (Playfair Display, Inter) |

---

## Project Structure

```
HotelBookingSystem/
├── src/
│   ├── HotelBookingSystem.sln
│   └── HotelBookingSystem/
│       ├── Controllers/
│       │   ├── HomeController.cs         # Home page, featured rooms
│       │   ├── RoomsController.cs        # Room search, listing, details
│       │   ├── BookingsController.cs     # Booking creation, confirmation, lookup
│       │   ├── AdminController.cs        # Dashboard, room/booking management
│       │   └── AccountController.cs      # Login, register, logout
│       ├── Models/
│       │   ├── Room.cs                   # Room entity with enums
│       │   ├── Booking.cs               # Booking entity with status workflow
│       │   ├── ApplicationUser.cs       # Extended Identity user
│       │   └── ErrorViewModel.cs
│       ├── Data/
│       │   └── ApplicationDbContext.cs  # EF Core DbContext with configurations
│       ├── ViewModels/
│       │   ├── RoomSearchViewModel.cs   # Room search & availability
│       │   ├── BookingViewModel.cs      # Booking form & confirmation
│       │   ├── DashboardViewModel.cs    # Admin dashboard data
│       │   └── AccountViewModels.cs     # Login & register forms
│       ├── Views/
│       │   ├── Shared/                  # Layout, partials, error page
│       │   ├── Home/                    # Landing page
│       │   ├── Rooms/                   # Room listing & details
│       │   ├── Bookings/               # Booking flow & management
│       │   ├── Admin/                   # Admin dashboard & CRUD
│       │   └── Account/                # Authentication views
│       ├── wwwroot/
│       │   ├── css/site.css            # Custom styles (gold theme)
│       │   └── js/site.js             # Client-side interactions
│       ├── Program.cs                   # App configuration & DB seeding
│       └── appsettings.json            # Connection string & settings
├── database/
│   ├── HotelBookingDB.sql              # Full schema + sample data
│   └── ER-Diagram.png                  # Entity relationship diagram
├── screenshots/                        # Application screenshots
├── AI_TOOLS_USED.md
└── README.md
```

---

## Setup & Installation

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/) (LocalDB, Express, or full edition)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended) or VS Code

### Steps

1. **Clone or extract the project**
   ```bash
   cd HotelBookingSystem/src
   ```

2. **Update the connection string** (if needed)
   
   Edit `src/HotelBookingSystem/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelBookingDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
     }
   }
   ```

3. **Restore packages and run**
   ```bash
   cd HotelBookingSystem
   dotnet restore
   dotnet run
   ```
   
   Or open `HotelBookingSystem.sln` in Visual Studio and press **F5**.

4. **Database is auto-created**
   
   The application uses Entity Framework Core migrations. On first run, it will:
   - Create the database automatically
   - Apply the schema
   - Seed roles (Admin, Guest)
   - Seed admin and guest user accounts
   - Seed 10 sample rooms and 5 sample bookings

5. **Alternatively, use the SQL script**
   
   Run `database/HotelBookingDB.sql` in SQL Server Management Studio to create the database with schema and sample data manually.

---

## Database

### ER Diagram

The database consists of the following key tables:

- **AspNetUsers** — Extended ASP.NET Identity users (FirstName, LastName, etc.)
- **AspNetRoles** — User roles (Admin, Guest)
- **Rooms** — Hotel rooms with type, pricing, amenities, capacity
- **Bookings** — Reservations linking guests to rooms with dates and status

### Key Relationships

- A **Room** can have many **Bookings** (one-to-many)
- A **User** can have many **Bookings** (one-to-many, optional — guest bookings without accounts have NULL UserId)
- **Bookings** reference a **Room** (required) and optionally a **User**

See `database/ER-Diagram.png` for the visual diagram.

---

## Demo Accounts

| Role | Email | Password |
|------|-------|----------|
| **Admin** | admin@grandhorizon.com | Admin@123 |
| **Guest** | guest@example.com | Guest@123 |

---

## Screenshots

Screenshots of all major screens are available in the `screenshots/` directory:

- Home page (desktop & mobile)
- Room listing with search
- Room details page
- Booking form
- Booking confirmation
- My Bookings page
- Booking lookup
- Admin dashboard
- Admin room management
- Admin booking management
- Login & Register pages

---

## Business Rules

| Rule | Description |
|------|-------------|
| **Date Validation** | Check-in ≥ today; Check-out > Check-in |
| **Max Stay** | Maximum 30 consecutive nights |
| **Occupancy Limit** | Guests ≤ room's max occupancy |
| **No Double Booking** | Overlapping date ranges on same room are blocked |
| **Booking Reference** | Auto-generated unique reference (HB-YYYYMMDD-XXXXXX) |
| **Cancellation** | Only Pending or Confirmed bookings can be cancelled |
| **Room Deletion** | Rooms with active bookings cannot be deleted (deactivate instead) |
| **Unique Room Numbers** | No duplicate room numbers allowed |

---

## Future Enhancements

- Online payment processing (Stripe/PayPal)
- Email confirmation and reminders
- Room photo upload system
- Guest reviews and ratings
- Dynamic/seasonal pricing
- Visual availability calendar
- Revenue and analytics reporting
- Mobile app with REST API
- Multi-language support
- Automated testing suite

---

## License

This project was created as a coding assessment submission. All rights reserved.
