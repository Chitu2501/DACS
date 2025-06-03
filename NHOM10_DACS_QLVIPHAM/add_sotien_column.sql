-- Add SoTienPhat column to BienBanVPhams table
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'BienBanVPhams' AND COLUMN_NAME = 'SoTienPhat'
)
BEGIN
    ALTER TABLE BienBanVPhams
    ADD SoTienPhat decimal(18, 2) NOT NULL DEFAULT 0;
    
    PRINT 'Column SoTienPhat added successfully.';
END
ELSE
BEGIN
    PRINT 'Column SoTienPhat already exists.';
END 