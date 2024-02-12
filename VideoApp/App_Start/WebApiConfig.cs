using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Cors;
using System.Web.Http.Cors;
using System.Web.Services.Description;
using Ninject.Modules;
using Ninject;

namespace Video_Streaming.App_Start
{

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.SuppressHostPrincipal();
            config.MapHttpAttributeRoutes();
            EnableCorsAttribute corsAttribute = new EnableCorsAttribute("*", "*", "*"); // You should adjust these values based on your security requirements
            config.EnableCors(corsAttribute);
            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional } //controller = "VideoManager", action = "getAllVideoFiles" }
           );
        }
    }
}