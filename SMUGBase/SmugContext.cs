using System.Data.Entity;

namespace SMUGBase
{
    public class SmugContexts : DbContext
    {
        public DbSet<Muguser> Mugusers { get; set; }
        public DbSet<Gallerys> Gallerys { get; set; }
    }
}
