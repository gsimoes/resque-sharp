using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ResqueSharp.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static string _RedisHost;

        public static string RedisHost
        {
            get { return _RedisHost ?? (_RedisHost = WebConfigurationManager.AppSettings["RedisHost"]); }
        }

        public MvcApplication()
        {
            BeginRequest += (sender, args) => {
                HttpContext.Current.Items["CurrentRequestResqueClient"] = new Resque(RedisHost);
            };

            EndRequest += (sender, args) => {
                ((Resque)HttpContext.Current.Items["CurrentRequestResqueClient"]).Dispose();
            };
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "RequeueAll",
                url: "resque/failed/requeue/all",
                defaults: new { controller = "Resque", action = "RequeueAll" }
            );

            routes.MapRoute(
               name: "ClearFailed",
               url: "resque/failed/clear",
               defaults: new { controller = "Resque", action = "ClearFailed" }
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleTable.Bundles.RegisterTemplateBundles();
        }
    }
}