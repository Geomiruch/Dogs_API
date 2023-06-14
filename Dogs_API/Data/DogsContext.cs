using Dogs_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dogs_API.Data
{
    public class DogsContext : DbContext
    {
        public DogsContext(DbContextOptions<DogsContext> options) : base(options) { }

        public DbSet<Dog> Dogs { get; set; }
    }
}
