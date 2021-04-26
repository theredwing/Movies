using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Movies.Models
{
    public class DatagridCat
    {
        public string MovieTitle { get; set; }
        public string Category { get; set; }
        public CategoryLU Categories { get; private set; }
        public IEnumerable<string> SelectedCategories { get; set; }
        public List<CategoryLU> SelectedCategoriesList { get; set; }
        public List<MovieCategory> MovieCats;

        [Key]
        public int MovieID { get; set; }
        public int CategoryID { get; set; }
        public List<MovieCatsAll> MovieTitles;

        public IEnumerator<MovieCategory> GetEnumerator()
        {
            return MovieCats.GetEnumerator();
        }

        public DatagridCat()
        {
            MovieTitle = "";
            MovieID = 0;
            CategoryID = 0;
            Category = "";
        }
    }
}