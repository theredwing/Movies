using Movies.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Movies.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private const int SQLConnection = 2;
        private const string EDIT = "e";
        private MovieDBContext db = new MovieDBContext();
        private List<CategoryLU> Categorys = new List<CategoryLU>();
        private List<PositionsLU> Positions = new List<PositionsLU>();
        private List<NamesLU> Names = new List<NamesLU>();
        private static SortingPagingInfo info = new SortingPagingInfo();
        private static Additions misc = new Additions();
        private EditDescV edv;
        private AdminV av;
        private MergeNamesV mnv = new MergeNamesV();
        IList<DataGridNames> dgName = new List<DataGridNames>();
        IList<DataGridPositions> dgPos = new List<DataGridPositions>();
        IList<DataGridCategories> dgCat = new List<DataGridCategories>();

        [HttpGet]
        public ActionResult AdminIndex(AdminV av, int? intID, string strName, string title, int? delete, int? edit, int? merge, int? search)
        {
            Errors Err = new Errors();
            Err.Message = "";
            av.strAddMsg = "";

            if (delete != null)
            {
                var dataset = new DataSet();
                var args = new object[] { intID };

                switch (title)
                {
                    case "Name":
                        MovieDBContext.LoadDataSet(ref dataset, "spDelName", args);
                        break;
                    case "Pos":
                        MovieDBContext.LoadDataSet(ref dataset, "spDelPosition", args);
                        break;
                    case "Cat":
                        MovieDBContext.LoadDataSet(ref dataset, "spDelCategory", args);
                        break;
                }

                Err.Message = dataset.Tables[0].Rows[0][0].ToString();
                if (Err.Message.Length > 0)
                {
                    av.TriggerOnLoad = true;
                }

                if (title == "Name")
                {
                    var qryMovieList = db.Database.SqlQuery<NamesLU>(
                        "exec dbo.[spSearchNames] @strName",
                        new SqlParameter("strName", (object)av.strName ?? DBNull.Value)).ToList();
                    av.Names = Search(Err, av, info);
                    av.strTab = "Names";
                    RefreshDataGridAndViewBags(av, qryMovieList, info, Err, Err.Message);
                    return View(av);
                }
                else if (title == "Pos")
                {
                    var qryPositionsList = db.Database.SqlQuery<PositionsLU>("exec dbo.[spGetAllPositions]").ToList();
                    av.Pos = SearchPos(Err, av, info);
                    av.strTab = "Pos";
                    RefreshDataGridAndViewBagsPos(av, qryPositionsList, info, Err, Err.Message);
                    return View(av);
                }
                else if (title == "Cat")
                {
                    var qryCategoriesList = db.Database.SqlQuery<CategoryLU>("exec dbo.[spGetCategories]").ToList();
                    av.Cats = SearchCat(Err, av, info);
                    av.strTab = "Cat";
                    RefreshDataGridAndViewBagsCats(av, qryCategoriesList, info, Err, Err.Message);
                    return View(av);
                }
            }

            if (edit != null)
            {
                return View();
            }
            if (merge != null)
            {
                return View();
            }
            if (search != null)
            {
                av.Names = Search(Err, av, info);
                return View(av);
            }
            else
            {
                switch (title)
                {
                    case "Name":
                        info.Init(10, "ascending", "Name", 0);
                        misc.GroupName = "Name";
                        break;
                    case "Pos":
                        info.Init(10, "ascending", "Position", 0);
                        misc.GroupName = "Pos";
                        break;
                    case "Cat":
                        info.Init(10, "ascending", "Category", 0);
                        misc.GroupName = "Cat";
                        break;
                    default:
                        misc.GroupName = "";
                        break;
                }

                av = new AdminV(Names);
                misc.EditPageAction = "Initial Load";
                misc.GroupName = "";
                DataPassed(misc, info, Err, "");
                return View(av);
            }
        }

        [HttpPost]
        public ActionResult AdminIndex(Additions misc, SortingPagingInfo info, AdminV av, EditDescV edv, int? NameID, int? PosID, int? CatID)
        {
            Errors Err = new Errors();
            Err.Message = "";
            ViewData["AddMsg"] = "";

            // *********************************************************************************************************************************************************
            // ***** Name **********************************************************************************************************************************************
            // *********************************************************************************************************************************************************
            if (misc.GroupName == "Name")
            {
                var qryMovieListOrder = new List<NamesLU>();

                //Sorting and paging through the datagrid of Names
                if (misc.EditPageAction == "Sort" || misc.EditPageAction == "Page")
                {
                    bool blnFirstTime = true;
                    av.TriggerOnLoad = false;
                    db.Database.Connection.Close();

                    var qryMovieList = db.Database.SqlQuery<NamesLU>(
                        "exec dbo.[spSearchNames] @strName",
                            new SqlParameter("strName", (object)av.strName ?? DBNull.Value)).ToList();
                    info.Init(10, info.SortDirection, "Name", qryMovieList.Count);

                    if (info.SortDirection == "descending")
                    {
                        qryMovieListOrder = qryMovieList.OrderByDescending(mv => mv.Name).ToList();
                    }
                    else
                    {
                        qryMovieListOrder = qryMovieList.OrderBy(mv => mv.Name).ToList();
                    }

                    foreach (var nm in qryMovieListOrder)
                    {
                        if (blnFirstTime)
                        {
                            dgName.Add(new DataGridNames() { Names = qryMovieList, NameLUID = nm.NamesLUID, Name = nm.Name });
                            blnFirstTime = false;
                        }
                        else
                            dgName.Add(new DataGridNames() { NameLUID = nm.NamesLUID, Name = nm.Name });
                    }

                    av.Names = qryMovieListOrder.Skip(Convert.ToInt32(info.CurrentPageIndex) * info.PageSize).Take(info.PageSize).ToList();
                    av.strTab = "Names";
                    DataPassed(misc, info, Err, "");
                    return View(av);
                }
                else if (misc.EditPageAction == "Search")
                {
                    av.strName = ("" + av.strName).Replace("'", "");
                    av.Names = Search(Err, av, info);
                    av.strTab = "Names";
                    return View(av);
                }
                else if (misc.EditPageAction == "Add")
                {
                    if (("" + av.strName).Trim() == "")
                    {
                        av.TriggerOnLoad = true;
                        DataPassed(misc, info, Err, "Name Cannot be Blank!");
                        av.strTab = "Names";
                        return View(av);
                    }
                    else
                    {
                        var dataset = new DataSet();
                        var args = new object[] { av.strName };
                        MovieDBContext.LoadDataSet(ref dataset, "spCheckNameAndInsert", args);
                        Err.Message = dataset.Tables[0].Rows[0][0].ToString();

                        if (Err.Message.Length > 0)
                        {
                            av.TriggerOnLoad = true;
                            DataPassed(misc, info, Err, Err.Message);
                            av.strTab = "Names";
                            return View(av);
                        }
                        else
                        {
                            DataPassed(misc, info, Err, "");
                            av.strTab = "Names";
                            av.TriggerOnLoad = false;
                            ViewData["AddMsg"] = av.strName + " Successfully Added";
                            return View(av);
                        }
                    }
                }
                else if (misc.EditPageAction == "Initial Load" || misc.EditPageAction == "Edit")
                {
                    av = PosGrid(db, av, misc, info, Err, "");
                    return View(av);
                }
                else
                    return View();
            }
            // *********************************************************************************************************************************************************
            // ***** Position **********************************************************************************************************************************************
            // *********************************************************************************************************************************************************
            else if (misc.GroupName == "Pos")
            {
                //Sorting and paging through the datagrid of Names
                if (misc.EditPageAction == "Sort" || misc.EditPageAction == "Page")
                {
                    av = PosGrid(db, av, misc, info, Err, "");
                    return View(av);
                }
                else if (misc.EditPageAction == "Add")
                {
                    if (("" + av.strPos).Trim() == "")
                    {
                        //DataPassed(misc, info, Err, "Position Cannot be Blank!");
                        av = PosGrid(db, av, misc, info, Err, "Position Cannot be Blank!");
                        av.TriggerOnLoad = true;
                        return View(av);
                    }
                    else
                    {
                        var dataset = new DataSet();
                        var args = new object[] { av.strPos };
                        MovieDBContext.LoadDataSet(ref dataset, "spCheckPosAndInsert", args);
                        Err.Message = dataset.Tables[0].Rows[0][0].ToString();

                        if (Err.Message.Length > 0)
                        {
                            //DataPassed(misc, info, Err, Err.Message);
                            av = PosGrid(db, av, misc, info, Err, Err.Message);
                            av.TriggerOnLoad = true;
                            return View(av);
                        }
                        else
                        {
                            //DataPassed(misc, info, Err, "");
                            av = PosGrid(db, av, misc, info, Err, "");
                            ViewData["AddMsg"] = "Position Successfully Added";
                            return View(av);
                        }
                    }
                }
                else if (misc.EditPageAction == "Initial Load" || misc.EditPageAction == "Edit")
                {
                    av = PosGrid(db, av, misc, info, Err, "");
                    return View(av);
                }
                else
                    return View();
            }

            // *********************************************************************************************************************************************************
            // ***** Category **********************************************************************************************************************************************
            // *********************************************************************************************************************************************************
            else /*if (misc.GroupName == "Cat")*/
            {
                //Sorting and paging through the datagrid of Names
                if (misc.EditPageAction == "Sort" || misc.EditPageAction == "Page")
                {
                    av = CatGrid(db, av, misc, info, Err, "");
                    return View(av);
                }
                else if (misc.EditPageAction == "Add")
                {
                    if (("" + av.strCat).Trim() == "")
                    {
                        av = CatGrid(db, av, misc, info, Err, "Category Cannot be Blank!");
                        av.TriggerOnLoad = true;
                        return View(av);
                    }
                    else
                    {
                        var dataset = new DataSet();
                        var args = new object[] { av.strCat };
                        MovieDBContext.LoadDataSet(ref dataset, "spCheckCatAndInsert", args);
                        Err.Message = dataset.Tables[0].Rows[0][0].ToString();

                        if (Err.Message.Length > 0)
                        {
                            av = CatGrid(db, av, misc, info, Err, Err.Message);
                            av.TriggerOnLoad = true;
                            return View(av);
                        }
                        else
                        {
                            av = CatGrid(db, av, misc, info, Err, "");
                            ViewData["AddMsg"] = "Category Successfully Added";
                            return View(av);
                        }
                    }
                }
                else if (misc.EditPageAction == "Initial Load" || misc.EditPageAction == "Edit")
                {
                    av = CatGrid(db, av, misc, info, Err, "");
                    return View(av);
                }
                else
                    return View();
            }
        }

        public ActionResult EditDesc(EditDescV edv, Additions misc, int id, string desc, string title, string srchname)
        {
            misc.GroupName = title;
            edv.strTitle = "";

            switch (title)
            {
                case "Name":
                    edv.strTitle = "Name";
                    edv.strSrchName = srchname;
                    break;
                case "Pos":
                    edv.strTitle = "Position";
                    edv.strSrchPos = srchname;
                    break;
                case "Cat":
                    edv.strTitle = "Category";
                    edv.strSrchCat = srchname;
                    break;
            }

            ViewBag.vbMisc = misc;
            edv.intID = id;
            edv.strDesc = desc;
            return View(edv);
        }

        [HttpPost]
        public ActionResult EditDesc(EditDescV edv, Additions misc)
        {
            Errors Err = new Errors();
            Err.Message = "";

            if (misc.EditPageAction == "Edit")
                if (misc.GroupName == "Name")
                {
                    edv.strTitle = "Name";

                    if (("" + edv.strDesc).Trim() == "")
                    {
                        edv.TriggerOnLoad = true;
                        DataPassed(misc, info, Err, "Name Cannot be Blank!");
                        return View(edv);
                    }
                    else
                    {
                        var dataset = new DataSet();
                        var args = new object[] { edv.intID, edv.strDesc };
                        MovieDBContext.LoadDataSet(ref dataset, "spCheckNameAndUpdateName", args);
                        Err.Message = dataset.Tables[0].Rows[0][0].ToString();

                        if (Err.Message.Length > 0)
                        {
                            edv.TriggerOnLoad = true;
                            DataPassed(misc, info, Err, Err.Message);
                            return View(edv);
                        }
                        else
                        {
                            av = new AdminV();
                            av.strName = edv.strSrchName;
                            av.Names = Search(Err, av, info);
                            av.strTab = "Names";
                            DataPassed(misc, info, Err, Err.Message);
                            return View("AdminIndex", av);
                        }
                    }
                }
                else if (misc.GroupName == "Pos")
                {
                    edv.strTitle = "Position";

                    if (("" + edv.strDesc).Trim() == "")
                    {
                        edv.TriggerOnLoad = true;
                        DataPassed(misc, info, Err, "Position Cannot be Blank!");
                        return View(edv);
                    }
                    else
                    {
                        var dataset = new DataSet();
                        var args = new object[] { edv.intID, edv.strDesc };
                        MovieDBContext.LoadDataSet(ref dataset, "spCheckPosAndUpdatePos", args);
                        Err.Message = dataset.Tables[0].Rows[0][0].ToString();

                        if (Err.Message.Length > 0)
                        {
                            DataPassed(misc, info, Err, Err.Message);
                            edv.TriggerOnLoad = true;
                            return View(edv);
                        }
                        else
                        {
                            av = new AdminV();
                            av.strPos = edv.strSrchPos;
                            av.Pos = SearchPos(Err, av, info);
                            av.strTab = "Pos";
                            DataPassed(misc, info, Err, Err.Message);
                            return View("AdminIndex", av);
                        }
                    }
                }
                else //Category
                {
                    edv.strTitle = "Category";

                    if (("" + edv.strDesc).Trim() == "")
                    {
                        edv.TriggerOnLoad = true;
                        DataPassed(misc, info, Err, "Category Cannot be Blank!");
                        return View(edv);
                    }
                    else
                    {
                        var dataset = new DataSet();
                        var args = new object[] { edv.intID, edv.strDesc };
                        MovieDBContext.LoadDataSet(ref dataset, "spCheckCatAndUpdateCat", args);
                        Err.Message = dataset.Tables[0].Rows[0][0].ToString();

                        if (Err.Message.Length > 0)
                        {
                            DataPassed(misc, info, Err, Err.Message);
                            edv.TriggerOnLoad = true;
                            return View(edv);
                        }
                        else
                        {
                            av = new AdminV();
                            av.strCat = edv.strSrchCat;
                            av.Cats = SearchCat(Err, av, info);
                            av.strTab = "Cat";
                            DataPassed(misc, info, Err, Err.Message);
                            return View("AdminIndex", av);
                        }
                    }
                }

            //Cancel-Edit
            av = new AdminV();
            if (misc.GroupName == "Name")
            {
                av.strName = edv.strSrchName;
                av.Names = Search(Err, av, info);
                av.strTab = "Names";
            }
            else if (misc.GroupName == "Pos")
            {
                av.strPos = edv.strSrchPos;
                av.Pos = SearchPos(Err, av, info);
                av.strTab = "Pos";
            }
            else //Category
            {
                av.strCat = edv.strSrchCat;
                av.Cats = SearchCat(Err, av, info);
                av.strTab = "Cat";
            }

            DataPassed(misc, info, Err, Err.Message);
            return View("AdminIndex", av);
        }

        private AdminV PosGrid(MovieDBContext db, AdminV av, Additions misc, SortingPagingInfo info, Errors Err, string strMsg)
        {
            bool blnFirstTime = true;
            var qryPosListOrder = new List<PositionsLU>();

            db.Database.Connection.Close();
            var qryPositionsList = db.Database.SqlQuery<PositionsLU>("exec dbo.[spGetAllPositions]").ToList();
            av = new AdminV(qryPositionsList);
            av.TriggerOnLoad = false;
            info.Init(10, info.SortDirection, "Position", qryPositionsList.Count);

            if (info.SortDirection == "descending")
            {
                qryPosListOrder = qryPositionsList.OrderByDescending(p => p.Position).ToList();
            }
            else
            {
                qryPosListOrder = qryPositionsList.OrderBy(p => p.Position).ToList();
            }

            foreach (var p in qryPosListOrder)
            {
                if (blnFirstTime)
                {
                    dgPos.Add(new DataGridPositions() { Positions = qryPositionsList, PositionLUID = p.PositionsLUID, Position = p.Position });
                    blnFirstTime = false;
                }
                else
                    dgPos.Add(new DataGridPositions() { PositionLUID = p.PositionsLUID, Position = p.Position });
            }

            info.CurrentPageIndex = 0;
            av.Pos = qryPosListOrder.Skip(Convert.ToInt32(info.CurrentPageIndex) * info.PageSize).Take(info.PageSize).ToList();
            av.strTab = "Pos";
            DataPassed(misc, info, Err, strMsg);
            return (av);
        }

        private AdminV CatGrid(MovieDBContext db, AdminV av, Additions misc, SortingPagingInfo info, Errors Err, string strMsg)
        {
            bool blnFirstTime = true;
            var qryCatListOrder = new List<CategoryLU>();

            db.Database.Connection.Close();
            var qryCategoriesList = db.Database.SqlQuery<CategoryLU>("exec dbo.[spGetCategories]").ToList();
            av = new AdminV(qryCategoriesList);
            av.TriggerOnLoad = false;
            info.Init(10, info.SortDirection, "Category", qryCategoriesList.Count);

            if (info.SortDirection == "descending")
            {
                qryCatListOrder = qryCategoriesList.OrderByDescending(c => c.CategoryName).ToList();
            }
            else
            {
                qryCatListOrder = qryCategoriesList.OrderBy(c => c.CategoryName).ToList();
            }

            foreach (var c in qryCatListOrder)
            {
                if (blnFirstTime)
                {
                    dgCat.Add(new DataGridCategories() { Categories = qryCategoriesList, CategoryLUID = c.CategoryLUID, Category = c.CategoryName });
                    blnFirstTime = false;
                }
                else
                    dgCat.Add(new DataGridCategories() { CategoryLUID = c.CategoryLUID, Category = c.CategoryName });
            }

            av.Cats = qryCatListOrder.Skip(Convert.ToInt32(info.CurrentPageIndex) * info.PageSize).Take(info.PageSize).ToList();
            av.strTab = "Cat";
            DataPassed(misc, info, Err, strMsg);
            return (av);
        }

        public ActionResult MergeNames(MergeNamesV mnv, Additions misc, int id, string person, string title, string srchname)
        {
            Errors Err = new Errors();
            Err.Message = "";

            mnv.intName1From = id;
            mnv.strName1From = person;
            mnv.intName2From = 0;
            mnv.strName2From = "";
            mnv.intName3From = 0;
            mnv.strName3From = "";
            mnv.intName4From = 0;
            mnv.strName4From = "";
            mnv.intName5From = 0;
            mnv.strName5From = "";
            mnv.intNameTo = 0;
            mnv.strNameTo = "";
            mnv.strSrchName = srchname;
            fillNamesDdl();
            ViewBag.Errors = Err;
            ViewBag.vbMisc = misc;
            return View(mnv);
        }

        [HttpPost]
        public ActionResult MergeNames(Additions misc, SortingPagingInfo info, MergeNamesV mnv)
        {
            Errors Err = new Errors();
            Err.Message = "";
            fillNamesDdl();

            if (misc.EditPageAction == "Save")
            {
                if (mnv.intNameTo == 0)
                {
                    mnv.TriggerOnLoad = true;
                    DataPassed(misc, info, Err, "Copy To and Merge With Cannot be Blank!");
                    ViewBag.Errors = Err;
                    return View(mnv);
                }
                else if (mnv.intName1From == 0 && mnv.intName2From == 0 && mnv.intName3From == 0 && mnv.intName4From == 0 && mnv.intName5From == 0)
                {
                    mnv.TriggerOnLoad = true;
                    DataPassed(misc, info, Err, "At Least One Copy From and Dispose Must be Selected!");
                    ViewBag.Errors = Err;
                    return View(mnv);
                }
                else if (mnv.intName1From == mnv.intNameTo || mnv.intName2From == mnv.intNameTo || mnv.intName3From == mnv.intNameTo || mnv.intName4From == mnv.intNameTo || mnv.intName5From == mnv.intNameTo)
                {
                    mnv.TriggerOnLoad = true;
                    DataPassed(misc, info, Err, "No Copy From and Dispose Selections can be Equal to Copy To and Merge!");
                    ViewBag.Errors = Err;
                    return View(mnv);
                }
                else
                {
                    var dataset = new DataSet();
                    var args = new object[] { mnv.intName1From, mnv.intName2From, mnv.intName3From, mnv.intName4From, mnv.intName5From, mnv.intNameTo };

                    MovieDBContext.LoadDataSet(ref dataset, "spMergeNames", args);
                    av = new AdminV();
                    av.strName = mnv.strSrchName;
                    av.Names = Search(Err, av, info);
                    DataPassed(misc, info, Err, Err.Message);
                    av.strTab = "Names";
                    return View("AdminIndex", av);
                }
            }
            else
            {
                av = new AdminV();
                av.strName = mnv.strSrchName;
                av.Names = Search(Err, av, info);
                DataPassed(misc, info, Err, Err.Message);
                av.strTab = "Names";
                return View("AdminIndex", av);
            }

        }

        private List<NamesLU> Search(Errors Err, AdminV av, SortingPagingInfo info)
        {
            var qryMovieList = db.Database.SqlQuery<NamesLU>(
                "exec dbo.[spSearchNames] @strName",
            new SqlParameter("strName", (object)av.strName ?? DBNull.Value)).ToList();
            av = new AdminV(qryMovieList);
            info.Init(10, "ascending", "Name", qryMovieList.Count);
            info.CurrentPageIndex = 0;
            RefreshDataGridAndViewBags(av, qryMovieList, info, Err);
            return (av.Names);
        }

        private List<PositionsLU> SearchPos(Errors Err, AdminV av, SortingPagingInfo info)
        {
            var qryPosList = db.Database.SqlQuery<PositionsLU>("exec dbo.[spGetAllPositions]").ToList();
            av = new AdminV(qryPosList);
            info.Init(10, "ascending", "Position", qryPosList.Count);
            info.CurrentPageIndex = 0;
            RefreshDataGridAndViewBagsPos(av, qryPosList, info, Err);
            return (av.Pos);
        }

        private List<CategoryLU> SearchCat(Errors Err, AdminV av, SortingPagingInfo info)
        {
            var qryCatList = db.Database.SqlQuery<CategoryLU>("exec dbo.[spGetCategories]").ToList();
            av = new AdminV(qryCatList);
            info.Init(10, "ascending", "Category", qryCatList.Count);
            info.CurrentPageIndex = 0;
            RefreshDataGridAndViewBagsCats(av, qryCatList, info, Err);
            return (av.Cats);
        }

        private void RefreshDataGridAndViewBags(AdminV av, List<NamesLU> qryNameList, SortingPagingInfo info, Errors Err, string strErr = "")
        {
            bool blnFirstTime = true;

            foreach (var nm in qryNameList)
            {
                if (blnFirstTime)
                {
                    dgName.Add(new DataGridNames() { Names = qryNameList, NameLUID = nm.NamesLUID, Name = nm.Name });
                    blnFirstTime = false;
                }
                else
                    dgName.Add(new DataGridNames() { NameLUID = nm.NamesLUID, Name = nm.Name });
            }

            av.Names = qryNameList.Skip(Convert.ToInt32(info.CurrentPageIndex) * info.PageSize).Take(info.PageSize).ToList();
            ViewBag.vbSortingPaging = info;
            ViewBag.vbMisc = misc;
            ViewBag.vbErrors = Err;
            ViewData["Message"] = strErr;
        }

        private void RefreshDataGridAndViewBagsPos(AdminV av, List<PositionsLU> qryPosList, SortingPagingInfo info, Errors Err, string strErr = "")
        {
            bool blnFirstTime = true;

            foreach (var p in qryPosList)
            {
                if (blnFirstTime)
                {
                    dgPos.Add(new DataGridPositions() { Positions = qryPosList, PositionLUID = p.PositionsLUID, Position = p.Position });
                    blnFirstTime = false;
                }
                else
                    dgPos.Add(new DataGridPositions() { PositionLUID = p.PositionsLUID, Position = p.Position });
            }

            av.Pos = qryPosList.Skip(Convert.ToInt32(info.CurrentPageIndex) * info.PageSize).Take(info.PageSize).ToList();
            ViewBag.vbSortingPaging = info;
            ViewBag.vbMisc = misc;
            ViewBag.vbErrors = Err;
            ViewData["Message"] = strErr;
        }

        private void RefreshDataGridAndViewBagsCats(AdminV av, List<CategoryLU> qryCatList, SortingPagingInfo info, Errors Err, string strErr = "")
        {
            bool blnFirstTime = true;

            foreach (var c in qryCatList)
            {
                if (blnFirstTime)
                {
                    dgCat.Add(new DataGridCategories() { Categories = qryCatList, CategoryLUID = c.CategoryLUID, Category = c.CategoryName });
                    blnFirstTime = false;
                }
                else
                    dgCat.Add(new DataGridCategories() { CategoryLUID = c.CategoryLUID, Category = c.CategoryName });
            }

            av.Cats = qryCatList.Skip(Convert.ToInt32(info.CurrentPageIndex) * info.PageSize).Take(info.PageSize).ToList();
            ViewBag.vbSortingPaging = info;
            ViewBag.vbMisc = misc;
            ViewBag.vbErrors = Err;
            ViewData["Message"] = strErr;
        }

        private void DataPassed(Additions misc, SortingPagingInfo info, Errors Err, string strMsg)
        {
            ViewBag.vbSortingPaging = info;
            ViewBag.vbMisc = misc;
            ViewBag.vbErrors = Err;
            ViewData["Message"] = strMsg;
        }

        //dropdown for names actor, director, etc...
        private void fillNamesDdl()
        {
            Names = NamesLU.GetNames().ToList();
            ViewBag.vbNamesID = new SelectList(Names, "NamesLUID", "Name");
        }
    }
}