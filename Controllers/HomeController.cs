using Microsoft.Office.Interop.Excel;
using Movies.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Movies.Controllers
{
    //The columns in the spreadsheet
    public struct MovieStruct
    {
        public string Title;
        public string Actors;
        public string Crew;
        public string Description;
    }

    [HandleError(View = "Error")]
    public class HomeController : Controller
    {
        private const int SQLConnection = 2;
        private static Movie mv = new Movie();
        //private static MovieCategory mvCat = new MovieCategory();
        //private static MovieNamesPosition mvNamesPosition = new MovieNamesPosition();
        private MovieDBContext db = new MovieDBContext();
        private MovieV mvV = new MovieV(mv);
        IList<MovieV> listmvV = new List<MovieV>();
        IList<Datagrid> dg = new List<Datagrid>();
        private IEnumerable<CategoryLU> Categorys = new List<CategoryLU>();
        private IEnumerable<NamesLU> NamesLU = new List<NamesLU>();
        private IEnumerable<PositionsLU> Positions = new List<PositionsLU>();
        private static SortingPagingInfo info = new SortingPagingInfo();
        private static DataGridParams dgParams = new DataGridParams();
        public MovieStruct[] arrMovies;
        const int MOVIE_CNT = 2000;

        public ActionResult Index()
        {
            //var model = db.MovieV;
            return View();
        }

        public ActionResult MovieSrchAndEdit()
        {
            ViewBag.CrewTitle = "";
            ViewBag.Category = "";
            fillPositionDdl();
            fillCategoryDdl();
            return View();
        }

        public ActionResult MovieImport()
        {
            string strLine;
            string[] arrMovie;
            int i = 0;
            int intMovieRecs = 0;
            ArrayList arrMovieTitles = new ArrayList();
            ArrayList arrMovieActors = new ArrayList();
            ArrayList arrMovieCrew = new ArrayList();
            ArrayList arrMovieDesc = new ArrayList();
            System.Data.DataTable tblMovieTitle = new System.Data.DataTable();

            arrMovies = new MovieStruct[MOVIE_CNT];
            arrMovie = new string[5];

            try
            {   // Open the text file using a stream reader.  The commas in the spreadsheet were changed to semicolons in order to create a csv file
                using (StreamReader sr = new StreamReader(Server.MapPath("~/App_Data/Movies.csv")))
                {
                    strLine = sr.ReadLine();

                    while (strLine != null)
                    {
                        arrMovie = strLine.Split(new[] { ',' });
                        //columns of the spreadsheet
                        arrMovies[i].Title = arrMovie[0];
                        arrMovies[i].Actors = arrMovie[1];
                        arrMovies[i].Crew = arrMovie[2];
                        arrMovies[i].Description = arrMovie[3];
                        i++;
                        strLine = sr.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                ViewData["Message"] = "ERROR in Home.MovieImport: " + e.Message;
                return View("Error");
            }

            DataColumn colMovieID = new DataColumn("MovieID", typeof(int));
            tblMovieTitle.Columns.Add(colMovieID);
            DataColumn colMovieTitle = new DataColumn("MovieTitle", typeof(string));
            tblMovieTitle.Columns.Add(colMovieTitle);

            for (i = 0; i < MOVIE_CNT; i++)
                if (("" + arrMovies[i].Title) != "")
                {
                    arrMovieTitles.Add(arrMovies[i].Title);
                    DataRow row = tblMovieTitle.NewRow();
                    row[tblMovieTitle.Columns[0]] = i;
                    row[tblMovieTitle.Columns[1]] = arrMovies[i].Title;
                    tblMovieTitle.Rows.Add(row);
                }
                else
                    break;

            intMovieRecs = i;
            System.Data.DataTable tblActors = new System.Data.DataTable();
            DataColumn colActorID = new DataColumn("ActorID", typeof(int));
            tblActors.Columns.Add(colActorID);
            DataColumn colActorName = new DataColumn("ActorName", typeof(string));
            tblActors.Columns.Add(colActorName);

            for (i = 0; i < MOVIE_CNT; i++)
            {
                arrMovieActors.Add("" + arrMovies[i].Actors);
                DataRow row = tblActors.NewRow();
                row[tblActors.Columns[0]] = i;
                row[tblActors.Columns[1]] = "" + arrMovies[i].Actors;
                tblActors.Rows.Add(row);
            }

            System.Data.DataTable tblCrew = new System.Data.DataTable();
            DataColumn colCrewID = new DataColumn("CrewID", typeof(int));
            tblCrew.Columns.Add(colCrewID);
            DataColumn colCrewName = new DataColumn("CrewName", typeof(string));
            tblCrew.Columns.Add(colCrewName);

            for (i = 0; i < MOVIE_CNT; i++)
            {
                arrMovieCrew.Add("" + arrMovies[i].Crew);
                DataRow row = tblCrew.NewRow();
                row[tblCrew.Columns[0]] = i;
                row[tblCrew.Columns[1]] = "" + arrMovies[i].Crew;
                tblCrew.Rows.Add(row);
            }

            System.Data.DataTable tblDesc = new System.Data.DataTable();
            DataColumn colDescID = new DataColumn("DescID", typeof(int));
            tblDesc.Columns.Add(colDescID);
            DataColumn colDesc = new DataColumn("Description", typeof(string));
            tblDesc.Columns.Add(colDesc);

            for (i = 0; i < MOVIE_CNT; i++)
            {
                arrMovieDesc.Add("" + arrMovies[i].Description);
                DataRow row = tblDesc.NewRow();
                row[tblDesc.Columns[0]] = i;
                row[tblDesc.Columns[1]] = "" + arrMovies[i].Description;
                tblDesc.Rows.Add(row);
            }

            for (i = 0; i < tblMovieTitle.Rows.Count; i++)
            {
                //Sets up the tables so each movie record has it's own record as well as foreign table records for it's actors, crew members such as director or writer, and it's associated movie categories
                ParseAndTransfer(tblMovieTitle, tblActors, tblCrew, tblDesc, i);
            }

            ViewData["Message"] = "Import Complete.  Records Processed: " + intMovieRecs.ToString();
            ViewData["Movies"] = arrMovies;
            ViewData["MoviesDB"] = tblMovieTitle;
            return View();
        }

        //dropdown for positions such as actor, director
        private void fillPositionDdl()
        {
            Positions = PositionsLU.GetPositions().ToList();
            ViewBag.intCrewTitleID = new SelectList(Positions, "PositionsLUID", "Position");
        }

        //dropdown for movie categories such as mystery, western
        private void fillCategoryDdl()
        {
            Categorys = CategoryLU.GetCategories().ToList();
            ViewBag.intCategoryID = new SelectList(Categorys, "CategoryLUID", "CategoryName");
        }

        public ActionResult ExportToExcel()
        {
            int intPrevMovieID = 0, intRow = 3;
            var lstMovies = new List<Tuple<string, string, string, string, string>> { };
            Microsoft.Office.Interop.Excel.Application oXL;
            string strMovieTitle = "", strDesc = "", strCat = "";
            StringBuilder sbActors = new StringBuilder();
            StringBuilder sbCrew = new StringBuilder();
            _Workbook oWB = null;
            _Worksheet oWS = null;

            var qryMovieList = db.Database.SqlQuery<MovieToExport>("exec dbo.[spSQLServerToExcel]").ToList();
            foreach (var m in qryMovieList)
            {
                if (intPrevMovieID != 0 && m.MovieID != intPrevMovieID)
                {
                    AddMovieAndCleanup(ref sbActors, ref sbCrew, ref lstMovies, strMovieTitle, strDesc, strCat);
                }

                if (m.Actor != null)
                    sbActors.Append(m.Actor + ";");
                else if (m.Crew != null)
                    sbCrew.Append(m.Crew + ";");

                if (intPrevMovieID != m.MovieID)
                {
                    intPrevMovieID = m.MovieID;

                    if (strMovieTitle != m.MovieTitle)
                        strMovieTitle = m.MovieTitle;
                    if (strDesc != m.Description)
                        strDesc = m.Description;
                }

                strCat = MovieCats(intPrevMovieID);
            }

            AddMovieAndCleanup(ref sbActors, ref sbCrew, ref lstMovies, strMovieTitle, strDesc, strCat);
            oXL = new Microsoft.Office.Interop.Excel.Application();
            oWB = oXL.Workbooks.Add(Type.Missing);
            oWB = oXL.Workbooks.Open(System.Web.Configuration.WebConfigurationManager.AppSettings["ExcelFile"]);
            //oWS = (_Worksheet)oWB.ActiveSheet;
            //oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
            oWS = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
            oWS.EnableCalculation = false;
            oWS.Cells[1, 1] = "Movie";
            oWS.Cells[1, 2] = "Actors";
            oWS.Cells[1, 3] = "Crew";
            oWS.Cells[1, 4] = "Description";
            oWS.Cells[1, 5] = "Category";

            foreach (var lst in lstMovies)
            {
                oWS.Cells[intRow, 1] = ("" + lst.Item1).ToString();
                oWS.Cells[intRow, 2] = ("" + lst.Item2).ToString();
                oWS.Cells[intRow, 3] = ("" + lst.Item3).ToString();
                oWS.Cells[intRow, 4] = ("" + lst.Item4).ToString();
                oWS.Cells[intRow++, 5] = ("" + lst.Item5).ToString();
                //try timer instead of button - Running javascript from c#
                //ScriptManager.RegisterClientScriptBlock(Button1, Button1.GetType(), "Hello", "alert('Hello World');", true);
            }

            oWB.Save();
            oWB.Close();
            oXL.Quit();
            ViewBag.Message = "Export Complete.  Records Processed: " + qryMovieList.Count.ToString();
            return View("MovieImport");
        }

        private void AddMovieAndCleanup(ref StringBuilder sbActors,
            ref StringBuilder sbCrew,
            ref List<Tuple<string, string, string, string, string>> lstMovies,
            string strMovieTitle,
            string strDesc,
            string strCat)
        {
            if (sbActors.ToString().Length > 0)
                sbActors.Remove(sbActors.ToString().Length - 1, 1);
            if (sbCrew.ToString().Length > 0)
                sbCrew.Remove(sbCrew.ToString().Length - 1, 1);
            if (strCat.ToString().Length > 0)
                strCat = strCat.Remove(strCat.Length - 1, 1);

            lstMovies.Add(new Tuple<string, string, string, string, string>(strMovieTitle, sbActors.ToString(), sbCrew.ToString(), strDesc, strCat));
            sbActors.Clear();
            sbCrew.Clear();
        }

        private string MovieCats(int intPrevMovieID)
        {
            StringBuilder sbCats = new StringBuilder();
            var qryMovieCats = db.Database.SqlQuery<CategoryLU>(
                    "exec dbo.[spGetMovieCats] @MovieID",
                        new SqlParameter("MovieID", intPrevMovieID)).ToList();

            foreach (var cat in qryMovieCats)
            {
                sbCats.Append(cat.CategoryName + ";");
            }

            return sbCats.ToString();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Edit()
        {
            return View("Edit", "Movies");
        }

        [HttpGet]
        public ActionResult Search()
        {
            MovieV mv = new MovieV();
            return View(mv);
        }

        [HttpGet]
        public ActionResult Help()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(MovieV mvV)
        {
            bool blnFirstTime = true;
            StringBuilder CatsToQry = new StringBuilder();
            StringBuilder CatsNameToQry = new StringBuilder();
            string CatsToQryStr, CatsNameToQryStr = "";
            //string strDblQuote = "\"";

            if (mvV.SelectedCategories != null && mvV.SelectedCategories.ToList()[0] != "0")
            {
                List<string> CategoriesList = mvV.SelectedCategories.ToList();

                for (int i = 0; i < mvV.SelectedCategories.Count(); i++)
                {
                    if (i == 0)
                        CatsToQry.Append("(");

                    CatsToQry.Append(CategoriesList[i] + ", ");

                    if (i == (mvV.SelectedCategories.Count() - 1))
                    {
                        CatsToQry = CatsToQry.Remove(CatsToQry.Length - 2, 2);
                        CatsToQry.Append(")");
                    }
                }

                CatsToQryStr = CatsToQry.ToString();
            }
            else
                CatsToQryStr = "";

            if (CatsToQryStr == "(0)")
                CatsToQryStr = "";

            //Search the database for the movies satisfying the search criteria
            mvV.strSelectedCategories = CatsToQryStr.ToString();
            var qryMovieList = db.Database.SqlQuery<Movie>(
                "exec dbo.[spSearchMovies] @strMovieTitle, @strDescription, @strActorName, @strCrewName, @intCrewTitleID, @CategoryListStr",
                    new SqlParameter("strMovieTitle", (object)mvV.strMovieTitleSrch ?? DBNull.Value),
                    new SqlParameter("strDescription", (object)mvV.strDescription ?? DBNull.Value),
                    new SqlParameter("strActorName", (object)mvV.strActorName ?? DBNull.Value),
                    new SqlParameter("strCrewName", (object)mvV.strCrewName ?? DBNull.Value),
                    new SqlParameter("intCrewTitleID", mvV.intCrewTitleID),
                    new SqlParameter("CategoryListStr", CatsToQryStr)
            ).ToList();
            qryMovieList = qryMovieList.OrderBy(mv => mv.MovieTitle).ToList();

            //Datagrid setup
            info.Init(10, "ascending", "MovieTitle", qryMovieList.Count());
            //Starting datagrid values
            dgParams.Init(mvV.strMovieTitleSrch, mvV.strDescription, mvV.strActorName, mvV.strCrewName, mvV.intCrewTitleID, mvV.strCrewTitle, mvV.strSelectedCategories, mvV.strCategory, 10, "ascending", "MovieTitle", qryMovieList.Count(), "N");
            ViewBag.SortingPagingInfo = info;
            db.Database.Connection.Close();

            if (CatsToQryStr != "")
            {
                var qryCatList = db.Database.SqlQuery<CategoryLU>(
                    "exec dbo.[spGetCats] @CategoryListStr",
                        new SqlParameter("CategoryListStr", CatsToQryStr)
                ).ToList();

                foreach (var c in qryCatList)
                {
                    CatsNameToQry.Append(c.CategoryName + ", ");
                }

                if (CatsNameToQry.Length > 0)
                    CatsNameToQryStr = CatsNameToQry.ToString().Substring(0, CatsNameToQry.ToString().Length - 2);
            }

            foreach (var mv in qryMovieList)
            {
                if (blnFirstTime)
                {
                    dg.Add(new Datagrid() { MovieTitles = qryMovieList, MovieID = mv.MovieID, MovieTitle = mv.MovieTitle, Description = mv.Description });
                    blnFirstTime = false;
                }
                else
                    dg.Add(new Datagrid() { MovieID = mv.MovieID, MovieTitle = mv.MovieTitle, Description = mv.Description });
            }

            dg = dg.Take(info.PageSize).ToList();
            if (mvV.intCrewTitleID > 0)
            {
                var strCrewTitle = from plu in db.PositionsLU
                                   where plu.PositionsLUID == mvV.intCrewTitleID
                                   select new { plu.Position };

                foreach (var sct in strCrewTitle)
                {
                    mvV.strCrewTitle = sct.Position;
                }
            }

            Session["SortDirection"] = "ascending";
            Session["CurrentPageIndex"] = info.CurrentPageIndex;
            mvV.strCategory = CatsNameToQryStr;
            ViewBag.MovieSrchVars = mvV;
            mv.dg = dg;
            ViewBag.Movie = mv;
            dgParams.strCrewTitleSrch = mvV.strCrewTitle;
            dgParams.strMovieTitleSrch = mvV.strMovieTitleSrch;
            dgParams.strDescSrch = mvV.strDescription;
            dgParams.strActorNameSrch = mvV.strActorName;
            dgParams.strCrewNameSrch = mvV.strCrewName;
            dgParams.strCrewTitleSrch = mvV.strCrewTitle;
            MovieV.dgParams = dgParams;

            //MovieV.dgParams.strMovieTitleSrch = mvV.strMovieTitleSrch;
            //MovieV.dgParams.strActorNameSrch = mvV.strActorName;
            //MovieV.dgParams.strCrewNameSrch = mvV.strCrewName;
            //MovieV.dgParams.strCrewTitleSrch = mvV.strCrewTitle;
            ViewBag.DataGridParams = dgParams;
            return View("Datagrid", dg);
        }

        [HttpPost]
        //Sorting and paging through the datagrid
        public ActionResult Datagrid(MovieV mvV, SortingPagingInfo info, DataGridParams dgParams, MovieDBContext db)
        {
            FillTheGrid(mvV, info, dgParams, db);
            return View("Datagrid", dg);
        }

        public void FillTheGrid(MovieV mvV, SortingPagingInfo info, DataGridParams dgParams, MovieDBContext db)
        {
            bool blnFirstTime = true;

            if (dgParams.strSelectedCategoriesSrch == null)
                dgParams.strSelectedCategoriesSrch = "";
            var qryMovieList = db.Database.SqlQuery<Movie>(
                "exec dbo.[spSearchMovies] @strMovieTitle, @strDescription, @strActorName, @strCrewName, @intCrewTitleID, @CategoryListStr",
                    new SqlParameter("strMovieTitle", (object)dgParams.strMovieTitleSrch ?? DBNull.Value),
                    new SqlParameter("strDescription", (object)dgParams.strDescSrch ?? DBNull.Value),
                    new SqlParameter("strActorName", (object)dgParams.strActorNameSrch ?? DBNull.Value),
                    new SqlParameter("strCrewName", (object)dgParams.strCrewNameSrch ?? DBNull.Value),
                    new SqlParameter("intCrewTitleID", dgParams.intCrewTitleIDSrch),
                    new SqlParameter("CategoryListStr", dgParams.strSelectedCategoriesSrch)).ToList();

            ViewBag.Movie = mv;
            ViewBag.MovieSrchVars = mvV;
            info.PageSize = 10;
            var qryMovieListOrder = new List<Movie>();

            if (info.SortDirection == "ascending")
            {
                qryMovieListOrder = qryMovieList.OrderBy(mv => mv.MovieTitle).ToList();
                info.SortDirection = "ascending";
            }
            else
            {
                qryMovieListOrder = qryMovieList.OrderByDescending(mv => mv.MovieTitle).ToList();
                info.SortDirection = "descending";
            }

            info.SortField = "MovieTitle";
            //How many rows per datagrid page?
            info.PageCount = info.GetPgCnt(qryMovieList.Count(), info.PageSize);
            ViewBag.SortingPagingInfo = info;
            //datagrid values before the sort or paging took place
            dgParams.Init(mvV.strMovieTitleSrch, mvV.strDescription, mvV.strActorName, mvV.strCrewName, mvV.intCrewTitleID, mvV.strCrewTitle,
                          mvV.strSelectedCategories, mvV.strCategory, info.PageSize, info.SortDirection, info.SortField, qryMovieListOrder.Count(), "N");
            mvV.strActorName = "" + dgParams.strActorNameSrch;
            mvV.strDescription = "" + dgParams.strDescSrch;
            mvV.strCrewName = "" + dgParams.strCrewNameSrch;
            mvV.strCategory = "" + mvV.strCategory;
            mvV.strCrewTitle = "" + mvV.strCrewTitle;
            mvV.strMovieTitleSrch = "" + dgParams.strMovieTitleSrch;
            mvV.strSelectedCategories = "" + dgParams.strSelectedCategoriesSrch;
            dgParams.SortDirectionFromPrev = info.SortDirection;
            dgParams.SortFieldFromPrev = info.SortField;
            dgParams.strSelectedCategoriesSrch = "" + dgParams.strSelectedCategoriesSrch;
            dgParams.CurrentPageIndexFromPrev = info.CurrentPageIndex;
            ViewBag.DataGridParams = dgParams;
            db.Database.Connection.Close();

            foreach (var mv in qryMovieListOrder)
            {
                if (blnFirstTime)
                {
                    dg.Add(new Datagrid() { MovieTitles = qryMovieList, MovieID = mv.MovieID, MovieTitle = mv.MovieTitle, Description = mv.Description });
                    blnFirstTime = false;
                }
                else
                    dg.Add(new Datagrid() { MovieID = mv.MovieID, MovieTitle = mv.MovieTitle, Description = mv.Description });
            }

            dg = dg.Skip(Convert.ToInt32(info.CurrentPageIndex) * info.PageSize).Take(info.PageSize).ToList();
            Session["CurrentPageIndex"] = info.CurrentPageIndex;
        }


        //Sets up the tables so each movie record has it's own record as well as foreign table records for it's actors, crew members such as director or writer, and it's associated movie categories
        private void ParseAndTransfer(System.Data.DataTable tblMovieTitle, System.Data.DataTable tblActors, System.Data.DataTable tblCrew, System.Data.DataTable tblDesc, int intRow)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SQLConnection].ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                }
                catch (Exception e)
                {
                    ViewData["Message"] = "ERROR: " + e.Message;
                    return;
                }

                cmd.CommandText = "spFillMovieTables";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@MovieTitle", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@ActorStr", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@CrewStr", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@CatAndDesc", SqlDbType.VarChar, 255);
                cmd.Parameters["@MovieTitle"].Value = tblMovieTitle.Rows[intRow][1].ToString().Replace("\"", "");
                cmd.Parameters["@ActorStr"].Value = tblActors.Rows[intRow][1].ToString().Replace("\"", "");
                cmd.Parameters["@CrewStr"].Value = tblCrew.Rows[intRow][1].ToString().Replace("\"", "");
                cmd.Parameters["@CatAndDesc"].Value = tblDesc.Rows[intRow][1].ToString().Replace("\"", "");
                cmd.CommandTimeout = 30;
                cmd.Prepare();

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    ViewData["Message"] = "ERROR: " + e.Message;
                    return;
                }
                finally
                {
                    cmd.Dispose();
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

    }
}