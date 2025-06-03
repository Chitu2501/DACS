-- Turn on output for messages
SET NOCOUNT OFF;

PRINT 'Listing all available databases:';
SELECT name FROM sys.databases;

-- Now check for table names in specific database
PRINT 'Trying to use application database...';
USE [QLVIPHAM_DB];

PRINT 'Listing all tables in the database:';
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';

-- Check if TheCanCuocs exists
PRINT 'Checking if TheCanCuocs table exists...';
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TheCanCuocs')
BEGIN
    PRINT 'TheCanCuocs table found. Adding new columns...';
    
    -- Check if columns already exist before adding them
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TheCanCuocs' AND COLUMN_NAME = 'AnhMatTruoc')
    BEGIN
        PRINT 'Adding AnhMatTruoc column...';
        ALTER TABLE TheCanCuocs ADD AnhMatTruoc NVARCHAR(MAX) NULL;
    END
    ELSE
    BEGIN
        PRINT 'AnhMatTruoc column already exists.';
    END
    
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TheCanCuocs' AND COLUMN_NAME = 'AnhMatSau')
    BEGIN
        PRINT 'Adding AnhMatSau column...';
        ALTER TABLE TheCanCuocs ADD AnhMatSau NVARCHAR(MAX) NULL;
    END
    ELSE
    BEGIN
        PRINT 'AnhMatSau column already exists.';
    END
    
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TheCanCuocs' AND COLUMN_NAME = 'QuocTich')
    BEGIN
        PRINT 'Adding QuocTich column...';
        ALTER TABLE TheCanCuocs ADD QuocTich NVARCHAR(100) NULL;
    END
    ELSE
    BEGIN
        PRINT 'QuocTich column already exists.';
    END
    
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TheCanCuocs' AND COLUMN_NAME = 'TrangThai')
    BEGIN
        PRINT 'Adding TrangThai column...';
        ALTER TABLE TheCanCuocs ADD TrangThai NVARCHAR(50) NULL;
    END
    ELSE
    BEGIN
        PRINT 'TrangThai column already exists.';
    END
    
    PRINT 'All columns added successfully.';
END
ELSE
BEGIN
    PRINT 'TheCanCuocs table not found. Checking for alternative names...';
    
    -- List tables that might be related to TheCanCuoc
    SELECT TABLE_NAME 
    FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_NAME LIKE '%The%' OR TABLE_NAME LIKE '%Can%' OR TABLE_NAME LIKE '%Cuoc%';
END 