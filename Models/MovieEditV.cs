using System.Collections.Generic;

namespace Movies.Models
{
    public class MovieEditV
    {
        public MovieEditV() { }

        public NamesLU Names { get; set; }
        public PositionsLU Positions { get; private set; }
        public CategoryLU Categories { get; private set; }
        public IEnumerable<string> SelectedCategories { get; set; }
        public List<Movie> lstMovie;
        public List<MovieNamesPosition> NamesAndTitles;
        public List<CategoryLU> SelectedCategoriesList { get; set; }

        public IEnumerator<MovieNamesPosition> GetEnumerator()
        {
            return NamesAndTitles.GetEnumerator();
        }

        public static DataGridParams dgParams { get; set; }
        public static SortingPagingInfo info { get; set; }
        public Movie Movies { get; set; }
        public MovieCategory MovieCategory { get; set; }
        public virtual ICollection<MovieNamesPosition> MovieNamesPosition { get; set; }
        //below are fields to display on view
        public string strPerson { get; set; }
        public string strPosition { get; set; }
        public string Category { get; set; }
        public string strMovieTitle { get; set; }
        public string strActorName { get; set; }
        public string strCrewName { get; set; }
        public string strCrewTitle { get; set; }
        public string strCategory { get; set; }
        public string strSelectedCategories { get; set; }
        public string strDesc { get; set; }
        public int intCrewTitleID { get; set; }
        public int intCategoryID { get; set; }
        public int MovieID { get; set; }
        public int NameID { get; set; }
        public int PositionID { get; set; }
        public bool blnMovieAdded { get; set; }
        public bool TriggerOnLoad { get; set; }

        public MovieEditV(Movie MovieTitles)
        {
            Movies = MovieTitles;
            Names = new NamesLU();
            Names.Name = "";
            Positions = new PositionsLU();
            Positions.Position = "";
            Categories = new CategoryLU();
            strDesc = "";
            strMovieTitle = "";
            strPerson = "";
            strPosition = "";
            strCategory = "";
            Category = "";
            MovieID = 0;
            NameID = 0;
            PositionID = 0;
            intCategoryID = 0;
        }
    }
}
