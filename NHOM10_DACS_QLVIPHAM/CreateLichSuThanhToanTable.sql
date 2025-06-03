-- Check if the table already exists
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LichSuThanhToans]') AND type in (N'U'))
BEGIN
    -- Create the table
    CREATE TABLE [dbo].[LichSuThanhToans](
        [MaLichSu] [int] IDENTITY(1,1) NOT NULL,
        [MaBienBan] [int] NOT NULL,
        [SoTien] [decimal](18, 0) NOT NULL,
        [PhuongThucThanhToan] [nvarchar](max) NULL,
        [NgayThanhToan] [datetime2](7) NOT NULL,
        [MaGiaoDich] [nvarchar](max) NULL,
        CONSTRAINT [PK_LichSuThanhToans] PRIMARY KEY CLUSTERED ([MaLichSu] ASC)
    );

    -- Add foreign key constraint to BienBanVPhams
    ALTER TABLE [dbo].[LichSuThanhToans] WITH CHECK ADD CONSTRAINT [FK_LichSuThanhToans_BienBanVPhams_MaBienBan] 
    FOREIGN KEY([MaBienBan]) REFERENCES [dbo].[BienBanVPhams] ([MaBienBan]) ON DELETE CASCADE;

    PRINT 'Table LichSuThanhToans created successfully';
END
ELSE
BEGIN
    PRINT 'Table LichSuThanhToans already exists';
END 