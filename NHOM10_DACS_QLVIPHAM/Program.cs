using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHOM10_DACS_QLVIPHAM.Models;
using NHOM10_DACS_QLVIPHAM.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure logging in development
if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddConsole().SetMinimumLevel(LogLevel.Debug);
}

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings - make less restrictive for testing
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false; // Changed to false to simplify testing
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6; // Changed from 8 to 6 for testing
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    
    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false; // Ensure this is false so users can log in without email confirmation
    options.SignIn.RequireConfirmedAccount = false; // Ensure this is false so users can log in without account confirmation
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure PayPal
builder.Services.Configure<PayPalConfig>(builder.Configuration.GetSection("PayPal"));

// Đăng ký ThongBaoService
builder.Services.AddScoped<IThongBaoService, ThongBaoService>();

// Đăng ký GeminiAIService và HttpClient
builder.Services.AddHttpClient<IGeminiAIService, GeminiAIService>();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Set server URLs with different ports to avoid conflicts
builder.WebHost.UseUrls("http://localhost:7895", "https://localhost:8905");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // In development environment, show detailed errors
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // This will create the database if it doesn't exist and apply any pending migrations
        Console.WriteLine("Database initialized successfully.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
        throw;
    }
}

// Initialize roles and admin account
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await SeedRolesAsync(roleManager);
    await SeedAdminUserAsync(userManager, roleManager);
}

// Add this after the app builder is configured but before app.Run()
// Add the SoTienPhat column if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var connection = dbContext.Database.GetDbConnection();
    
    try
    {
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
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
            END";
            
            command.ExecuteNonQuery();
            Console.WriteLine("SoTienPhat column check completed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error checking/adding SoTienPhat column: {ex.Message}");
    }
    finally
    {
        if (connection.State == System.Data.ConnectionState.Open)
            connection.Close();
    }
}

// Add after the SoTienPhat column check
// Add YeuCauCapNhatThongTins table if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var connection = dbContext.Database.GetDbConnection();
    
    try
    {
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
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
            END";
            
            command.ExecuteNonQuery();
            Console.WriteLine("YeuCauCapNhatThongTins table check completed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error checking/adding YeuCauCapNhatThongTins table: {ex.Message}");
    }
    finally
    {
        if (connection.State == System.Data.ConnectionState.Open)
            connection.Close();
    }
}

// Add after the YeuCauCapNhatThongTins table check
// Add LichSuThanhToans table if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var connection = dbContext.Database.GetDbConnection();
    
    try
    {
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
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
            END";
            
            command.ExecuteNonQuery();
            Console.WriteLine("LichSuThanhToans table check completed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error checking/adding LichSuThanhToans table: {ex.Message}");
    }
    finally
    {
        if (connection.State == System.Data.ConnectionState.Open)
            connection.Close();
    }
}

