-- Check if the table already exists, if not create it
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'YeuCauBangChungViPhams')
BEGIN
    CREATE TABLE [dbo].[YeuCauBangChungViPhams] (
        [MaYeuCau] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [MaBienBan] INT NOT NULL,
        [MaCongDan] INT NOT NULL,
        [NgayYeuCau] DATETIME2 NOT NULL,
        [LyDoYeuCau] NVARCHAR(MAX) NULL,
        [TrangThai] NVARCHAR(50) NULL,
        [NgayXuLy] DATETIME2 NULL,
        [GhiChu] NVARCHAR(MAX) NULL,
        [NguoiXuLy] NVARCHAR(256) NULL,
        [DuongDanBangChung] NVARCHAR(MAX) NULL,
        CONSTRAINT [FK_YeuCauBangChungViPhams_BienBanVPhams] FOREIGN KEY ([MaBienBan]) REFERENCES [dbo].[BienBanVPhams] ([MaBienBan]) ON DELETE NO ACTION,
        CONSTRAINT [FK_YeuCauBangChungViPhams_CongDans] FOREIGN KEY ([MaCongDan]) REFERENCES [dbo].[CongDans] ([MaCongDan]) ON DELETE NO ACTION
    );
    PRINT 'Created table YeuCauBangChungViPhams';
END
ELSE
BEGIN
    PRINT 'Table YeuCauBangChungViPhams already exists';
END 