using System.ComponentModel.DataAnnotations;

namespace SMUGBase
{
    public class Gallerys
    {
        [Key]
        public int RowId { get; set; }
        public string GalleryName { get; set; }
        public int UserId { get; set; }
        public virtual Muguser Muguser { get; set; }
    }
}