// Add after the LichSuThanhToans table check
// Update ThongBaos table if necessary
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var connection = dbContext.Database.GetDbConnection();
    
    try
    {
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            -- Add UserId column if it doesn't exist
            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'UserId' AND Object_ID = Object_ID('ThongBaos'))
            BEGIN
                ALTER TABLE ThongBaos
                ADD UserId NVARCHAR(450) NULL;
                
                PRINT 'Column UserId added successfully.';
            END
            ELSE
            BEGIN
                PRINT 'Column UserId already exists.';
            END

            -- Rename NgayThongBao to NgayTao if NgayThongBao exists and NgayTao doesn't
            IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'NgayThongBao' AND Object_ID = Object_ID('ThongBaos'))
            AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'NgayTao' AND Object_ID = Object_ID('ThongBaos'))
            BEGIN
                EXEC sp_rename 'ThongBaos.NgayThongBao', 'NgayTao', 'COLUMN';
                PRINT 'Column NgayThongBao renamed to NgayTao.';
            END
            ELSE
            BEGIN
                PRINT 'Column rename check completed.';
            END

            -- Add DaDoc column if it doesn't exist
            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'DaDoc' AND Object_ID = Object_ID('ThongBaos'))
            BEGIN
                ALTER TABLE ThongBaos
                ADD DaDoc BIT NOT NULL DEFAULT 0;
                
                PRINT 'Column DaDoc added successfully.';
            END
            ELSE
            BEGIN
                PRINT 'Column DaDoc already exists.';
            END

            -- Add LinkChiTiet column if it doesn't exist
            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'LinkChiTiet' AND Object_ID = Object_ID('ThongBaos'))
            BEGIN
                ALTER TABLE ThongBaos
                ADD LinkChiTiet NVARCHAR(MAX) NOT NULL DEFAULT '';
                
                PRINT 'Column LinkChiTiet added successfully.';
            END
            ELSE
            BEGIN
                PRINT 'Column LinkChiTiet already exists.';
            END

            -- Add MaBienBan column if it doesn't exist
            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'MaBienBan' AND Object_ID = Object_ID('ThongBaos'))
            BEGIN
                ALTER TABLE ThongBaos
                ADD MaBienBan INT NULL;
                
                PRINT 'Column MaBienBan added successfully.';
            END
            ELSE
            BEGIN
                PRINT 'Column MaBienBan already exists.';
            END

            -- Make MaCongDan nullable if it's not already
            IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'MaCongDan' AND Object_ID = Object_ID('ThongBaos')
                       AND is_nullable = 0)
            BEGIN
                -- Drop constraints if they exist
                DECLARE @constraintName NVARCHAR(200)
                SELECT @constraintName = name
                FROM sys.default_constraints
                WHERE parent_object_id = Object_ID('ThongBaos')
                AND parent_column_id = (
                    SELECT column_id 
                    FROM sys.columns
                    WHERE Name = 'MaCongDan'
                    AND Object_ID = Object_ID('ThongBaos')
                )
                
                IF @constraintName IS NOT NULL
                    EXEC('ALTER TABLE ThongBaos DROP CONSTRAINT ' + @constraintName)
                
                -- Now alter the column
                ALTER TABLE ThongBaos
                ALTER COLUMN MaCongDan INT NULL;
                
                PRINT 'Column MaCongDan made nullable.';
            END
            ELSE
            BEGIN
                PRINT 'Column MaCongDan already nullable or check completed.';
            END";
            
            command.ExecuteNonQuery();
            Console.WriteLine("ThongBaos table update check completed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating ThongBaos table: {ex.Message}");
    }
    finally
    {
        if (connection.State == System.Data.ConnectionState.Open)
            connection.Close();
    }
}

// Add BangChungKhieuNais table if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var connection = dbContext.Database.GetDbConnection();
    
    try
    {
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BangChungKhieuNais')
            BEGIN
                CREATE TABLE [dbo].[BangChungKhieuNais] (
                    [MaBangChung] INT IDENTITY(1,1) NOT NULL,
                    [MaKhieuNai] INT NOT NULL,
                    [DuongDanFile] NVARCHAR(MAX) NULL,
                    [MoTa] NVARCHAR(500) NULL,
                    [NgayTao] DATETIME2 NOT NULL,
                    [NguoiTao] NVARCHAR(100) NULL,
                    CONSTRAINT [PK_BangChungKhieuNais] PRIMARY KEY ([MaBangChung]),
                    CONSTRAINT [FK_BangChungKhieuNais_KhieuNaiVPhams_MaKhieuNai] FOREIGN KEY ([MaKhieuNai]) 
                    REFERENCES [KhieuNaiVPhams] ([MaKhieuNai]) ON DELETE CASCADE
                );

                CREATE INDEX [IX_BangChungKhieuNais_MaKhieuNai] ON [dbo].[BangChungKhieuNais] ([MaKhieuNai]);
                
                PRINT 'BangChungKhieuNais table created successfully.';
            END
            ELSE
            BEGIN
                PRINT 'BangChungKhieuNais table already exists.';
            END";
            
            command.ExecuteNonQuery();
            Console.WriteLine("BangChungKhieuNais table check completed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error checking/adding BangChungKhieuNais table: {ex.Message}");
    }
    finally
    {
        if (connection.State == System.Data.ConnectionState.Open)
            connection.Close();
    }
}

