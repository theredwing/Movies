namespace Movies.Models
{
    public class DataGridParams
    {
        //When the user leaves a page, sometimes datagrid data will need to be saved such as current sort field and direction as well as the current page of the datagrid
        public string strMovieTitleSrch { get; set; }
        public string strDescSrch { get; set; }
        public string strActorNameSrch { get; set; }
        public string strCrewNameSrch { get; set; }

        public string strCrewTitleSrch { get; set; }

        public int intCrewTitleIDSrch { get; set; }
        public string strSelectedCategoriesSrch { get; set; }
        public string strCategorySrch { get; set; }
        public string SortFieldFromPrev { get; set; }
        public string SortDirectionFromPrev { get; set; }
        public string xAction { get; set; }
        public int PageSizeFromPrev { get; set; }
        public int PageCountFromPrev { get; set; }
        public int CurrentPageIndexFromPrev { get; set; }

        public string FromEdit { get; set; }
        private SortingPagingInfo info = new SortingPagingInfo();

        public void Init(string strMTSrch, string strDSCSrch, string strANSrch, string strCNSrch, int intCTIDSrch, string strCrewTitSrch, string strSelCatSrch, string strCatSrch, int intPageSz, string strSortDir,
                         string strSortFld, int intRecCnt, string FEdit)
        {
            strMovieTitleSrch = strMTSrch;
            strDescSrch = strDSCSrch;
            strActorNameSrch = strANSrch;
            strCrewNameSrch = strCNSrch;
            intCrewTitleIDSrch = intCTIDSrch;
            strCrewTitleSrch = strCrewTitSrch;
            strSelectedCategoriesSrch = "" + strSelCatSrch;
            strCategorySrch = "" + strCatSrch;
            PageSizeFromPrev = intPageSz;
            //Initial sort direction needs to be ascending so we need to flip it
            SortDirectionFromPrev = strSortDir;
            SortFieldFromPrev = strSortFld;
            PageCountFromPrev = info.GetPgCnt(intRecCnt, intPageSz);
            FromEdit = FEdit;
        }

        public void Init(int intPageSz, string strSortDir, string strSortFld, int intRecCnt, string FEdit)
        {
            PageSizeFromPrev = intPageSz;
            //Initial sort direction needs to be ascending so we need to flip it
            SortDirectionFromPrev = strSortDir;
            SortFieldFromPrev = strSortFld;
            PageCountFromPrev = info.GetPgCnt(intRecCnt, intPageSz);
            FromEdit = FEdit;
        }
    }
}