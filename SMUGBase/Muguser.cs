using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMUGBase
{
    public class Muguser
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string OAuthToken { get; set; }
        public string OAuthTokenSecret { get; set; }
        public string SyncFolder { get; set; }
        public int LoggedUser { get; set; }
        public virtual List<Gallerys> Gallerys { get; set; }
    }
}