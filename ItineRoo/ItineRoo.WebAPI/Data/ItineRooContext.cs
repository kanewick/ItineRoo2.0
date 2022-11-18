using ItineRoo.WebAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ItineRoo.WebAPI.Data
{
    public class ItineRooContext : DbContext
    {
        public ItineRooContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users => Set<User>();

        public void Save() => SaveChanges();
    }
}
