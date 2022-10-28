using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Sleekflow
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {            
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            // To allow specific action method name to be used. Example, Get -> GetByName
            config.Routes.MapHttpRoute("API Default", "{controller}/{action}/{id}",
            new { id = RouteParameter.Optional });
        }
    }
}
