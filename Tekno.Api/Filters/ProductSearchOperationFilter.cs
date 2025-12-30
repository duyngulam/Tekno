using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Tekno.Api.Filters
{
    /// <summary>
    /// Enhances Swagger for GET /api/products with examples and a filters query parameter.
    /// Ensures only a single 'filters' query parameter is shown.
    /// </summary>
    public class ProductSearchOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null || context == null) return;

            var relativePath = context.ApiDescription.RelativePath?.Trim().ToLowerInvariant();
            var httpMethod = context.ApiDescription.HttpMethod?.ToUpperInvariant();

            if (relativePath == "api/products" && httpMethod == "GET")
            {
                operation.Summary = "Search and browse products — supports keyword, category, brand, price and spec filters.";

                // Clear any existing explanation then append a concise guidance
                var extra = "\n\nSwagger examples and usage:\n" +
                            "- Use single JSON 'filters' parameter: `filters={\\\"GPU\\\":[\\\"RTX 4070\\\"] ,\\\"RAM\\\":[\\\"16GB\\\"]}` (URL-encode when sending).\n" +
                            "- Enter JSON into the 'filters' field in Swagger UI because dynamic keys cannot be rendered by the form.\n" +
                            "- Example curl (JSON filters): curl \"/api/products?filters=%7B%5C%22GPU%5C%22%3A%5B%5C%22RTX%204070%5C%22%5D%7D\"\n";

                if (string.IsNullOrEmpty(operation.Description))
                    operation.Description = extra;
                else if (!operation.Description.Contains("Swagger examples and usage"))
                    operation.Description += extra;

                // Remove any existing parameters that contain 'filters' (case-insensitive)
                var toRemove = operation.Parameters.Where(p => p.Name?.IndexOf("filters", System.StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                foreach (var p in toRemove)
                {
                    operation.Parameters.Remove(p);
                }

                // Ensure only a single 'filters' parameter is presented in UI
                if (!operation.Parameters.Any(p => string.Equals(p.Name, "filters", System.StringComparison.OrdinalIgnoreCase)))
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = "filters",
                        In = ParameterLocation.Query,
                        Description = "JSON encoded filters object. Example: {\"GPU\":[\"RTX 4070\"],\"RAM\":[\"16GB\"]} (URL-encode when sending)",
                        Required = false,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Example = new OpenApiString("{\"GPU\":[\"RTX 4070\"]}")
                        }
                    });
                }
            }
        }
    }
}
