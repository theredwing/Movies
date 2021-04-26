namespace Movies.Models
{
    public class SortingPagingInfo
    {
        public string SortField { get; set; }
        public string SortDirection { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int CurrentPageIndex { get; set; }
        //If the user arrives to a page with a datagrid, the count is one.  If they then select a datagrid page and then sort, then the count will be three.  Therefore, if the user desires to return to the
        //previous page it will do a windows.gohistory(-3)
        public int PagesAhead { get; set; }
        //What action did the user request:  Datagrid of sorting or paging?  Saving/Canceling of data on a page
        public string EditPageAction { get; set; }

        public void Init(int intPageSz, string strSortDir, string strSortFld, int intRecCnt)
        {
            PageSize = intPageSz;
            //Initial sort direction needs to be ascending so we need to flip it
            SortDirection = strSortDir;
            SortField = strSortFld;
            EditPageAction = "";
            PageCount = GetPgCnt(intRecCnt, intPageSz);
        }

        //Only so many records allowed per page on a datagrid
        public int GetPgCnt(int intMovieCnt, int intPgSz)
        {
            int intVal, intRem;

            intVal = (intMovieCnt / intPgSz);
            intRem = (intMovieCnt % intPgSz);

            if (intRem != 0)
                return (intVal + 1);
            else
                return (intVal);
        }
    }
}