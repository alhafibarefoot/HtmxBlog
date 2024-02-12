using HtmxBlog.Models;
using Microsoft.EntityFrameworkCore;

namespace HtmxBlog.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Post> Posts => Set<Post>();
    }
}
