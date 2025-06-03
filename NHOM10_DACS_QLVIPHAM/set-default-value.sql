USE QLVIPHAM_DB;

-- Option 1: Set a default constraint for the column
IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_BienBanVPhams_NgayLadDuBienBan')
BEGIN
    ALTER TABLE dbo.BienBanVPhams
    ADD CONSTRAINT DF_BienBanVPhams_NgayLadDuBienBan
    DEFAULT GETDATE() FOR NgayLadDuBienBan;
    
    PRINT 'Default constraint added for NgayLadDuBienBan column.';
END

-- Option 2: Update existing NULL values to current date/time
UPDATE dbo.BienBanVPhams
SET NgayLadDuBienBan = GETDATE()
WHERE NgayLadDuBienBan IS NULL;

PRINT 'Updated any NULL values in NgayLadDuBienBan column to current date.'; 