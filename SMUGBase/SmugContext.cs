using System.Data.Entity;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SMUGBase
{
    public class SmugContexts : DbContext
    {
        public DbSet<Muguser> Mugusers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Album> Albums { get; set; }

        public Muguser RetrieveMugUser()
        {
            var loggedUser = Mugusers.FirstOrDefault(x => x.LoggedUser == 1);
            return loggedUser;
        }

        public void insertCategoriesAndAlbumsFromJSON(string jsonstring)
        {
            var jsonObject = JObject.Parse(jsonstring);
            foreach (var category in jsonObject["Albums"].GroupBy(s => (string)s.SelectToken("Category.Name")).ToList())
            {
                CheckInsertCategory(category.Key, Mugusers.First(x => x.LoggedUser == 1).UserId);
                foreach (var albums in jsonObject.SelectToken("Albums").Where(s => (string)s.SelectToken("Category.Name") == category.Key).Select(ss => ss.SelectToken("Title")).ToList())
                {
                    CheckInsertAlbum(albums.ToString(), Categories.First(x => x.CategoryName == category.Key));
                }
            }
        }

        private void CheckInsertCategory(string _categoryName, int UserId)
        {
            if (!Categories.Any(x => x.CategoryName == _categoryName))
            {
                var newCategory = new Category
                {
                    CategoryName = _categoryName,
                    UserId = UserId
                };
                Categories.Add(newCategory);
                SaveChanges();
            }
        }
        
        private void CheckInsertAlbum(string _albumName, Category _category)
        {
            if (!Albums.Any(x => x.AlbumName == _albumName && x.Category.RowId == _category.RowId))
            {
                var newAlbum = new Album
                    {
                        AlbumName = _albumName,
                        Category = _category
                    };
                Albums.Add(newAlbum);
                SaveChanges();
            }
        }

    }

}
