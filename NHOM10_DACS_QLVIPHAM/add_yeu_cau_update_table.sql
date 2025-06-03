-- Check if table already exists and create if it doesn't
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'YeuCauCapNhatThongTins')
BEGIN
    CREATE TABLE [dbo].[YeuCauCapNhatThongTins] (
        [MaYeuCau] INT IDENTITY(1,1) NOT NULL,
        [MaCongDan] INT NULL,
        [MaNguoiDung] NVARCHAR(450) NULL,
        [NgayYeuCau] DATETIME2 NOT NULL,
        [NoiDungCapNhat] NVARCHAR(MAX) NOT NULL,
        [ThongTinCapNhat] NVARCHAR(MAX) NOT NULL,
        [TrangThai] NVARCHAR(50) NOT NULL,
        [NgayXuLy] DATETIME2 NULL,
        [NguoiXuLy] NVARCHAR(MAX) NULL,
        [GhiChu] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_YeuCauCapNhatThongTins] PRIMARY KEY ([MaYeuCau]),
        CONSTRAINT [FK_YeuCauCapNhatThongTins_CongDans_MaCongDan] FOREIGN KEY ([MaCongDan]) REFERENCES [CongDans] ([MaCongDan]) ON DELETE NO ACTION,
        CONSTRAINT [FK_YeuCauCapNhatThongTins_AspNetUsers_MaNguoiDung] FOREIGN KEY ([MaNguoiDung]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );

    CREATE INDEX [IX_YeuCauCapNhatThongTins_MaCongDan] ON [dbo].[YeuCauCapNhatThongTins] ([MaCongDan]);
    CREATE INDEX [IX_YeuCauCapNhatThongTins_MaNguoiDung] ON [dbo].[YeuCauCapNhatThongTins] ([MaNguoiDung]);
    
    PRINT 'YeuCauCapNhatThongTins table created successfully.';
END
ELSE
BEGIN
    PRINT 'YeuCauCapNhatThongTins table already exists.';
END 