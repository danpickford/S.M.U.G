using System.ComponentModel.DataAnnotations;

namespace SMUGBase
{
    public class Category
    {
        [Key]
        public int RowId { get; set; }
        public string CategoryName { get; set; }
        public int UserId { get; set; }
        public virtual Muguser Muguser { get; set; }
    }
}
