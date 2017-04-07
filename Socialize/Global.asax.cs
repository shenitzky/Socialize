using Socialize.App_Start;
using Socialize.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Socialize
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Default stuff
            AreaRegistration.RegisterAllAreas();

            // Manually installed WebAPI 2.2 after making an MVC project.
            GlobalConfiguration.Configure(WebApiConfig.Register); // NEW way
                                                                  //WebApiConfig.Register(GlobalConfiguration.Configuration); // DEPRECATED

            // Default stuff
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Start running the thread
            var dummy = StartThreadHandler.workerFactory.Value;
        }
    }
}
