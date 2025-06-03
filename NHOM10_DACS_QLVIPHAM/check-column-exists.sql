USE QLVIPHAM_DB;

-- Check if MaBienBan column exists in KhieuNaiVPhams table
IF EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'KhieuNaiVPhams' AND COLUMN_NAME = 'MaBienBan'
)
BEGIN
    PRINT 'MaBienBan column exists in KhieuNaiVPhams table';
    
    -- Get column details
    SELECT 
        c.name AS ColumnName,
        t.name AS DataType,
        c.is_nullable AS IsNullable,
        CASE WHEN fk.name IS NOT NULL THEN 'Yes' ELSE 'No' END AS HasForeignKey,
        OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable
    FROM sys.columns c
    JOIN sys.types t ON c.user_type_id = t.user_type_id
    LEFT JOIN sys.foreign_key_columns fkc ON fkc.parent_object_id = OBJECT_ID('KhieuNaiVPhams') 
        AND fkc.parent_column_id = c.column_id
    LEFT JOIN sys.foreign_keys fk ON fk.object_id = fkc.constraint_object_id
    WHERE c.object_id = OBJECT_ID('KhieuNaiVPhams') AND c.name = 'MaBienBan';
END
ELSE
BEGIN
    PRINT 'MaBienBan column does NOT exist in KhieuNaiVPhams table';
END 