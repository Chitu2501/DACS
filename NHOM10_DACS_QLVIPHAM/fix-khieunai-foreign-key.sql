USE QLVIPHAM_DB;

-- Check if the FK constraints exist and if not, add them
IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys 
    WHERE name = 'FK_KhieuNaiVPhams_BienBanVPhams_MaBienBan'
    AND parent_object_id = OBJECT_ID('dbo.KhieuNaiVPhams')
)
BEGIN
    -- Add foreign key constraint
    ALTER TABLE dbo.KhieuNaiVPhams
    ADD CONSTRAINT FK_KhieuNaiVPhams_BienBanVPhams_MaBienBan
    FOREIGN KEY (MaBienBan) REFERENCES dbo.BienBanVPhams(MaBienBan);
    
    PRINT 'Foreign key constraint FK_KhieuNaiVPhams_BienBanVPhams_MaBienBan added successfully';
END
ELSE
BEGIN
    PRINT 'Foreign key constraint FK_KhieuNaiVPhams_BienBanVPhams_MaBienBan already exists';
END

-- Check if the index exists and if not, add it
IF NOT EXISTS (
    SELECT * FROM sys.indexes 
    WHERE name = 'IX_KhieuNaiVPhams_MaBienBan'
    AND object_id = OBJECT_ID('dbo.KhieuNaiVPhams')
)
BEGIN
    -- Create index
    CREATE INDEX IX_KhieuNaiVPhams_MaBienBan ON dbo.KhieuNaiVPhams(MaBienBan);
    
    PRINT 'Index IX_KhieuNaiVPhams_MaBienBan created successfully';
END
ELSE
BEGIN
    PRINT 'Index IX_KhieuNaiVPhams_MaBienBan already exists';
END 