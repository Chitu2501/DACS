-- Add NgayThanhToan column to BienBanVPhams table
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'BienBanVPhams' 
    AND COLUMN_NAME = 'NgayThanhToan'
)
BEGIN
    ALTER TABLE BienBanVPhams
    ADD NgayThanhToan DATETIME NULL;
    
    PRINT 'NgayThanhToan column added successfully.'
END
ELSE
BEGIN
    PRINT 'NgayThanhToan column already exists.'
END 