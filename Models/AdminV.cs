using System.Collections.Generic;

namespace Movies.Models
{
    public class AdminV
    {
        public AdminV() { }
        public string strTab { get; set; }
        public string strName { get; set; }
        public string strPos { get; set; }
        public string strCat { get; set; }
        public string strAddMsg { get; set; }
        public bool blnMergeName { get; set; }
        public int intNameID { get; set; }
        public int intPosID { get; set; }
        public int intCatID { get; set; }
        public List<NamesLU> Names;
        public List<PositionsLU> Pos;
        public List<CategoryLU> Cats;
        public PositionsLU Positions { get; private set; }
        public CategoryLU Categories { get; private set; }
        public static SortingPagingInfo infoName { get; set; }
        public static SortingPagingInfo infoPos { get; set; }
        public static SortingPagingInfo infoCat { get; set; }
        public bool TriggerOnLoad { get; set; }

        public AdminV(List<NamesLU> NamesLst)
        {
            Names = NamesLst;
            TriggerOnLoad = false;
            strName = "";
            intNameID = 0;
            blnMergeName = false;
        }

        public AdminV(List<PositionsLU> PosLst)
        {
            Pos = PosLst;
            TriggerOnLoad = false;
            strPos = "";
            intPosID = 0;
        }

        public AdminV(List<CategoryLU> CatsLst)
        {
            Cats = CatsLst;
            TriggerOnLoad = false;
            strCat = "";
            intCatID = 0;
        }
    }
}