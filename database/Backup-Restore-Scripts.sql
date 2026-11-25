-- ============================================================
-- SQL Server Backup & Restore Scripts
-- Database: HotelBookingDB
-- ============================================================

-- 1. Backup Script
-- Run this in SQL Server Management Studio to create a fresh backup
USE master;
GO

BACKUP DATABASE [HotelBookingDB]
TO DISK = N'D:\Hotel_Management_System\database\HotelBookingDB.bak'
WITH FORMAT,
     MEDIANAME = 'HotelBookingBackup',
     NAME = 'Full Backup of HotelBookingDB',
     STATS = 10;
GO

-- 2. Restore Script
-- Use this script to restore the database onto any SQL Server instance
USE master;
GO

-- If the database is currently open in other connections, drop them
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'HotelBookingDB')
BEGIN
    ALTER DATABASE [HotelBookingDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
END
GO

RESTORE DATABASE [HotelBookingDB]
FROM DISK = N'D:\Hotel_Management_System\database\HotelBookingDB.bak'
WITH FILE = 1,
     REPLACE,
     STATS = 10;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'HotelBookingDB')
BEGIN
    ALTER DATABASE [HotelBookingDB] SET MULTI_USER;
END
GO
