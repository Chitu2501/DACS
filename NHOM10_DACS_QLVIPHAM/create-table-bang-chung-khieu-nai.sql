USE QLVIPHAM_DB;

-- Check if the BangChungKhieuNais table exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BangChungKhieuNais')
BEGIN
    -- Create the BangChungKhieuNais table
    CREATE TABLE [dbo].[BangChungKhieuNais] (
        [MaBangChung] INT IDENTITY(1,1) NOT NULL,
        [MaKhieuNai] INT NOT NULL,
        [DuongDanFile] NVARCHAR(MAX) NULL,
        [MoTa] NVARCHAR(500) NULL,
        [NgayTao] DATETIME2 NOT NULL,
        [NguoiTao] NVARCHAR(256) NULL,
        CONSTRAINT [PK_BangChungKhieuNais] PRIMARY KEY ([MaBangChung]),
        CONSTRAINT [FK_BangChungKhieuNais_KhieuNaiVPhams_MaKhieuNai] FOREIGN KEY ([MaKhieuNai]) REFERENCES [dbo].[KhieuNaiVPhams] ([MaKhieuNai]) ON DELETE CASCADE
    );

    -- Create index on MaKhieuNai column
    CREATE INDEX [IX_BangChungKhieuNais_MaKhieuNai] ON [dbo].[BangChungKhieuNais] ([MaKhieuNai]);

    PRINT 'BangChungKhieuNais table created successfully.';
END
ELSE
BEGIN
    PRINT 'BangChungKhieuNais table already exists.';
END 