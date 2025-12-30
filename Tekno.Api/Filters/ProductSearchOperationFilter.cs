using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Tekno.Api.Filters
{
    /// <summary>
    /// Enhances Swagger for GET /api/products with examples and a filtersJson query parameter.
    /// </summary>
    public class ProductSearchOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null || context == null) return;

            // Only target the product listing GET operation path
            var relativePath = context.ApiDescription.RelativePath?.Trim().ToLowerInvariant();
            var httpMethod = context.ApiDescription.HttpMethod?.ToUpperInvariant();

            if (relativePath == "api/products" && httpMethod == "GET")
            {
                // Short summary for FE
                operation.Summary = "Search and browse products — supports keyword, category, brand, price and spec filters.";

                // Append detailed usage to description (keeps existing XML comments)
                var extra = "\n\nSwagger examples and usage:\n" +
                            "- Query filters as query params: `filters[RAM]=16GB,32GB` or repeated `filters[Color]=Black&filters[Color]=White`.\n" +
                            "- JSON filters (URL-encoded) via `filtersJson`: `?filtersJson={\"RAM\":[\"16GB\",\"32GB\"]}` (URL-encode in browser).\n" +
                            "- Example curl (comma filters):\n``n``curl \"/api/products?keyword=iPhone&filters[Color]=Black,White&sort=-price&page=1&pageSize=12\"``n``\n" +
                            "- Example curl (filtersJson encoded):\n``n``curl \"/api/products?filtersJson=%7B%22RAM%22%3A%5B%2216GB%22%2C%2232GB%22%5D%7D\"``n``\n" +
                            "Notes:\n- In Swagger UI you may need to edit the request URL to add `filters[...]` keys because the form doesn't support dynamic keys.\n";

                if (string.IsNullOrEmpty(operation.Description))
                    operation.Description = extra;
                else if (!operation.Description.Contains("Swagger examples and usage"))
                    operation.Description += extra;

                // Add filtersJson explicit parameter so FE sees it in UI
                if (!operation.Parameters.Any(p => p.Name == "filtersJson"))
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = "filtersJson",
                        In = ParameterLocation.Query,
                        Description = "URL-encoded JSON filters, e.g. { \"RAM\": [\"16GB\",\"32GB\"] }",
                        Required = false,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Example = new OpenApiString("{\"RAM\":[\"16GB\",\"32GB\"]}")
                        }
                    });
                }

                // Add example for filters[...] parameter to aid FE (note: Swagger cannot enumerate dynamic keys)
                if (!operation.Parameters.Any(p => p.Name == "filters[Color]"))
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = "filters[Color]",
                        In = ParameterLocation.Query,
                        Description = "Example filter parameter for specs. Use comma-separated values or repeat the key for multiple values.",
                        Required = false,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Example = new OpenApiString("Black,White")
                        }
                    });
                }
            }
        }
    }
}
