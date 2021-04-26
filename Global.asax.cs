using Movies.Models;
using System;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Movies
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.SetInitializer<MovieDBContext>(null);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["Movies"] = "x";
        }

        protected void Session_End(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

    }
}
