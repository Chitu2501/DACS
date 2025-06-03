-- SQL Script to create LichSuThanhToans table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LichSuThanhToans')
BEGIN
    CREATE TABLE [dbo].[LichSuThanhToans] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [MaBienBan] INT NOT NULL,
        [SoTien] DECIMAL(18, 2) NOT NULL,
        [PhuongThucThanhToan] NVARCHAR(100) NULL,
        [NgayThanhToan] DATETIME NOT NULL,
        [MaGiaoDich] NVARCHAR(100) NULL,
        CONSTRAINT [PK_LichSuThanhToans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LichSuThanhToans_BienBanVPhams_MaBienBan] FOREIGN KEY ([MaBienBan]) 
        REFERENCES [BienBanVPhams] ([MaBienBan]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_LichSuThanhToans_MaBienBan] ON [dbo].[LichSuThanhToans] ([MaBienBan]);
    
    PRINT 'LichSuThanhToans table created successfully.';
END
ELSE
BEGIN
    PRINT 'LichSuThanhToans table already exists.';
END 