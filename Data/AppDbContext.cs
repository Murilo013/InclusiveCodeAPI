using InclusiveCode.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace InclusiveCode.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<AnalysisResult> AnalysisResults { get; set; }
    }
}
