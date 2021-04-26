using System.Collections.Generic;

namespace Movies.Models
{
    public class DataGridCategories
    {
        public string Category { get; set; }
        public int CategoryLUID { get; set; }
        public List<CategoryLU> Categories;

        public IEnumerator<CategoryLU> GetEnumerator()
        {
            return Categories.GetEnumerator();
        }
    }
}