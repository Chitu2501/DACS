USE QLVIPHAM_DB;

-- Update the NgayLadDuBienBan column to allow NULL values
ALTER TABLE dbo.BienBanVPhams
ALTER COLUMN NgayLadDuBienBan DATETIME2 NULL;

-- Update any records with a MinValue date to NULL
UPDATE dbo.BienBanVPhams
SET NgayLadDuBienBan = NULL
WHERE NgayLadDuBienBan = '0001-01-01 00:00:00.0000000';

PRINT 'Column NgayLadDuBienBan has been modified to allow NULL values.'; 