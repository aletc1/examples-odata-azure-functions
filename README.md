# Example: OData in Azure Functions v2

Oficially, there is no support for OData endpoints in *Azure Functions*.

This repo presents a *workaround* to implement OData in **Azure Functions v2** using a custom extension method `HttpRequest.ApplyTo<T>(IQueryable<T> query)`

This is only supported for C# and here you have an example of how the final function will look like:

```csharp
public static class FunctionODataExample
{
    [FunctionName("Function1")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequest req,
        ILogger log)
    {
        // Parte 1 - Inicializamos una lista de productos (Puede ser una tabla de SQL o CosmosDB a través de Entity Framework Core)
        var data = new List<Product>() {
            new Product() { Title = "Mountain Bike SERIOUS ROCKVILLE", Category = "Mountain Bicycle" },
            new Product() { Title = "Mountain Bike eléctrica HAIBIKE SDURO HARD SEVEN", Category = "Mountain Bicycle" },
            new Product() { Title = "Sillín BROOKS CAMBIUM C15 CARVED ALL WEATHER", Category = "Sillin" },
            new Product() { Title = "Poncho VAUDE COVERO II Amarillo", Category = "Chaquetas" },
        };

        // Parte 2 - Aplicamos la consulta OData al IQueryable<Product> a la fuente de datos anterior
        var result = req.ApplyTo<Product>(data.AsQueryable());

        // Parte 3 - Se retorna el resultado
        return new OkObjectResult(result);
    }
}
```

A detailed explanation can be found at [Implementar OData con funciones de Azure v2](https://www.algohace.net/posts/odata-en-azure-functions-v2/) (SPANISH ONLY)