-- ============================================================
-- Hotel Booking System - Database Schema and Sample Data
-- Database: HotelBookingDB
-- SQL Server
-- ============================================================

-- Create the database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'HotelBookingDB')
BEGIN
    CREATE DATABASE HotelBookingDB;
END
GO

USE HotelBookingDB;
GO

-- ============================================================
-- Drop existing tables (in correct order for FK constraints)
-- ============================================================
IF OBJECT_ID('dbo.Bookings', 'U') IS NOT NULL DROP TABLE dbo.Bookings;
IF OBJECT_ID('dbo.AspNetUserTokens', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserTokens;
IF OBJECT_ID('dbo.AspNetUserLogins', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserLogins;
IF OBJECT_ID('dbo.AspNetUserClaims', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserClaims;
IF OBJECT_ID('dbo.AspNetUserRoles', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserRoles;
IF OBJECT_ID('dbo.AspNetRoleClaims', 'U') IS NOT NULL DROP TABLE dbo.AspNetRoleClaims;
IF OBJECT_ID('dbo.AspNetRoles', 'U') IS NOT NULL DROP TABLE dbo.AspNetRoles;
IF OBJECT_ID('dbo.AspNetUsers', 'U') IS NOT NULL DROP TABLE dbo.AspNetUsers;
IF OBJECT_ID('dbo.Rooms', 'U') IS NOT NULL DROP TABLE dbo.Rooms;
GO

-- ============================================================
-- ASP.NET Core Identity Tables
-- ============================================================

CREATE TABLE [dbo].[AspNetRoles] (
    [Id]               NVARCHAR(450) NOT NULL,
    [Name]             NVARCHAR(256) NULL,
    [NormalizedName]   NVARCHAR(256) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR(450)  NOT NULL,
    [FirstName]            NVARCHAR(100)  NOT NULL,
    [LastName]             NVARCHAR(100)  NOT NULL,
    [DateOfBirth]          DATETIME2      NULL,
    [Address]              NVARCHAR(500)  NULL,
    [RegistrationDate]     DATETIME2      NOT NULL DEFAULT GETDATE(),
    [UserName]             NVARCHAR(256)  NULL,
    [NormalizedUserName]   NVARCHAR(256)  NULL,
    [Email]                NVARCHAR(256)  NULL,
    [NormalizedEmail]      NVARCHAR(256)  NULL,
    [EmailConfirmed]       BIT            NOT NULL DEFAULT 0,
    [PasswordHash]         NVARCHAR(MAX)  NULL,
    [SecurityStamp]        NVARCHAR(MAX)  NULL,
    [ConcurrencyStamp]     NVARCHAR(MAX)  NULL,
    [PhoneNumber]          NVARCHAR(MAX)  NULL,
    [PhoneNumberConfirmed] BIT            NOT NULL DEFAULT 0,
    [TwoFactorEnabled]     BIT            NOT NULL DEFAULT 0,
    [LockoutEnd]           DATETIMEOFFSET NULL,
    [LockoutEnabled]       BIT            NOT NULL DEFAULT 1,
    [AccessFailedCount]    INT            NOT NULL DEFAULT 0,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] NVARCHAR(450) NOT NULL,
    [RoleId] NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id]         INT            IDENTITY(1,1) NOT NULL,
    [UserId]     NVARCHAR(450)  NOT NULL,
    [ClaimType]  NVARCHAR(MAX)  NULL,
    [ClaimValue] NVARCHAR(MAX)  NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider]       NVARCHAR(450) NOT NULL,
    [ProviderKey]         NVARCHAR(450) NOT NULL,
    [ProviderDisplayName] NVARCHAR(MAX) NULL,
    [UserId]              NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserTokens] (
    [UserId]        NVARCHAR(450) NOT NULL,
    [LoginProvider] NVARCHAR(450) NOT NULL,
    [Name]          NVARCHAR(450) NOT NULL,
    [Value]         NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetRoleClaims] (
    [Id]         INT            IDENTITY(1,1) NOT NULL,
    [RoleId]     NVARCHAR(450)  NOT NULL,
    [ClaimType]  NVARCHAR(MAX)  NULL,
    [ClaimValue] NVARCHAR(MAX)  NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles]([Id]) ON DELETE CASCADE
);
GO

