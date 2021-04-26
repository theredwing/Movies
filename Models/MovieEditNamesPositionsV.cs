using System.Collections.Generic;

namespace Movies.Models
{
    public class MovieEditNamesPositionsV
    {
        public MovieEditNamesPositionsV() { }
        public MovieNamesPosition MovieNamesPositions { get; set; }
        //public string ErrMsg { get; set; }
        public string strPerson { get; set; }
        public string strPosition { get; set; }
        public string xAction { get; set; }
        public int MovieID { get; set; }
        public int NamessID { get; set; }
        public int PositionsID { get; set; }
        public bool TriggerOnLoad { get; set; }
        //public bool btnSavePressed { get; set; }
        //public string TriggerOnLoadMessage { get; set; }
        public List<MovieNamesPosition> NamesAndTitles;
        public static DataGridParams dgParamsENP { get; set; }
        public static SortingPagingInfo infoENP { get; set; }

        public MovieEditNamesPositionsV(MovieNamesPosition MNP)
        {
            MovieNamesPositions = MNP;
            strPerson = "";
            strPosition = "";
            xAction = "";
            PositionsID = 1;
            NamessID = 1;
        }
    }


}