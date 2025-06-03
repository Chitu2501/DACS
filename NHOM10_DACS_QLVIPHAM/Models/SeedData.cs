using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Check if we already have data
                if (context.TheCanCuocs.Any())
                {
                    return; // DB has been seeded
                }

                // Create some ID cards
                context.TheCanCuocs.AddRange(
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

                // Save changes to the database
                await context.SaveChangesAsync();
            }
        }
    }
} 