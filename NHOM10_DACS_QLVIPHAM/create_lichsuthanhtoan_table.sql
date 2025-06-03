-- Run this script in SQL Server Management Studio or Azure Data Studio 
-- against the QLVIPHAM_DB database on the SWEETHER server

USE QLVIPHAM_DB;
GO

-- Check if the table already exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LichSuThanhToans')
BEGIN
    -- Create the LichSuThanhToans table
    CREATE TABLE LichSuThanhToans (
        Id INT IDENTITY(1,1) NOT NULL,
        MaBienBan INT NOT NULL,
        SoTien DECIMAL(18, 2) NOT NULL,
        PhuongThucThanhToan NVARCHAR(100) NULL,
        NgayThanhToan DATETIME NOT NULL,
        MaGiaoDich NVARCHAR(100) NULL,
        CONSTRAINT PK_LichSuThanhToans PRIMARY KEY (Id),
        CONSTRAINT FK_LichSuThanhToans_BienBanVPhams FOREIGN KEY (MaBienBan) 
        REFERENCES BienBanVPhams(MaBienBan) ON DELETE CASCADE
    );

    -- Create index on MaBienBan
    CREATE INDEX IX_LichSuThanhToans_MaBienBan ON LichSuThanhToans (MaBienBan);
    
    PRINT 'Table LichSuThanhToans created successfully.';
END
ELSE
BEGIN
    PRINT 'Table LichSuThanhToans already exists.';
END
GO 