// Add the LoaiKhieuNai and related columns to KhieuNaiVPhams table if they don't exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var connection = dbContext.Database.GetDbConnection();
    
    try
    {
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            IF NOT EXISTS (
                SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'KhieuNaiVPhams' AND COLUMN_NAME = 'LoaiKhieuNai'
            )
            BEGIN
                ALTER TABLE KhieuNaiVPhams
                ADD LoaiKhieuNai NVARCHAR(50) NOT NULL DEFAULT 'ThanhToan';
                
                PRINT 'Column LoaiKhieuNai added successfully.';
            END
            ELSE
            BEGIN
                PRINT 'Column LoaiKhieuNai already exists.';
            END";
            
            command.ExecuteNonQuery();
            
            // Add LyDoYeuCau column
            command.CommandText = @"
            IF NOT EXISTS (
                SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'KhieuNaiVPhams' AND COLUMN_NAME = 'LyDoYeuCau'
            )
            BEGIN
                ALTER TABLE KhieuNaiVPhams
                ADD LyDoYeuCau NVARCHAR(MAX) NULL;
                
                PRINT 'Column LyDoYeuCau added successfully.';
            END";
            
            command.ExecuteNonQuery();
            
            // Add GhiChu column
            command.CommandText = @"
            IF NOT EXISTS (
                SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'KhieuNaiVPhams' AND COLUMN_NAME = 'GhiChu'
            )
            BEGIN
                ALTER TABLE KhieuNaiVPhams
                ADD GhiChu NVARCHAR(MAX) NULL;
                
                PRINT 'Column GhiChu added successfully.';
            END";
            
            command.ExecuteNonQuery();
            
            // Add DuongDanBangChung column
            command.CommandText = @"
            IF NOT EXISTS (
                SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'KhieuNaiVPhams' AND COLUMN_NAME = 'DuongDanBangChung'
            )
            BEGIN
                ALTER TABLE KhieuNaiVPhams
                ADD DuongDanBangChung NVARCHAR(MAX) NULL;
                
                PRINT 'Column DuongDanBangChung added successfully.';
            END";
            
            command.ExecuteNonQuery();
            
            Console.WriteLine("KhieuNaiVPhams columns check completed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error checking/adding KhieuNaiVPhams columns: {ex.Message}");
    }
    finally
    {
        if (connection.State == System.Data.ConnectionState.Open)
            connection.Close();
    }
}

