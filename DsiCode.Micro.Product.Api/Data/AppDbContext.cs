using Microsoft.EntityFrameworkCore;

namespace DsiCode.Micro.Product.Api.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
            
        }

        public DbSet<DsiCode.Micro.Product.Api.Models.Product> Productos { get; set; }

        
    }
}
