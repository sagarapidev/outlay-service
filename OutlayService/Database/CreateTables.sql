-- OutlayService Database Schema Creation Script
-- Run this script against your Azure SQL Database to create the required tables

-- Create Users table
CREATE TABLE [dbo].[Users]
(
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(256) NOT NULL,
    [CreatedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Create index on Email column for faster lookups
CREATE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users]
(
    [Email] ASC
);

-- Add a unique constraint on Email to prevent duplicate email addresses
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [UQ_Users_Email] UNIQUE ([Email]);

-- Optional: Add a check constraint to ensure Email is not empty
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [CK_Users_Email_NotEmpty] CHECK (LEN([Email]) > 0);

-- Optional: Add a check constraint to ensure Name is not empty
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [CK_Users_Name_NotEmpty] CHECK (LEN([Name]) > 0);
