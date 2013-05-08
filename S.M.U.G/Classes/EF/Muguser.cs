using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S.M.U.G.Classes.EF
{
    internal class Muguser
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string OAuthToken { get; set; }
        public string OAuthSecret { get; set; }
        public string SyncFolder { get; set; }
        public virtual List<Gallerys> Gallerys { get; set; }
    }
}