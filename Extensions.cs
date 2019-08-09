using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.UriParser;
using System;
using System.Linq;

namespace AzureFunctionOData
{
    public static class Extensions
    {
        private static IServiceProvider _provider = null;
        private static RouteBuilder _routeBuilder = null;

        public static IQueryable ApplyTo<TEntity>(this HttpRequest request, IQueryable<TEntity> query)
            where TEntity : class
        {
            // Parte 1 - Se registran los componentes requeridos por la implementación de 
            // Microsoft ASP.NET Core OData y se memorizan en una variable estática
            if (_provider == null)
            {
                var collection = new ServiceCollection();
                collection.AddMvcCore();
                collection.AddOData();
                collection.AddTransient<ODataUriResolver>();
                collection.AddTransient<ODataQueryValidator>();
                collection.AddTransient<TopQueryValidator>();
                collection.AddTransient<FilterQueryValidator>();
                collection.AddTransient<SkipQueryValidator>();
                collection.AddTransient<OrderByQueryValidator>();
                _provider = collection.BuildServiceProvider();
            }

            // Parte 2 - Se configura la ruta de ASP.NET Core OData
            if (_routeBuilder == null)
            {
                _routeBuilder = new RouteBuilder(new ApplicationBuilder(_provider));
                _routeBuilder.EnableDependencyInjection();
            }

            // Parte 3 - Se simula un pedido HTTP como si viniese desde ASP.NET Core
            var modelBuilder = new ODataConventionModelBuilder(_provider);
            modelBuilder.AddEntityType(typeof(TEntity));
            var edmModel = modelBuilder.GetEdmModel();

            var httpContext = new DefaultHttpContext
            {
                RequestServices = _provider
            };
            HttpRequest req = new DefaultHttpRequest(httpContext)
            {
                Method = "GET",
                Host = request.Host,
                Path = request.Path,
                QueryString = request.QueryString
            };

            var oDataQueryContext = new ODataQueryContext(edmModel, typeof(TEntity), new Microsoft.AspNet.OData.Routing.ODataPath());
            var odataQuery = new ODataQueryOptions<TEntity>(oDataQueryContext, req);

            // Parte 4 - Se aplica la consulta OData al queryable que nos pasan por parámetro
            return odataQuery.ApplyTo(query.AsQueryable());
        }
    }
}