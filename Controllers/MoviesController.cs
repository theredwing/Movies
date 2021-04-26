using Movies.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Movies.Controllers
{
    public class MoviesController : Controller
    {
        private const int SQLConnection = 2;
        private const string EDIT = "e";
        private MovieDBContext db = new MovieDBContext();
        private static Movie mv = new Movie();
        private static MovieCatsAll mvCataAll = new MovieCatsAll();
        private static MovieNamesPosition MNP = new MovieNamesPosition();
        private MovieV mvV = new MovieV(mv);
        private static SortingPagingInfo info = new SortingPagingInfo();
        private static SortingPagingInfo infoENP = new SortingPagingInfo();
        private static DataGridParams dgParams = new DataGridParams();
        private static MovieEditNamesPositionsV menpV = new MovieEditNamesPositionsV(MNP);

        private static MovieEditV mEv = new MovieEditV(mv);
        IList<DataGridNamesPosition> dgMNP = new List<DataGridNamesPosition>();
        IList<Movie> mvList = new List<Movie>();
        IList<Datagrid> dg = new List<Datagrid>();
        IList<DatagridCat> dgCat = new List<DatagridCat>();
        private IEnumerable<CategoryLU> Categorys = new List<CategoryLU>();
        private IEnumerable<PositionsLU> Positions = new List<PositionsLU>();
        private IEnumerable<NamesLU> Names = new List<NamesLU>();

        public ActionResult Index()
        {
            return View(db.Movies.ToList());
        }

        // GET: Movies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // GET: Movies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MovieID,MovieTitle")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movie);
        }

        //Get
        public ActionResult DeleteMovie(int? xid)
        {
            var args = new object[1];
            DataSet dataset;

            if (xid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                ViewBag.Movie = mv;
                ViewBag.MovieSrchVars = mvV;
                ViewBag.DataGridParams = MovieV.dgParams;
                ViewBag.SortingPagingInfo = info;
                dataset = new DataSet();
                args = new object[] { xid };
                MovieDBContext.LoadDataSet(ref dataset, "spDelMovie", args);
                return View("~/Views/Home/Datagrid.cshtml", dg);
            }
        }

        // GET: Movies/Edit/5
        // Click on the movie link in the datagrid to edit the selected record
        public ActionResult EditNamesPositions(int? xid, string position, int namesID, int positionID, int? MovieNamesPositionID, MovieEditNamesPositionsV menpV, DataGridParams dgParamsENP,
                                               SortingPagingInfo infoENP, string Delete, string Add, string strTitle, string strDesc)
        {
            var args = new object[1];
            var qryMovieOne = new List<Movie>();
            DataSet dataset;
            Errors Err = new Errors();
            Err.Message = "";

            if (xid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (xid > 0)
            {
                dataset = new DataSet();
                args = new object[] { mv.MovieID, strTitle, strDesc };
                MovieDBContext.LoadDataSet(ref dataset, "spUpdateMovie", args);
            }

            if (Delete == "y")
            {
                dataset = new DataSet();
                args = new object[] { MovieNamesPositionID };
                MovieDBContext.LoadDataSet(ref dataset, "spDelMovieNamePos", args);

                qryMovieOne = db.Database.SqlQuery<Movie>(
                    "exec dbo.[spGetMovie] @MovieID",
                        new SqlParameter("MovieID", mv.MovieID)).ToList();
                mEv.strMovieTitle = qryMovieOne[0].MovieTitle;
                mEv.strDesc = qryMovieOne[0].Description;
                ViewBag.Movie = mv;
                ViewBag.DataGridParams = dgParams;
                ViewBag.DataGridParamsENP = dgParamsENP;
                ViewBag.SortingPagingInfo = info;
                CatsForMovie(mEv);

                var qryMovieList = db.Database.SqlQuery<MovieNamesPosition>(
                    "exec dbo.[spMovieNamesTitles] @MovieID",
                    new SqlParameter("MovieID", mv.MovieID)).ToList();
                mEv.NamesAndTitles = qryMovieList.Take(info.PageSize).ToList();
                mEv.TriggerOnLoad = false;
                return View("~/Views/Movies/Edit.cshtml", mEv);
            }

            if (xid > 0)
            {
                mv = db.Movies.Find(xid);
                if (mv == null)
                {
                    return HttpNotFound();
                }

                var qryMovieList = db.Database.SqlQuery<MovieNamesPosition>(
                    "exec dbo.[spMovieNamesTitles] @MovieID",
                    new SqlParameter("MovieID", mv.MovieID)).ToList();
                //Datagrid for the movie:  Actors, director
                infoENP.Init(5, "ascending", "Name", qryMovieList.Count());
                qryMovieList = qryMovieList.OrderBy(mv => mv.Name).ToList();
                menpV.NamesAndTitles = qryMovieList.Take(infoENP.PageSize).ToList();
            }
            else
            {
                mEv.TriggerOnLoad = true;
                mEv.strMovieTitle = strTitle;
                mEv.strDesc = strDesc;
                mEv.MovieID = mv.MovieID;
                CatsForMovie(mEv);
                ViewData["Message"] = "Please Save the Movie before adding a Name/Position!";
                ViewBag.Movie = mv;
                ViewBag.DataGridParams = dgParams;
                ViewBag.DataGridParamsENP = dgParamsENP;
                ViewBag.SortingPagingInfo = info;
                return View("~/Views/Movies/Edit.cshtml", mEv);
            }

            if (MovieEditV.dgParams == null)
                MovieEditV.dgParams = dgParams;
            if (MovieEditV.info == null)
                MovieEditV.info = info;
            if (MovieEditNamesPositionsV.dgParamsENP == null)
            {
                MovieEditNamesPositionsV.dgParamsENP = MovieEditV.dgParams;
            }

            if (MovieEditNamesPositionsV.infoENP == null)
                MovieEditNamesPositionsV.infoENP = info;

            fillPositionDdl();
            fillNamesDdl();
            ViewBag.Movie = mv;

            if (Add == "a")
            {
                menpV.NamessID = 0;
                menpV.PositionsID = 0;
                dgParamsENP.xAction = Add;
                menpV.xAction = Add;
                MNP.MovieID = mv.MovieID;
                menpV.NamesAndTitles = null;
                MNP.MovieNamesPositionID = 0;
            }
            else
            {
                menpV.NamessID = namesID;
                menpV.PositionsID = positionID;
                dgParamsENP.xAction = EDIT;
                menpV.xAction = EDIT;
                MNP.MovieID = mv.MovieID;
                MNP.MovieNamesPositionID = Convert.ToInt32(MovieNamesPositionID);
            }

            ViewBag.SortingPagingInfo = MovieEditV.info;
            ViewBag.SortingPagingInfoENP = infoENP;
            ViewBag.DataGridParams = MovieEditV.dgParams;
            ViewBag.DataGridParamsENP = dgParamsENP;
            ViewBag.Errors = Err;
            ViewBag.Edit = mEv;
            menpV.MovieID = mv.MovieID;

            return View(menpV);
        }

        [HttpPost]
        public ActionResult EditNamesPositions(SortingPagingInfo info, MovieEditNamesPositionsV menpV, SortingPagingInfo infoENP, DataGridParams dgParamsENP)
        {
            Errors Err = new Errors();
            Err.Message = "";


            if (dgParamsENP.xAction == "Cancel")
            {
                CatsForMovie(mEv);
                mEv.strMovieTitle = mv.MovieTitle;
                mEv.strDesc = mv.Description;
                ViewBag.Movie = mv;
                ViewBag.DataGridParams = dgParams;
                ViewBag.DataGridParamsENP = dgParamsENP;
                ViewBag.SortingPagingInfo = info;
                mEv.TriggerOnLoad = false;
                return View("~/Views/Movies/Edit.cshtml", mEv);
            }

            fillPositionDdl();
            fillNamesDdl();
            menpV.TriggerOnLoad = false;

            var qryMovieListOrder = new List<MovieNamesPosition>();
            var qryMovieOne = new List<Movie>();
            var qryMovieList = db.Database.SqlQuery<MovieNamesPosition>(
                "exec dbo.[spMovieNamesTitles] @MovieID",
                    new SqlParameter("MovieID", mEv.MovieID)).ToList();
            mEv.NamesAndTitles = qryMovieList.Skip(Convert.ToInt32(info.CurrentPageIndex) * info.PageSize).Take(info.PageSize).ToList();
            mEv.TriggerOnLoad = false;

            if (dgParamsENP.xAction == "e")
            {
                var qryNameExist = db.Database.SqlQuery<NamesStr>(
                    "exec dbo.[spCheckNameAndUpdate] @MovieNamesPositionID, @MovieID, @NamesID, @NewNamesID, @PositionsID, @NewPositionsID ",
                        new SqlParameter("MovieNamesPositionID", MNP.MovieNamesPositionID),
                        new SqlParameter("MovieID", MNP.MovieID),
                        new SqlParameter("NamesID", MNP.NamesID),
                        new SqlParameter("NewNamesID", menpV.NamessID),
                        new SqlParameter("PositionsID", MNP.PositionID),
                        new SqlParameter("NewPositionsID", menpV.PositionsID)
                        ).ToList();

                foreach (var q in qryNameExist)
                    Err.Message = q.Msg.ToString();
            }
            else
            {
                if (menpV.NamessID > 0 && menpV.PositionsID > 0)
                {
                    var dataset = new DataSet();
                    var args = new object[] { mEv.MovieID, menpV.NamessID, menpV.PositionsID };
                    MovieDBContext.LoadDataSet(ref dataset, "spInsertMovieNamePos", args);
                    Err.Message = dataset.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    qryMovieOne = db.Database.SqlQuery<Movie>(
                        "exec dbo.[spGetMovie] @MovieID",
                            new SqlParameter("MovieID", mv.MovieID)).ToList();
                    mEv.strMovieTitle = qryMovieOne[0].MovieTitle;
                    mEv.strDesc = qryMovieOne[0].Description;
                    mEv.MovieID = qryMovieOne[0].MovieID;
                    CatsForMovie(mEv);
                    ViewBag.Movie = mv;
                    ViewBag.DataGridParams = dgParams;
                    ViewBag.DataGridParamsENP = dgParamsENP;
                    ViewBag.SortingPagingInfo = info;
                    mEv.TriggerOnLoad = false;
                    return View("~/Views/Movies/Edit.cshtml", mEv);
                }
            }

            ViewBag.Movie = mv;
            ViewBag.DataGridParams = dgParams;
            ViewBag.DataGridParamsENP = dgParamsENP;
            ViewBag.SortingPagingInfo = info;

            if (Err.Message.Length > 0)
            {
                infoENP.PagesAhead++;
                ViewBag.SortingPagingInfoENP = infoENP;
                menpV.TriggerOnLoad = true;
                ViewBag.Errors = Err;
                menpV.MovieID = mv.MovieID;
                mEv.blnMovieAdded = false;
                ViewBag.Edit = mEv;
                return View(menpV);
            }
            else
            {
                mEv.TriggerOnLoad = false;
                var qryMovieNamePosList = db.Database.SqlQuery<MovieNamesPosition>(
                    "exec dbo.[spMovieNamesTitles] @MovieID",
                        new SqlParameter("MovieID", mv.MovieID)).ToList();
                qryMovieNamePosList = qryMovieNamePosList.OrderBy(mv => mv.Name).ToList();

                if (info.PageSize == 0)
                {
                    info.Init(5, "ascending", "Name", qryMovieNamePosList.Count());
                    qryMovieOne = db.Database.SqlQuery<Movie>(
                        "exec dbo.[spGetMovie] @MovieID",
                            new SqlParameter("MovieID", mv.MovieID)).ToList();
                    mEv.strMovieTitle = qryMovieOne[0].MovieTitle;
                    mEv.strDesc = qryMovieOne[0].Description;
                }
                else
                {
                    mEv.strMovieTitle = mv.MovieTitle;
                    mEv.strDesc = mv.Description;
                }

                CatsForMovie(mEv);
                mEv.NamesAndTitles = qryMovieNamePosList.Take(info.PageSize).ToList();
                ViewBag.NamesAndTitles = mEv.NamesAndTitles;
                mEv.TriggerOnLoad = false;
                return View("~/Views/Movies/Edit.cshtml", mEv);
            }
        }

        // GET: Movies/Edit/5
        // Click on the movie link in the datagrid to edit the selected record
        public ActionResult Edit(int? id, DataGridParams dgParams, DataGridParams dgParamsENP)
        {
            if (id == 0)
            {
                fillPositionDdl();
                fillCategoryDdl();
                if (mv == null)
                    mv = new Movie();

                mv.MovieID = 0;
                ViewBag.Movie = mv;
                mEv.MovieID = 0;
                mEv.strMovieTitle = "";
                mEv.strDesc = "";
                mEv.NamesAndTitles = null;
                mEv.SelectedCategories = null;
                info.Init(5, "ascending", "Name", 0);
                ViewBag.SortingPagingInfo = info;
                ViewData["SaveOrCancel"] = 0;
                ViewData["Message"] = "";
                Assign_dgParams(dgParams);
                mEv.TriggerOnLoad = false;
                return View(mEv);
            }
            else
            {
                mv = db.Movies.Find(id);
                if (mv == null)
                {
                    return HttpNotFound();
                }

                ViewBag.Movie = mv;
                mEv.MovieID = mv.MovieID;
                mEv.strMovieTitle = mv.MovieTitle;
                mEv.strDesc = mv.Description;

                var qryMovieList = db.Database.SqlQuery<MovieNamesPosition>(
                    "exec dbo.[spMovieNamesTitles] @MovieID",
                        new SqlParameter("MovieID", mEv.MovieID)).ToList();

                //Datagrid for the movie:  Actors, director
                info.Init(5, "ascending", "Name", qryMovieList.Count());
                qryMovieList = qryMovieList.OrderBy(mv => mv.Name).ToList();
                mEv.NamesAndTitles = qryMovieList.Take(info.PageSize).ToList();
            }

            CatsForMovie(mEv);
            if (MovieEditV.dgParams == null)
                MovieEditV.dgParams = dgParams;
            if (MovieEditV.info == null)
                MovieEditV.info = info;

            ViewBag.NamesAndTitles = mEv.NamesAndTitles;
            db.Database.Connection.Close();
            ViewBag.SortingPagingInfo = info;
            dgParams.strCategorySrch = mvV.strCategory;
            ViewBag.DataGridParams = dgParams;
            MovieV.dgParams.strCategorySrch = mvV.strCategory;
            ViewData["SaveOrCancel"] = 0;
            ViewData["Message"] = "";
            mEv.TriggerOnLoad = false;
            return View(mEv);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        // The data for the movie will display in a sortable, pageable datagrid
        public ActionResult Edit(SortingPagingInfo info, int id, int? CatId, MovieEditV mEv, DataGridParams dgParams, DataGridParams dgParamsENP, MovieCategory mc)
        {
            var qryMovieName = new List<Movie>();
            var dataset = new DataSet();
            var args = new object[1];
            var qryMovieListOrder = new List<MovieNamesPosition>();
            var qryMovieList = db.Database.SqlQuery<MovieNamesPosition>(
                "exec dbo.[spMovieNamesTitles] @MovieID",
                    new SqlParameter("MovieID", id)).ToList();
            mEv.NamesAndTitles = qryMovieList.Skip(Convert.ToInt32(info.CurrentPageIndex) * info.PageSize).Take(info.PageSize).ToList();

            if (id > 0)
                mEv.MovieID = id;
            else
                mEv.MovieID = mv.MovieID;

            CatsForMovie(mEv);

            //Sorting and paging through the datagrid of Names and positions for the movie
            if (info.EditPageAction == "Sort" || info.EditPageAction == "Page")
            {
                bool blnFirstTime = true;
                db.Database.Connection.Close();

                if (info.SortDirection == "ascending")
                {
                    if (info.SortField == "Name")
                    {
                        qryMovieListOrder = qryMovieList.OrderBy(mv => mv.Name).ToList();
                    }
                    else
                    {
                        qryMovieListOrder = qryMovieList.OrderBy(mv => mv.Position).ToList();
                    }

                    info.SortDirection = "ascending";
                }
                else
                {
                    if (info.SortField == "Name")
                    {
                        qryMovieListOrder = qryMovieList.OrderByDescending(mv => mv.Name).ToList();
                    }
                    else
                    {
                        qryMovieListOrder = qryMovieList.OrderByDescending(mv => mv.Position).ToList();
                    }

                    info.SortDirection = "descending";
                }

                foreach (var mv in qryMovieListOrder)
                {
                    if (blnFirstTime)
                    {
                        dgMNP.Add(new DataGridNamesPosition() { Names = qryMovieList, MovieID = mv.MovieID, Name = mv.Name, Position = mv.Position });
                        blnFirstTime = false;
                    }
                    else
                        dgMNP.Add(new DataGridNamesPosition() { MovieID = mv.MovieID, Name = mv.Name, Position = mv.Position });
                }

                mEv.NamesAndTitles = qryMovieListOrder.Skip(Convert.ToInt32(info.CurrentPageIndex) * info.PageSize).Take(info.PageSize).ToList();
                ViewBag.NamesAndTitles = mEv.NamesAndTitles;
                ViewData["SaveOrCancel"] = 0;
                ViewBag.Movie = mv;
                ViewBag.SortingPagingInfo = info;
                Assign_dgParams(dgParams);
                ViewData["Message"] = "";
                mEv.TriggerOnLoad = false;
                return View(mEv);
            }
            //add category for movie
            else if (info.EditPageAction == "Add")
            {
                if (mv.MovieID == 0)
                {
                    mEv.MovieID = 0;
                    mEv.SelectedCategoriesList = null;
                }

                ViewData["SaveOrCancel"] = 0;
                ViewData["Message"] = "";
                ViewBag.Movie = mv;
                ViewBag.SortingPagingInfo = info;
                Assign_dgParams(dgParams);

                if (mEv.intCategoryID == 0)
                {
                    mEv.TriggerOnLoad = true;
                    ViewData["Message"] = "Please Select a Category!";
                    return View(mEv);
                }
                else
                {
                    if (mEv.SelectedCategoriesList != null)
                    {
                        foreach (var cat in mEv.SelectedCategoriesList)
                        {
                            if (cat.CategoryLUID == mEv.intCategoryID)
                            {
                                mEv.TriggerOnLoad = true;
                                ViewData["Message"] = "This Category has already been Selected!";
                                return View(mEv);
                            }
                        }
                    }

                    if (mv.MovieID == 0)
                    {
                        if (("" + mEv.strMovieTitle).Trim() == "")
                        {
                            mEv.TriggerOnLoad = true;
                            ViewData["Message"] = "Categories Cannot be Entered Until a Movie Title is!";
                            return View(mEv);
                        }
                        else
                        {
                            if (mEv.blnMovieAdded)
                                ;
                            else
                            {
                                if (mv.MovieID > 0)
                                    ;
                                else
                                {
                                    if (mv.MovieID == 0)
                                    {
                                        dataset = new DataSet();
                                        args = new object[] { mEv.strMovieTitle, mEv.strDesc };
                                        MovieDBContext.LoadDataSet(ref dataset, "spAddMovie", args);
                                        mv.MovieID = Convert.ToInt32(dataset.Tables[0].Rows[0][0]);

                                        qryMovieName = db.Database.SqlQuery<Movie>(
                                            "exec dbo.[spGetMovie] @MovieID",
                                                new SqlParameter("MovieID", mv.MovieID)).ToList();

                                        mEv.strMovieTitle = qryMovieName[0].MovieTitle;
                                        mEv.strDesc = qryMovieName[0].Description;
                                        mEv.MovieID = mv.MovieID;
                                        mEv.blnMovieAdded = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        qryMovieList = db.Database.SqlQuery<MovieNamesPosition>(
                            "exec dbo.[spMovieNamesTitles] @MovieID",
                                new SqlParameter("MovieID", mv.MovieID)).ToList();
                        info.Init(10, "ascending", "Name", qryMovieList.Count());
                        mEv.NamesAndTitles = qryMovieList.Skip(Convert.ToInt32(info.CurrentPageIndex) * info.PageSize).Take(info.PageSize).ToList();
                    }

                    dataset = new DataSet();
                    args = new object[] { mv.MovieID, mEv.intCategoryID };
                    MovieDBContext.LoadDataSet(ref dataset, "spAddCatToMovie", args);
                    CatsForMovie(mEv);
                    ViewBag.Movie = mv;
                    mEv.TriggerOnLoad = false;
                    return View(mEv);
                }
            }
            else if (info.EditPageAction == "Delete")
            {
                ViewData["SaveOrCancel"] = 0;
                ViewData["Message"] = "";
                ViewBag.Movie = mv;
                ViewBag.SortingPagingInfo = info;
                Assign_dgParams(dgParams);

                if (CatId == null)
                {
                    mEv.TriggerOnLoad = true;
                    ViewData["Message"] = "Select Category to Delete!";
                    return View(mEv);
                }
                else
                {
                    mc.MovieCategoryId = System.Convert.ToInt16(CatId);
                    dataset = new DataSet();
                    args = new object[] { mv.MovieID, mc.MovieCategoryId };
                    MovieDBContext.LoadDataSet(ref dataset, "spDelMovieCat", args);
                    CatsForMovie(mEv);
                    mEv.TriggerOnLoad = false;
                    return View(mEv);
                }
            }
            else
            {
                //Save movie title and description
                if (info.EditPageAction == "Save")
                {
                    if (("" + mEv.strMovieTitle).Trim() == "")
                    {
                        mEv.TriggerOnLoad = true;
                        ViewBag.Movie = mv;
                        ViewBag.SortingPagingInfo = info;
                        Assign_dgParams(dgParams);
                        ViewData["Message"] = "Please enter a Movie Title!";
                        return View(mEv);
                    }
                }

                if (info.EditPageAction == "Save" && id != 0)
                {
                    dataset = new DataSet();
                    args = new object[] { mv.MovieID, mEv.strMovieTitle, mEv.strDesc };
                    MovieDBContext.LoadDataSet(ref dataset, "spUpdateMovie", args);
                }
                else if ((info.EditPageAction == "Cancel" || info.EditPageAction == "Save") && id == 0)
                {
                    if (id == 0 && info.EditPageAction == "Save")
                    {
                        if (mEv.blnMovieAdded)
                            ;
                        else
                        {
                            if (mv.MovieID > 0)
                                ;
                            else
                            {
                                dataset = new DataSet();
                                args = new object[] { mEv.strMovieTitle, mEv.strDesc };
                                MovieDBContext.LoadDataSet(ref dataset, "spAddMovie", args);
                                mv.MovieID = Convert.ToInt32(dataset.Tables[0].Rows[0][0]);

                                var qryMovieOne = db.Database.SqlQuery<Movie>(
                                    "exec dbo.[spGetMovie] @MovieID",
                                        new SqlParameter("MovieID", mv.MovieID)).ToList();

                                mv.MovieTitle = qryMovieOne[0].MovieTitle;
                                mv.Description = qryMovieOne[0].Description;
                                mEv.blnMovieAdded = true;
                            }
                        }
                    }

                    fillPositionDdl();
                    fillCategoryDdl();
                    ViewBag.intCrewTitleID = ViewBag.vbPosID;
                    ViewBag.vbSelectedCategoriesList = null;
                    return View("~/Views/Home/MovieSrchAndEdit.cshtml");
                }

                bool blnFirstTime = true;

                if (MovieV.dgParams == null)
                    MovieV.dgParams = new DataGridParams();

                if (MovieV.dgParams.strSelectedCategoriesSrch == null)
                    MovieV.dgParams.strSelectedCategoriesSrch = "";

                var qryMovieList2 = db.Database.SqlQuery<Movie>(
                    "exec dbo.[spSearchMovies] @strMovieTitle, @strDescription, @strActorName, @strCrewName, @intCrewTitleID, @CategoryListStr",
                        new SqlParameter("strMovieTitle", (object)MovieV.dgParams.strMovieTitleSrch ?? DBNull.Value),
                        new SqlParameter("strDescription", (object)MovieV.dgParams.strDescSrch ?? DBNull.Value),
                        new SqlParameter("strActorName", (object)MovieV.dgParams.strActorNameSrch ?? DBNull.Value),
                        new SqlParameter("strCrewName", (object)MovieV.dgParams.strCrewNameSrch ?? DBNull.Value),
                        new SqlParameter("intCrewTitleID", MovieV.dgParams.intCrewTitleIDSrch),
                        new SqlParameter("CategoryListStr", MovieV.dgParams.strSelectedCategoriesSrch)).ToList();

                foreach (var mv in qryMovieList2)
                {
                    if (blnFirstTime)
                    {
                        dg.Add(new Datagrid() { MovieTitles = qryMovieList2, MovieID = mv.MovieID, MovieTitle = mv.MovieTitle, Description = mv.Description });
                        blnFirstTime = false;
                    }
                    else
                        dg.Add(new Datagrid() { MovieID = mv.MovieID, MovieTitle = mv.MovieTitle, Description = mv.Description });
                }

                mvV.strSelectedCategories = dgParams.strSelectedCategoriesSrch;
                Session["CurrentPageIndex"] = info.CurrentPageIndex;
                ViewBag.Movie = mv;
                mvV.intCrewTitleID = MovieV.dgParams.intCrewTitleIDSrch;
                mvV.strDescription = MovieV.dgParams.strDescSrch;
                mvV.strActorName = MovieV.dgParams.strActorNameSrch;
                mvV.strCrewName = MovieV.dgParams.strCrewNameSrch;
                mvV.strCrewTitle = MovieV.dgParams.strCrewTitleSrch;
                ViewBag.MovieSrchVars = mvV;
                ViewBag.NamesAndTitles = mEv.NamesAndTitles;
                ViewBag.DataGridParams = MovieV.dgParams;
                info.SortField = MovieV.dgParams.SortFieldFromPrev;
                info.SortDirection = MovieV.dgParams.SortDirectionFromPrev;
                info.PageCount = MovieV.dgParams.PageCountFromPrev;
                info.PageSize = MovieV.dgParams.PageSizeFromPrev;
                info.CurrentPageIndex = MovieV.dgParams.CurrentPageIndexFromPrev;
                dg = dg.Skip(Convert.ToInt32(info.CurrentPageIndex) * info.PageSize).Take(info.PageSize).ToList();
                ViewBag.SortingPagingInfo = info;
                ViewData["SaveOrCancel"] = 1;
                infoENP.Init(10, "ascending", "Name", qryMovieList.Count());

                ////If saving or canceling, control transfers to a previous page
                if (info.EditPageAction == "Cancel")
                {
                    mEv.TriggerOnLoad = false;
                    ViewData["Message"] = "";
                    return View("~/Views/Home/Datagrid.cshtml", dg);
                }
                else if (mEv.strMovieTitle == null && info.EditPageAction == "Save")
                {
                    mEv.TriggerOnLoad = true;
                    ViewData["Message"] = "Movie is a Required Field!";
                    return View(mEv);
                }
                else
                {
                    mEv.TriggerOnLoad = false;
                    return View("~/Views/Home/Datagrid.cshtml", dg);
                }
            }
        }

        private void Assign_dgParams(DataGridParams dgParams)
        {
            dgParams.strCrewTitleSrch = "" + dgParams.strCrewTitleSrch;
            dgParams.strDescSrch = "" + dgParams.strDescSrch;
            dgParams.strActorNameSrch = "" + dgParams.strActorNameSrch;
            dgParams.strCategorySrch = "" + dgParams.strCategorySrch;
            dgParams.strCrewNameSrch = "" + dgParams.strCrewNameSrch;
            dgParams.strMovieTitleSrch = "" + dgParams.strMovieTitleSrch;
            dgParams.strSelectedCategoriesSrch = "" + dgParams.strSelectedCategoriesSrch;
            ViewBag.DataGridParams = dgParams;
        }

        private void CatsForMovie(MovieEditV mEv)
        {
            fillCategoryDdl();
            var qryMovieCats = db.Database.SqlQuery<CategoryLU>(
                    "exec dbo.[spGetMovieCats] @MovieID",
                        new SqlParameter("MovieID", mEv.MovieID)).ToList();
            mEv.SelectedCategoriesList = qryMovieCats.ToList();
            ViewBag.vbSelectedCategoriesList = new SelectList(mEv.SelectedCategoriesList, "CategoryLUID", "CategoryName");
        }

        //dropdown for positions such as actor, director
        private void fillPositionDdl()
        {
            Positions = PositionsLU.GetPositionsAll().ToList();
            ViewBag.vbPosID = new SelectList(Positions, "PositionsLUID", "Position");
        }

        //dropdown for movie categories such as mystery, western
        private void fillCategoryDdl()
        {
            Categorys = CategoryLU.GetCategories().ToList();
            ViewBag.intCategoryID = new SelectList(Categorys, "CategoryLUID", "CategoryName");
        }

        //dropdown for names actor, director, etc...
        private void fillNamesDdl()
        {
            Names = NamesLU.GetNames().ToList();
            ViewBag.vbNamesID = new SelectList(Names, "NamesLUID", "Name");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //HttpGet
        public ActionResult Categories(MovieV mvV, SortingPagingInfo info, DataGridParams dgParams, Movie mv, MovieCatsAll mvCA)
        {
            string strLastMovieTitle = "";

            dgParams.strSelectedCategoriesSrch = "";
            ViewBag.Movie = mv;
            ViewBag.MovieSrchVars = mvV;
            ViewBag.SortingPagingInfo = info;
            ViewBag.DataGridParams = dgParams;
            ViewBag.MovieCatsAll = mvCA;
            var qryMovieList = db.Database.SqlQuery<MovieCatsAll>(
                "exec dbo.[spGetAllMoviesAndCats]").ToList();
            info.Init(10, "ascending", "MovieTitle", qryMovieList.Count());
            var qryMovieListOrder = new List<MovieCatsAll>();

            //if (info.SortDirection == "ascending")
            //{
            //    qryMovieListOrder = qryMovieList.OrderBy(mv => mv.MovieTitle).ToList();
            //    info.SortDirection = "ascending";
            //}
            //else
            //{
            //    qryMovieListOrder = qryMovieList.OrderByDescending(mv => mv.MovieTitle).ToList();
            //    info.SortDirection = "descending";
            //}

            info.SortField = "MovieTitle";
            //How many rows per datagrid page?
            info.PageCount = info.GetPgCnt(qryMovieList.Count(), info.PageSize);
            ViewBag.SortingPagingInfo = info;
            strLastMovieTitle = "";

            foreach (var mvCat in qryMovieListOrder)
            {
                //var qryMovieCats = db.Database.SqlQuery<MovieCategory>(
                //    "exec dbo.[spGetMovieCatsAll] @MovieID",
                //            new SqlParameter("MovieID", mvCat.MovieID)).ToList();

                if (strLastMovieTitle == "" || (strLastMovieTitle != mvCat.MovieTitle))
                {
                    dgCat.Add(new DatagridCat()
                    {
                        MovieTitles = qryMovieList,
                        MovieID = mvCat.MovieID,
                        MovieTitle = mvCat.MovieTitle,
                        CategoryID = mvCat.CategoryLUID,
                        Category = mvCat.Category
                    });
                }
                else
                    //dgCat.Add(new DatagridCat() { MovieID = mvCat.MovieID, MovieTitle = mvCat.MovieTitle, CategoryID = mvCat.CategoryLUID, Category = mvCat.Category, MovieCats = qryMovieCats });
                    dgCat.Add(new DatagridCat()
                    {
                        MovieID = mvCat.MovieID,
                        MovieTitle = "",
                        CategoryID = mvCat.CategoryLUID,
                        Category = mvCat.Category
                    });
            }
            return View(dgCat);
        }

        [HttpPost]
        public ActionResult Categories()
        {
            FillTheGrid();
            return View(dgCat);
        }

        public void FillTheGrid()
        {
            bool blnFirstTime = true;
            //SortingPagingInfo info;
            //DataGridParams dgParams;
            //MovieDBContext db;
            //MovieCatsAll mvCA;

            //if (dgParams.strSelectedCategoriesSrch == null)
            //    dgParams.strSelectedCategoriesSrch = "";
            var qryMovieList = db.Database.SqlQuery<MovieCatsAll>(
                "exec dbo.[spGetAllMoviesAndCats]").ToList();

            ViewBag.Cats = Categorys;
            ViewBag.MovieCatsAll = mvCataAll;
            //ViewBag.MovieSrchVars = mvV;
            info.Init(10, "ascending", "MovieTitle", qryMovieList.Count());
            var qryMovieListOrder = new List<MovieCatsAll>();

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
            dgParams.Init(info.PageSize, info.SortDirection, info.SortField, qryMovieListOrder.Count(), "N");
            dgParams.SortDirectionFromPrev = info.SortDirection;
            dgParams.SortFieldFromPrev = info.SortField;
            dgParams.strSelectedCategoriesSrch = "" + dgParams.strSelectedCategoriesSrch;
            dgParams.CurrentPageIndexFromPrev = info.CurrentPageIndex;
            ViewBag.DataGridParams = dgParams;
            db.Database.Connection.Close();

            foreach (var mvCat in qryMovieListOrder)
            {
                var qryMovieCats = db.Database.SqlQuery<MovieCategory>(
                    "exec dbo.[spGetMovieCatsAll] @MovieID",
                            new SqlParameter("MovieID", mvCat.MovieID)).ToList();

                if (blnFirstTime)
                {
                    dgCat.Add(new DatagridCat()
                    {
                        MovieTitles = qryMovieList,
                        MovieID = mvCat.MovieID,
                        MovieTitle = mvCat.MovieTitle,
                        CategoryID = mvCat.CategoryLUID,
                        Category = mvCat.Category,
                        MovieCats = qryMovieCats
                    });
                    blnFirstTime = false;
                }
                else
                    dgCat.Add(new DatagridCat() { MovieID = mvCat.MovieID, MovieTitle = mvCat.MovieTitle, CategoryID = mvCat.CategoryLUID, Category = mvCat.Category, MovieCats = qryMovieCats });
            }

            dgCat = dgCat.Skip(Convert.ToInt32(info.CurrentPageIndex) * info.PageSize).Take(info.PageSize).ToList();
            Session["CurrentPageIndex"] = info.CurrentPageIndex;
        }
    }
}
