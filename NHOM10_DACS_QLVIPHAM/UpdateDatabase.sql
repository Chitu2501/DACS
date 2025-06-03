-- Check if NgayThongBao exists and rename it to NgayTao
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'NgayThongBao' AND Object_ID = Object_ID('ThongBaos'))
BEGIN
    EXEC sp_rename 'ThongBaos.NgayThongBao', 'NgayTao', 'COLUMN';
    PRINT 'Renamed NgayThongBao to NgayTao';
END
ELSE
BEGIN
    PRINT 'NgayThongBao column does not exist or already renamed';
END

-- Check if TrangThai exists and rename it to DaDoc (boolean)
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'TrangThai' AND Object_ID = Object_ID('ThongBaos'))
BEGIN
    -- First add the new boolean column
    ALTER TABLE ThongBaos ADD DaDoc BIT NOT NULL DEFAULT 0;
    PRINT 'Added DaDoc column';
    
    -- Update values (assuming "Chưa đọc" means not read)
    UPDATE ThongBaos SET DaDoc = 0 WHERE TrangThai = N'Chưa đọc';
    UPDATE ThongBaos SET DaDoc = 1 WHERE TrangThai != N'Chưa đọc';
    PRINT 'Updated DaDoc values based on TrangThai';
    
    -- Drop the old column
    ALTER TABLE ThongBaos DROP COLUMN TrangThai;
    PRINT 'Dropped TrangThai column';
END
ELSE
BEGIN
    PRINT 'TrangThai column does not exist or already removed';
END

-- Check if Role column exists and add it if missing
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'Role' AND Object_ID = Object_ID('ThongBaos'))
BEGIN
    ALTER TABLE ThongBaos ADD Role NVARCHAR(50) NULL;
    PRINT 'Added Role column';
END
ELSE
BEGIN
    PRINT 'Role column already exists';
END

-- Check if LinkChiTiet column exists and add it if missing
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'LinkChiTiet' AND Object_ID = Object_ID('ThongBaos'))
BEGIN
    ALTER TABLE ThongBaos ADD LinkChiTiet NVARCHAR(255) NULL;
    PRINT 'Added LinkChiTiet column';
END
ELSE
BEGIN
    PRINT 'LinkChiTiet column already exists';
END

-- Check if MaBienBan column exists and add it if missing
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'MaBienBan' AND Object_ID = Object_ID('ThongBaos'))
BEGIN
    ALTER TABLE ThongBaos ADD MaBienBan INT NULL;
    
    -- Add foreign key constraint if BienBanVPhams table exists
    IF EXISTS (SELECT 1 FROM sys.tables WHERE Name = 'BienBanVPhams')
    BEGIN
        ALTER TABLE ThongBaos 
        ADD CONSTRAINT FK_ThongBaos_BienBanVPhams_MaBienBan 
        FOREIGN KEY (MaBienBan) REFERENCES BienBanVPhams(MaBienBan);
    END
    
    PRINT 'Added MaBienBan column with foreign key constraint';
END
ELSE
BEGIN
    PRINT 'MaBienBan column already exists';
END

PRINT 'Database update completed'; 