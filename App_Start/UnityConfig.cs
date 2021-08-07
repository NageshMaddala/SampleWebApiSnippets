using System.Web.Http;
using SampleSnippets.DAL;
using SampleSnippets.Interfaces;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;

namespace SampleSnippets
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<IProductRepository, ProductsRepository>(new ContainerControlledLifetimeManager());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}