-- ============================================================
-- Application Tables
-- ============================================================

-- Rooms Table
CREATE TABLE [dbo].[Rooms] (
    [Id]            INT             IDENTITY(1,1) NOT NULL,
    [RoomNumber]    NVARCHAR(10)    NOT NULL,
    [Name]          NVARCHAR(100)   NOT NULL,
    [Type]          INT             NOT NULL,  -- 0=Standard, 1=Deluxe, 2=Suite, 3=Family, 4=Presidential
    [BedType]       INT             NOT NULL,  -- 0=Single, 1=Double, 2=Queen, 3=King, 4=Twin
    [Description]   NVARCHAR(1000)  NOT NULL DEFAULT '',
    [PricePerNight] DECIMAL(10,2)   NOT NULL,
    [MaxOccupancy]  INT             NOT NULL DEFAULT 2,
    [FloorNumber]   INT             NOT NULL DEFAULT 1,
    [SizeInSqFt]    INT             NOT NULL DEFAULT 300,
    [IsActive]      BIT             NOT NULL DEFAULT 1,
    [ImageUrl]      NVARCHAR(500)   NULL,
    [Amenities]     NVARCHAR(1000)  NULL,
    CONSTRAINT [PK_Rooms] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [UQ_Rooms_RoomNumber] UNIQUE ([RoomNumber])
);
GO

-- Bookings Table
CREATE TABLE [dbo].[Bookings] (
    [Id]               INT             IDENTITY(1,1) NOT NULL,
    [BookingReference] NVARCHAR(20)    NOT NULL,
    [RoomId]           INT             NOT NULL,
    [UserId]           NVARCHAR(450)   NULL,
    [GuestFirstName]   NVARCHAR(100)   NOT NULL,
    [GuestLastName]    NVARCHAR(100)   NOT NULL,
    [GuestEmail]       NVARCHAR(256)   NOT NULL,
    [GuestPhone]       NVARCHAR(20)    NOT NULL,
    [CheckInDate]      DATETIME2       NOT NULL,
    [CheckOutDate]     DATETIME2       NOT NULL,
    [NumberOfGuests]   INT             NOT NULL DEFAULT 1,
    [NumberOfNights]   INT             NOT NULL DEFAULT 1,
    [PricePerNight]    DECIMAL(10,2)   NOT NULL,
    [TotalPrice]       DECIMAL(10,2)   NOT NULL,
    [Status]           INT             NOT NULL DEFAULT 0, -- 0=Pending, 1=Confirmed, 2=CheckedIn, 3=CheckedOut, 4=Cancelled
    [BookingDate]      DATETIME2       NOT NULL DEFAULT GETDATE(),
    [SpecialRequests]  NVARCHAR(500)   NULL,
    [AdminNotes]       NVARCHAR(500)   NULL,
    CONSTRAINT [PK_Bookings] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [UQ_Bookings_BookingReference] UNIQUE ([BookingReference]),
    CONSTRAINT [FK_Bookings_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [dbo].[Rooms]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Bookings_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]) ON DELETE SET NULL
);
GO

-- ============================================================
-- Indexes
-- ============================================================
CREATE INDEX [IX_Rooms_Type]           ON [dbo].[Rooms] ([Type]);
CREATE INDEX [IX_Rooms_IsActive]       ON [dbo].[Rooms] ([IsActive]);
CREATE INDEX [IX_Rooms_PricePerNight]  ON [dbo].[Rooms] ([PricePerNight]);

