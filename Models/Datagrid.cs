using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Movies.Models
{
    //Datagrid of selected movies satisfying the search criteria.  Contains Movie name and Description
    public class Datagrid
    {
        public string MovieTitle { get; set; }

        [Key]
        public int MovieID { get; set; }
        public string Description { get; set; }
        public List<Movie> MovieTitles;

        public IEnumerator<Movie> GetEnumerator()
        {
            return MovieTitles.GetEnumerator();
        }

        public Datagrid()
        {
            MovieTitle = "";
            MovieID = 0;
            Description = "";
        }
    }

}