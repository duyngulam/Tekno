using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tekno.Api.Filters
{
    /// <summary>
    /// Adds default values and enhanced descriptions to Swagger operations
    /// </summary>
    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Add default responses if not present
            if (!operation.Responses.ContainsKey("401"))
            {
                operation.Responses.Add("401", new OpenApiResponse
                {
                    Description = "Unauthorized - Invalid or missing JWT token"
                });
            }

            if (!operation.Responses.ContainsKey("403"))
            {
                operation.Responses.Add("403", new OpenApiResponse
                {
                    Description = "Forbidden - User doesn't have required permissions"
                });
            }

            if (!operation.Responses.ContainsKey("500"))
            {
                operation.Responses.Add("500", new OpenApiResponse
                {
                    Description = "Internal Server Error - Something went wrong on the server"
                });
            }

            // Add operation ID if not present
            if (string.IsNullOrEmpty(operation.OperationId))
            {
                operation.OperationId = $"{context.MethodInfo.DeclaringType?.Name}_{context.MethodInfo.Name}";
            }

            // Mark deprecated operations
            if (context.MethodInfo.GetCustomAttributes(typeof(ObsoleteAttribute), false).Any())
            {
                operation.Deprecated = true;
            }

            // Add request examples for common content types
            if (operation.RequestBody?.Content != null)
            {
                foreach (var content in operation.RequestBody.Content)
                {
                    if (content.Value.Schema?.Reference != null)
                    {
                        var schemaName = content.Value.Schema.Reference.Id;
                        content.Value.Example = GetExampleForSchema(schemaName);
                    }
                }
            }
        }

        private static Microsoft.OpenApi.Any.IOpenApiAny? GetExampleForSchema(string schemaName)
        {
            // Add common examples
            return schemaName switch
            {
                "LoginDto" => new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["email"] = new Microsoft.OpenApi.Any.OpenApiString("john.doe@tekno.com"),
                    ["password"] = new Microsoft.OpenApi.Any.OpenApiString("User123!")
                },
                "RegisterDto" => new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["fullName"] = new Microsoft.OpenApi.Any.OpenApiString("John Doe"),
                    ["email"] = new Microsoft.OpenApi.Any.OpenApiString("john.doe@tekno.com"),
                    ["password"] = new Microsoft.OpenApi.Any.OpenApiString("User123!"),
                    ["confirmPassword"] = new Microsoft.OpenApi.Any.OpenApiString("User123!")
                },
                _ => null
            };
        }
    }

    /// <summary>
    /// Adds descriptions to Swagger tags (controller groups)
    /// </summary>
    public class SwaggerTagDescriptions : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Provide a curated list of tags. Use a single 'Admin' tag for all admin controllers
            var tags = new List<OpenApiTag>
            {
                new OpenApiTag
                {
                    Name = "Admin",
                    Description = "**Admin Endpoints** - Requires Admin role. Group for all admin controllers (Admin*)."
                },
                new OpenApiTag
                {
                    Name = "Auth",
                    Description = "**Authentication & Authorization** - Register, login, token refresh"
                },
                new OpenApiTag
                {
                    Name = "Profile",
                    Description = "**User Profile Management** - View/update profile, manage addresses"
                },
                new OpenApiTag
                {
                    Name = "Product",
                    Description = "**Product Catalog** - Browse products, search, filter, view details"
                },
                new OpenApiTag
                {
                    Name = "Category",
                    Description = "**Product Categories** - Browse category tree, get category products"
                },
                new OpenApiTag
                {
                    Name = "Brand",
                    Description = "**Product Brands** - List brands, get brand products"
                },
                new OpenApiTag
                {
                    Name = "Cart",
                    Description = "**Shopping Cart** - Add/remove items, update quantities, partial checkout"
                },
                new OpenApiTag
                {
                    Name = "Wishlist",
                    Description = "**User Wishlist** - Save favorite products for later"
                },
                new OpenApiTag
                {
                    Name = "Payment",
                    Description = "**Payment Processing** - Process payments, check status, payment history"
                },
                new OpenApiTag
                {
                    Name = "Review",
                    Description = "**Product Reviews** - Submit reviews, ratings, view product reviews"
                },
                new OpenApiTag
                {
                    Name = "Coupon",
                    Description = "**Discount Coupons** - Apply coupons, check validity"
                },
                new OpenApiTag
                {
                    Name = "Blog",
                    Description = "**Blog Posts** - Read articles, news, guides"
                },
                new OpenApiTag
                {
                    Name = "Advertisement",
                    Description = "**Product Advertisements** - View promotional banners, featured products"
                }
            };

            // Merge with existing tags but avoid duplicate Admin* tags
            var existing = swaggerDoc.Tags ?? new List<OpenApiTag>();
            var merged = new List<OpenApiTag>(tags);

            foreach (var ex in existing)
            {
                // Skip existing tags that start with 'Admin' to avoid duplicates like AdminProduct
                if (ex.Name.StartsWith("Admin", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!merged.Any(t => string.Equals(t.Name, ex.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    merged.Add(ex);
                }
            }

            swaggerDoc.Tags = merged;
        }
    }
}