CREATE INDEX [IX_Bookings_RoomId]      ON [dbo].[Bookings] ([RoomId]);
CREATE INDEX [IX_Bookings_UserId]      ON [dbo].[Bookings] ([UserId]);
CREATE INDEX [IX_Bookings_Status]      ON [dbo].[Bookings] ([Status]);
CREATE INDEX [IX_Bookings_CheckInDate] ON [dbo].[Bookings] ([CheckInDate]);
CREATE INDEX [IX_Bookings_CheckOutDate] ON [dbo].[Bookings] ([CheckOutDate]);
CREATE INDEX [IX_Bookings_BookingDate] ON [dbo].[Bookings] ([BookingDate]);

CREATE INDEX [IX_AspNetUsers_NormalizedEmail]    ON [dbo].[AspNetUsers] ([NormalizedEmail]);
CREATE INDEX [IX_AspNetUsers_NormalizedUserName] ON [dbo].[AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
CREATE INDEX [IX_AspNetRoles_NormalizedName]     ON [dbo].[AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

-- ============================================================
-- Seed Data: Roles
-- ============================================================
INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
VALUES
    (NEWID(), 'Admin', 'ADMIN', NEWID()),
    (NEWID(), 'Guest', 'GUEST', NEWID());
GO

-- ============================================================
-- Seed Data: Rooms
-- ============================================================
INSERT INTO [dbo].[Rooms] ([RoomNumber], [Name], [Type], [BedType], [Description], [PricePerNight], [MaxOccupancy], [FloorNumber], [SizeInSqFt], [IsActive], [ImageUrl], [Amenities])
VALUES
    ('101', 'Comfort Standard',      0, 2, 'A cozy standard room with modern amenities, perfect for solo travelers or couples. Features a comfortable queen bed, work desk, and city views.',     99.99,  2, 1,  280, 1, '/images/rooms/standard-1.jpg',      'Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe'),
    ('102', 'Classic Standard',      0, 4, 'A well-appointed standard room with twin beds, ideal for friends or colleagues traveling together. Includes all essential amenities.',               89.99,  2, 1,  270, 1, '/images/rooms/standard-2.jpg',      'Wi-Fi,TV,Air Conditioning,Room Service,Safe'),
    ('103', 'Economy Single',        0, 0, 'Budget-friendly single room perfect for business travelers. Clean, comfortable, and equipped with all essentials.',                                  69.99,  1, 1,  200, 1, '/images/rooms/standard-3.jpg',      'Wi-Fi,TV,Air Conditioning,Work Desk,Safe'),
    ('201', 'Deluxe Garden View',    1, 3, 'Spacious deluxe room with a luxurious king bed and beautiful garden views. Features premium furnishings, a sitting area, and marble bathroom.',     179.99,  2, 2,  400, 1, '/images/rooms/deluxe-1.jpg',        'Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe,Bathrobe,Coffee Machine,Garden View'),
    ('202', 'Deluxe City View',      1, 3, 'Elegantly designed deluxe room overlooking the city skyline. Enjoy premium comfort with a king-sized bed and upgraded amenities.',                 199.99,  2, 2,  420, 1, '/images/rooms/deluxe-2.jpg',        'Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe,Bathrobe,Coffee Machine,City View,Balcony'),
    ('301', 'Executive Suite',       2, 3, 'Luxurious executive suite featuring a separate living area, dining space, and bedroom with king bed. Perfect for extended stays or business travelers.', 299.99, 3, 3, 650, 1, '/images/rooms/suite-1.jpg', 'Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe,Bathrobe,Coffee Machine,City View,Living Room,Work Desk,Jacuzzi'),
    ('302', 'Honeymoon Suite',       2, 3, 'Romantic suite designed for couples, featuring a four-poster king bed, private balcony, and in-room jacuzzi with panoramic views.',                349.99,  2, 3,  580, 1, '/images/rooms/suite-2.jpg',         'Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe,Bathrobe,Coffee Machine,Panoramic View,Balcony,Jacuzzi,Champagne'),
    ('401', 'Family Room',           3, 1, 'Spacious family room with connecting beds, designed for families with children. Includes a play area and child-friendly amenities.',               249.99,  5, 4,  550, 1, '/images/rooms/family-1.jpg',        'Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe,Kids Area,Extra Beds,Garden View'),
    ('402', 'Family Deluxe',         3, 2, 'Premium family accommodation with separate sleeping areas for parents and children, plus a kitchenette for added convenience.',                   279.99,  6, 4,  620, 1, '/images/rooms/family-2.jpg',        'Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe,Kids Area,Extra Beds,Kitchenette,Balcony'),
    ('501', 'Presidential Suite',    4, 3, 'The crown jewel of Grand Horizon Hotel. This lavish presidential suite spans the entire floor with panoramic views, private dining room, butler service, and the finest luxury amenities.', 599.99, 4, 5, 1200, 1, '/images/rooms/presidential-1.jpg', 'Wi-Fi,TV,Air Conditioning,Mini Bar,Room Service,Safe,Bathrobe,Coffee Machine,Panoramic View,Living Room,Dining Room,Jacuzzi,Butler Service,Private Terrace,Grand Piano');
GO

-- ============================================================
-- Seed Data: Sample Bookings
-- (Note: User accounts are created by the application at startup via ASP.NET Identity)
-- ============================================================
INSERT INTO [dbo].[Bookings] ([BookingReference], [RoomId], [UserId], [GuestFirstName], [GuestLastName], [GuestEmail], [GuestPhone], [CheckInDate], [CheckOutDate], [NumberOfGuests], [NumberOfNights], [PricePerNight], [TotalPrice], [Status], [BookingDate], [SpecialRequests])
VALUES
    ('HB-20261125-SAMPLE1', 1, NULL, 'John',    'Smith',    'john.smith@example.com',  '555-0201', DATEADD(DAY, 5,  CAST(GETDATE() AS DATE)), DATEADD(DAY, 8,  CAST(GETDATE() AS DATE)), 2, 3, 99.99,  299.97, 1, DATEADD(DAY, -2, GETDATE()), 'Late check-in please, arriving around 10 PM'),
    ('HB-20261125-SAMPLE2', 4, NULL, 'Sarah',   'Johnson',  'sarah.j@example.com',     '555-0202', DATEADD(DAY, 1,  CAST(GETDATE() AS DATE)), DATEADD(DAY, 4,  CAST(GETDATE() AS DATE)), 2, 3, 179.99, 539.97, 1, DATEADD(DAY, -5, GETDATE()), NULL),
    ('HB-20261125-SAMPLE3', 6, NULL, 'Michael', 'Chen',     'm.chen@example.com',      '555-0203', CAST(GETDATE() AS DATE),                   DATEADD(DAY, 2,  CAST(GETDATE() AS DATE)), 2, 2, 299.99, 599.98, 2, DATEADD(DAY, -7, GETDATE()), NULL),
    ('HB-20261125-SAMPLE4', 8, NULL, 'Emily',   'Davis',    'emily.d@example.com',     '555-0204', DATEADD(DAY, -3, CAST(GETDATE() AS DATE)), DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 4, 2, 249.99, 499.98, 3, DATEADD(DAY, -10, GETDATE()), NULL),
    ('HB-20261125-SAMPLE5', 10,NULL, 'Robert',  'Wilson',   'r.wilson@example.com',    '555-0205', DATEADD(DAY, 10, CAST(GETDATE() AS DATE)), DATEADD(DAY, 14, CAST(GETDATE() AS DATE)), 2, 4, 599.99, 2399.96, 0, DATEADD(DAY, -1, GETDATE()), 'Anniversary celebration - champagne and flowers in room');
GO

PRINT 'Database schema created and sample data inserted successfully!';
GO
