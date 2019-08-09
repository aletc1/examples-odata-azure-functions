using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzureFunctionOData.Models;
using System.Collections.Generic;
using System.Linq;

namespace AzureFunctionOData
{
    public static class FunctionODataExample
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequest req,
            ILogger log)
        {
            // Inicializamos una lista de productos (Puede ser una tabla de SQL o CosmosDB a través de Entity Framework Core)
            var data = new List<Product>() {
                new Product() { Title = "Mountain Bike SERIOUS ROCKVILLE", Category = "Mountain Bicycle" },
                new Product() { Title = "Mountain Bike eléctrica HAIBIKE SDURO HARD SEVEN", Category = "Mountain Bicycle" },
                new Product() { Title = "Sillín BROOKS CAMBIUM C15 CARVED ALL WEATHER", Category = "Sillin" },
                new Product() { Title = "Poncho VAUDE COVERO II Amarillo", Category = "Chaquetas" },
            };

            // Aplicamos la consulta OData al IQueryable<Product> a la fuente de datos anterior
            var result = req.ApplyTo<Product>(data.AsQueryable());

            // Se retorna el resultado
            return new OkObjectResult(result);
        }
    }
}
