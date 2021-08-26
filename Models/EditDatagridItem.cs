using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movies.Models
{
    public class EditDatagridItem
    {
        public int id { get; set; }
        public string urlpath { get; set; }
        public string strMovieTitle { get; set; }
        public string strDescription { get; set; }
        public string strMovieTitleSrch { get; set; }
        public string strDescSrch { get; set; }
        public string strActorNameSrch { get; set; }
        public string strCrewNameSrch { get; set; }
        public int intCrewTitleIDSrch { get; set; }
        public string strCrewTitleSrch { get; set; }
        public string strSelectedCategoriesSrch { get; set; }
        public string SortFieldFromPrev { get; set; }
        public string SortDirectionFromPrev { get; set; }
        public int PageCountFromPrev { get; set; }
        public int PageSizeFromPrev { get; set; }
        public int CurrentPageIndexFromPrev { get; set; }
        public string strCategory { get; set; }
        public string FromEdit { get; set; }
    }
}