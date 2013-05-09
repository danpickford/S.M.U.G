using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S.M.U.G.Classes.EF
{
    class SmugContexts : DbContext
    {
        public DbSet<Muguser> Mugusers { get; set; }
        public DbSet<Gallerys> Gallerys { get; set; }
    }
}
