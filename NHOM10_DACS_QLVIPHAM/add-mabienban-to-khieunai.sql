-- Add MaBienBan column to KhieuNaiVPhams table if it doesn't exist
USE QLVIPHAM_DB;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'MaBienBan' AND Object_ID = Object_ID(N'dbo.KhieuNaiVPhams'))
BEGIN
    -- Make NgayTraLoi nullable first (if it's not already)
    IF EXISTS (SELECT 1 FROM sys.columns 
               WHERE Name = N'NgayTraLoi' 
               AND Object_ID = Object_ID(N'dbo.KhieuNaiVPhams')
               AND is_nullable = 0)
    BEGIN
        ALTER TABLE dbo.KhieuNaiVPhams
        ALTER COLUMN NgayTraLoi datetime2 NULL;
    END

    -- Add MaBienBan column
    ALTER TABLE dbo.KhieuNaiVPhams
    ADD MaBienBan int NULL;
    
    -- Create index
    CREATE INDEX IX_KhieuNaiVPhams_MaBienBan ON dbo.KhieuNaiVPhams(MaBienBan);
    
    -- Add foreign key constraint
    ALTER TABLE dbo.KhieuNaiVPhams
    ADD CONSTRAINT FK_KhieuNaiVPhams_BienBanVPhams_MaBienBan
    FOREIGN KEY (MaBienBan) REFERENCES dbo.BienBanVPhams(MaBienBan);
    
    PRINT 'MaBienBan column added to KhieuNaiVPhams table';
END
ELSE
BEGIN
    PRINT 'MaBienBan column already exists in KhieuNaiVPhams table';
END 