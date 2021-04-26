using System.Collections.Generic;

namespace Movies.Models
{
    public class MovieV
    {
        public MovieV() { }

        public static DataGridParams dgParams { get; set; }
        public NamesLU Names { get; set; }
        public PositionsLU Positions { get; private set; }
        public CategoryLU Categories { get; private set; }
        public IEnumerable<string> SelectedCategories { get; set; }
        public Movie Movies { get; set; }
        public MovieCategory MovieCategory { get; set; }
        public MovieNamesPosition MovieNamesPosition { get; private set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Category { get; set; }
        //below are fields to display on view
        public string strMovieTitleSrch { get; set; }
        public string strActorName { get; set; }
        public string strCrewName { get; set; }
        public string strCrewTitle { get; set; }
        public string strCategory { get; set; }
        public string strSelectedCategories { get; set; }
        public string strDescription { get; set; }
        public int intCrewTitleID { get; set; }
        public int intCategoryID { get; set; }
        public int MovieID { get; set; }


        public MovieV(Movie MovieTitles)
        {
            Movies = MovieTitles;
            Names = new NamesLU();
            Positions = new PositionsLU();
            Categories = new CategoryLU();
            Name = "";
            Position = "";
            Category = "";
            strMovieTitleSrch = null;
            strActorName = null;
            strCrewName = null;
            strCrewTitle = null;
            strDescription = null;
            strCategory = "";
            intCrewTitleID = 0;
            intCategoryID = 0;
            MovieID = 0;
        }
    }
}