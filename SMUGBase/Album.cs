using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUGBase
{
    public class Album
    {
        [Key]
        public int RowId { get; set; }
        public string AlbumName { get; set; }
        public virtual Category Category { get; set; }
    }
}
