-- Add missing columns to ThongBao table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'UserId' AND Object_ID = Object_ID('ThongBaos'))
BEGIN
    ALTER TABLE ThongBaos
    ADD UserId NVARCHAR(450) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'DaDoc' AND Object_ID = Object_ID('ThongBaos'))
BEGIN
    ALTER TABLE ThongBaos
    ADD DaDoc BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'LinkChiTiet' AND Object_ID = Object_ID('ThongBaos'))
BEGIN
    ALTER TABLE ThongBaos
    ADD LinkChiTiet NVARCHAR(MAX) NOT NULL DEFAULT '';
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'MaBienBan' AND Object_ID = Object_ID('ThongBaos'))
BEGIN
    ALTER TABLE ThongBaos
    ADD MaBienBan INT NULL;
END

-- Rename NgayThongBao to NgayTao if it exists
IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'NgayThongBao' AND Object_ID = Object_ID('ThongBaos'))
AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'NgayTao' AND Object_ID = Object_ID('ThongBaos'))
BEGIN
    EXEC sp_rename 'ThongBaos.NgayThongBao', 'NgayTao', 'COLUMN';
END

-- Make MaCongDan nullable if it's not already
IF EXISTS (SELECT 1 FROM sys.columns 
           WHERE Name = 'MaCongDan' 
           AND Object_ID = Object_ID('ThongBaos')
           AND is_nullable = 0)
BEGIN
    -- First drop any constraints on the column
    DECLARE @constraintName NVARCHAR(200)
    SELECT @constraintName = name
    FROM sys.default_constraints
    WHERE parent_object_id = Object_ID('ThongBaos')
    AND parent_column_id = (
        SELECT column_id 
        FROM sys.columns
        WHERE Name = 'MaCongDan'
        AND Object_ID = Object_ID('ThongBaos')
    )
    
    IF @constraintName IS NOT NULL
        EXEC('ALTER TABLE ThongBaos DROP CONSTRAINT ' + @constraintName)
    
    -- Now alter the column
    ALTER TABLE ThongBaos
    ALTER COLUMN MaCongDan INT NULL;
END

-- Add foreign key relationships if they don't exist
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
               WHERE parent_object_id = Object_ID('ThongBaos')
               AND referenced_object_id = Object_ID('AspNetUsers'))
BEGIN
    ALTER TABLE ThongBaos
    ADD CONSTRAINT FK_ThongBaos_AspNetUsers FOREIGN KEY (UserId)
    REFERENCES AspNetUsers (Id);
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
               WHERE parent_object_id = Object_ID('ThongBaos')
               AND referenced_object_id = Object_ID('CongDans'))
BEGIN
    ALTER TABLE ThongBaos
    ADD CONSTRAINT FK_ThongBaos_CongDans FOREIGN KEY (MaCongDan)
    REFERENCES CongDans (MaCongDan);
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
               WHERE parent_object_id = Object_ID('ThongBaos')
               AND referenced_object_id = Object_ID('BienBanVPhams'))
BEGIN
    ALTER TABLE ThongBaos
    ADD CONSTRAINT FK_ThongBaos_BienBanVPhams FOREIGN KEY (MaBienBan)
    REFERENCES BienBanVPhams (MaBienBan);
END 