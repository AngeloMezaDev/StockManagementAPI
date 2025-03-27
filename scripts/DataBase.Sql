-- Create database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'InventoryDB')
BEGIN
    CREATE DATABASE InventoryDB;
END
GO

USE InventoryDB;
GO

-- Create Products table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products' AND type = 'U')
BEGIN
    CREATE TABLE Products (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        Category NVARCHAR(50),
        ImageUrl NVARCHAR(255),
        Price DECIMAL(18, 2) NOT NULL,
        Stock INT NOT NULL DEFAULT 0
    );
END
GO

-- Create Transactions table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Transactions' AND type = 'U')
BEGIN
    CREATE TABLE Transactions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Date DATETIME NOT NULL DEFAULT GETDATE(),
    Type INT NOT NULL CHECK (Type IN (0, 1)), -- 0=Purchase, 1=Sale
        ProductId INT FOREIGN KEY REFERENCES Products(Id),
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18, 2) NOT NULL,
        TotalPrice DECIMAL(18, 2) NOT NULL,
        Details NVARCHAR(500),
        CONSTRAINT CHK_Quantity CHECK (Quantity > 0)
    );
    
    -- Add a comment explaining the expected values for Type column
    EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Type column should contain "Purchase" or "Sale". Maps to TransactionType enum (0=Purchase, 1=Sale)',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Transactions',
    @level2type = N'COLUMN', @level2name = N'Type';
END
GO

-- Insert sample data only if Products table is empty
IF NOT EXISTS (SELECT TOP 1 * FROM Products)
BEGIN
    INSERT INTO Products (Name, Description, Category, ImageUrl, Price, Stock)
    VALUES 
    ('Laptop HP 15', 'Laptop HP 15 with Intel Core i5, 8GB RAM, 512GB SSD', 'Electronics', 'laptop.jpg', 899.99, 15),
    ('Wireless Mouse', 'Ergonomic wireless mouse with long battery life', 'Accessories', 'mouse.jpg', 29.99, 50),
    ('Desk Chair', 'Comfortable office chair with adjustable height', 'Furniture', 'chair.jpg', 149.99, 10),
    ('External HDD 1TB', 'External hard drive with 1TB storage capacity', 'Storage', 'hdd.jpg', 79.99, 25),
    ('Bluetooth Speaker', 'Portable bluetooth speaker with 10h battery life', 'Audio', 'speaker.jpg', 59.99, 30);
END
GO