// Add another using block to create a test citizen account
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    // Create a test citizen user
    var citizenEmail = "citizen@test.com";
    var citizenUser = await userManager.FindByEmailAsync(citizenEmail);
    
    if (citizenUser == null)
    {
        // Create the citizen user
        citizenUser = new ApplicationUser
        {
            UserName = "citizen",
            Email = citizenEmail,
            EmailConfirmed = true,
            HoTen = "Công Dân Test",
            PhoneNumber = "0987654321"
        };
        
        var result = await userManager.CreateAsync(citizenUser, "Test@123");
        
        if (result.Succeeded)
        {
            // Ensure the CongDan role exists
            if (!await roleManager.RoleExistsAsync("CongDan"))
            {
                await roleManager.CreateAsync(new IdentityRole("CongDan"));
            }
            
            // Assign CongDan role
            await userManager.AddToRoleAsync(citizenUser, "CongDan");
            
            // Create a citizen record in the CongDan table
            var congDan = new CongDan
            {
                TenCongDan = "Công Dân Test",
                DiaChi = "123 Test Street",
                SDT = "0987654321",
                GMAIL = citizenEmail,
                GIOTTING = "Nam",
                CONGVIEC = "Test Engineer",
                QuocTich = "Việt Nam",
                NgaySinh = new DateTime(1990, 1, 1),
                MaNguoiDung = citizenUser.Id,
                MaTheCC = "079123456789" // Assign a test ID card
            };
            
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.CongDans.Add(congDan);
            await dbContext.SaveChangesAsync();
            
            Console.WriteLine("Test citizen user created successfully.");
        }
        else
        {
            Console.WriteLine($"Failed to create test citizen user. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}

// Add a using block to check if TheCanCuocs table exists and create it if needed
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var connection = dbContext.Database.GetDbConnection();
    
    try
    {
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TheCanCuocs')
            BEGIN
                CREATE TABLE [dbo].[TheCanCuocs] (
                    [MaTheCC] NVARCHAR(450) NOT NULL,
                    [TenCC] NVARCHAR(MAX) NOT NULL,
                    [NgayCap] DATETIME2 NULL,
                    [NgayHetHan] DATETIME2 NULL,
                    [NoiCap] NVARCHAR(MAX) NULL,
                    [GhiChu] NVARCHAR(MAX) NULL,
                    CONSTRAINT [PK_TheCanCuocs] PRIMARY KEY ([MaTheCC])
                );
                
                PRINT 'TheCanCuocs table created successfully.';
            END
            ELSE
            BEGIN
                -- Check if all required columns exist and add them if they don't
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TheCanCuocs' AND COLUMN_NAME = 'NgayCap')
                BEGIN
                    ALTER TABLE TheCanCuocs ADD NgayCap DATETIME2 NULL;
                    PRINT 'Column NgayCap added to TheCanCuocs table.';
                END
                
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TheCanCuocs' AND COLUMN_NAME = 'NoiCap')
                BEGIN
                    ALTER TABLE TheCanCuocs ADD NoiCap NVARCHAR(MAX) NULL;
                    PRINT 'Column NoiCap added to TheCanCuocs table.';
                END
                
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TheCanCuocs' AND COLUMN_NAME = 'GhiChu')
                BEGIN
                    ALTER TABLE TheCanCuocs ADD GhiChu NVARCHAR(MAX) NULL;
                    PRINT 'Column GhiChu added to TheCanCuocs table.';
                END
                
                PRINT 'TheCanCuocs table already exists. Verified columns.';
            END";
            
            command.ExecuteNonQuery();
            Console.WriteLine("TheCanCuocs table check completed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error checking/creating TheCanCuocs table: {ex.Message}");
    }
    finally
    {
        if (connection.State == System.Data.ConnectionState.Open)
            connection.Close();
    }
}

// Add a using block to seed TheCanCuoc data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Check if we already have ID card data
    if (!dbContext.TheCanCuocs.Any())
    {
        Console.WriteLine("Seeding TheCanCuoc data...");
        
        // Create some ID cards
        dbContext.TheCanCuocs.AddRange(
            new TheCanCuoc
            {
                MaTheCC = "079123456789",
                TenCC = "Căn cước công dân",
                NgayCap = DateTime.Parse("2020-01-01"),
                NgayHetHan = DateTime.Parse("2030-01-01"),
                NoiCap = "Cục Cảnh sát quản lý hành chính về trật tự xã hội",
                GhiChu = "Thẻ mẫu"
            },
            new TheCanCuoc
            {
                MaTheCC = "079198765432",
                TenCC = "Căn cước công dân",
                NgayCap = DateTime.Parse("2021-06-15"),
                NgayHetHan = DateTime.Parse("2031-06-15"),
                NoiCap = "Cục Cảnh sát quản lý hành chính về trật tự xã hội",
                GhiChu = "Thẻ mẫu"
            }
        );
        
        await dbContext.SaveChangesAsync();
        Console.WriteLine("TheCanCuoc data seeded successfully.");
    }
    else
    {
        Console.WriteLine("TheCanCuoc data already exists. Skipping seed.");
    }
}

app.Run();

// Role seeding method
async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
{
    // Create all required roles
    string[] roles = { "Admin", "CanBo", "CongDan", "User" };
    
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// Admin user seeding method
async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
{
    // Create admin user if it doesn't exist
    var adminEmail = "admin@quanlyvipham.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    
    if (adminUser == null)
    {
        // Create the admin user
        adminUser = new ApplicationUser
        {
            UserName = "admin",
            Email = adminEmail,
            EmailConfirmed = true,
            HoTen = "Quản trị viên",
            PhoneNumber = "0123456789"
        };
        
        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        
        if (result.Succeeded)
        {
            // Ensure the Admin role exists
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            
            // Assign admin role
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
