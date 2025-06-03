-- Ensure we're using the right database
USE [NHOM10_DACS_QLVIPHAM];
GO

-- Enable printing of messages
SET NOCOUNT OFF;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Add MaNguoiDung column to CongDans table if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CongDans]') AND name = 'MaNguoiDung')
    BEGIN
        ALTER TABLE CongDans ADD MaNguoiDung nvarchar(450) NULL;
        PRINT 'Added MaNguoiDung column to CongDans table';
    END
    ELSE
    BEGIN
        PRINT 'MaNguoiDung column already exists';
    END

    -- Add GioiTinh column if GIOTTING exists but GioiTinh doesn't (we'll maintain both for compatibility)
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CongDans]') AND name = 'GIOTTING')
       AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CongDans]') AND name = 'GioiTinh')
    BEGIN
        ALTER TABLE CongDans ADD GioiTinh nvarchar(max) NULL;
        PRINT 'Added GioiTinh column to CongDans table';
        
        -- Copy data from GIOTTING to GioiTinh
        UPDATE CongDans SET GioiTinh = GIOTTING;
        PRINT 'Copied data from GIOTTING to GioiTinh';
    END
    ELSE
    BEGIN
        PRINT 'GioiTinh column already exists or GIOTTING column does not exist';
    END

    -- Add foreign key constraint
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CongDans]') AND name = 'MaNguoiDung')
       AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'[dbo].[CongDans]') AND name = 'FK_CongDans_AspNetUsers_MaNguoiDung')
    BEGIN
        -- First, ensure all MaNguoiDung values exist in AspNetUsers or set to NULL
        UPDATE CongDans
        SET MaNguoiDung = NULL
        WHERE MaNguoiDung IS NOT NULL 
        AND NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = MaNguoiDung);
        
        -- Now add the constraint
        ALTER TABLE CongDans
        ADD CONSTRAINT FK_CongDans_AspNetUsers_MaNguoiDung
        FOREIGN KEY (MaNguoiDung) REFERENCES AspNetUsers(Id);
        PRINT 'Added foreign key constraint FK_CongDans_AspNetUsers_MaNguoiDung';
    END
    ELSE
    BEGIN
        PRINT 'Foreign key constraint already exists or MaNguoiDung column does not exist';
    END

    COMMIT TRANSACTION;
    PRINT 'Database update completed successfully';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Error occurred while updating database:';
    PRINT ERROR_MESSAGE();
END CATCH; 