using System;
using Microsoft.Data.SqlClient;

namespace DbUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Use the correct connection string from appsettings.json
                string connectionString = "Server=SWEETHER;Database=QLVIPHAM_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
                
                // SQL to create LichSuThanhToans table if it doesn't exist
                string sql = @"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = 'LichSuThanhToans'
                )
                BEGIN
                    CREATE TABLE LichSuThanhToans (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        MaBienBan INT NOT NULL,
                        SoTien DECIMAL(18, 2) NOT NULL,
                        PhuongThucThanhToan NVARCHAR(100) NULL,
                        NgayThanhToan DATETIME NOT NULL,
                        MaGiaoDich NVARCHAR(100) NULL,
                        CONSTRAINT FK_LichSuThanhToans_BienBanVPhams FOREIGN KEY (MaBienBan) 
                        REFERENCES BienBanVPhams(MaBienBan) ON DELETE CASCADE
                    );
                    
                    PRINT 'Table LichSuThanhToans created successfully.';
                END
                ELSE
                BEGIN
                    PRINT 'Table LichSuThanhToans already exists.';
                END";

                Console.WriteLine("Connecting to database and creating LichSuThanhToans table...");
                
                // Execute SQL command
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Database connection opened successfully.");
                    
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Database update operation completed successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating database: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
} 