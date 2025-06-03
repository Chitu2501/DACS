-- Update the migrations history table to mark the migrations as applied
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250510185226_AddLichSuThanhToanTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250510185226_AddLichSuThanhToanTable', '9.0.0');
    PRINT 'Added migration 20250510185226_AddLichSuThanhToanTable to the history table';
END
ELSE
BEGIN
    PRINT 'Migration 20250510185226_AddLichSuThanhToanTable already exists in the history table';
END

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250511092135_AddThongBaoColumns')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250511092135_AddThongBaoColumns', '9.0.0');
    PRINT 'Added migration 20250511092135_AddThongBaoColumns to the history table';
END
ELSE
BEGIN
    PRINT 'Migration 20250511092135_AddThongBaoColumns already exists in the history table';
END

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250511125602_AddYeuCauBangChungViPhamsTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250511125602_AddYeuCauBangChungViPhamsTable', '9.0.0');
    PRINT 'Added migration 20250511125602_AddYeuCauBangChungViPhamsTable to the history table';
END
ELSE
BEGIN
    PRINT 'Migration 20250511125602_AddYeuCauBangChungViPhamsTable already exists in the history table';
END 