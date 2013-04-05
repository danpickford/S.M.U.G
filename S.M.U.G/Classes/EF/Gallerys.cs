using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S.M.U.G.Classes.EF
{
    class Gallerys
    {
        [Key]
        public int RowId { get; set; }
        public string GalleryName { get; set; }
        public int UserId { get; set; }
        public virtual Muguser Muguser { get; set; }
    }
}
