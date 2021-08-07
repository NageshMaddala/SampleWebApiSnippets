using System.Web.Http;
using SampleSnippets.ActionFilters;
using SampleSnippets.AuthFilters;
using SampleSnippets.Handlers;

namespace SampleSnippets
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.SuppressHostPrincipal();

            //Delegating handler
            config.MessageHandlers.Add(new RequestResponseHandler());

            //Action Filters
            //config.Filters.Add(new ValidateModelStateAttribute());
            config.Filters.Add(new RouterTimerFilterAttribute());

            //Basic Auth
            config.Filters.Add(new BasicAuthFilterAttribute());

            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "LegacyValueApi",
            //    routeTemplate: "api/legacyVal/{id}",
            //    defaults: new { controller = "values", id = RouteParameter.Optional }
            //);

            //Template style routing, this where we define the template for routing.
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
