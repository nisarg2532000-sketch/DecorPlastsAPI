using Microsoft.EntityFrameworkCore;
using OtpAPI.Models;
namespace OtpAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<OtpEntity> OtpVerifications { get; set; }
    }
}
