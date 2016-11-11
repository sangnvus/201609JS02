using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BSNCapstone
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            //DangVH. Create. Start (08/11/2016)
            routes.MapRoute(
                name: "GroupMember",
                url: "Groups/{id}/Members",
                defaults: new { controller = "Groups", action = "Members", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "GroupMainPage",
                url: "Groups/{id}/MainPage",
                defaults: new { controller = "Groups", action = "MainPage", id = UrlParameter.Optional }
            );
            //DangVH. Create. End (08/11/2016)

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
