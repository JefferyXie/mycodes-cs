using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MyPlainAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            /*config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );*/
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "ContactMultiParaAPI",
                routeTemplate: "api/{controller}/{action}/{id}/{name}",
                constraints: new { action = "rawmultiple" },
                defaults: new { name = RouteParameter.Optional }
            );
            config.Formatters.XmlFormatter.UseXmlSerializer = true;
        }
    }
}
