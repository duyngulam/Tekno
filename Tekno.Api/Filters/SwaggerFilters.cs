using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

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

            // Ensure all 200 responses include a concrete example payload
            ApplyDefaultSuccessExample(operation, context);

            // If controller name starts with 'Admin', add an additional 'Admin' tag to the operation
            var controllerName = context.MethodInfo.DeclaringType?.Name;
            if (!string.IsNullOrEmpty(controllerName) && controllerName.StartsWith("Admin", StringComparison.OrdinalIgnoreCase))
            {
                if (operation.Tags == null)
                    operation.Tags = new System.Collections.Generic.List<OpenApiTag>();

                if (!operation.Tags.Any(t => string.Equals(t.Name, "Admin", StringComparison.OrdinalIgnoreCase)))
                {
                    // Insert Admin as first tag so it appears in tag filters easily
                    operation.Tags.Insert(0, new OpenApiTag { Name = "Admin" });
                }
            }
            else if (operation.Tags != null && operation.Tags.Any(t => t?.Name != null && t.Name.StartsWith("Admin", StringComparison.OrdinalIgnoreCase)))
            {
                if (!operation.Tags.Any(t => string.Equals(t.Name, "Admin", StringComparison.OrdinalIgnoreCase)))
                {
                    operation.Tags.Insert(0, new OpenApiTag { Name = "Admin" });
                }
            }
        }

        private static void ApplyDefaultSuccessExample(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!operation.Responses.TryGetValue("200", out var okResponse))
            {
                return;
            }

            if (okResponse.Content == null || okResponse.Content.Count == 0)
            {
                var responseType = context.ApiDescription.SupportedResponseTypes
                    .FirstOrDefault(r => r.StatusCode == 200)?.Type;

                if (responseType != null && responseType != typeof(void))
                {
                    var schema = context.SchemaGenerator.GenerateSchema(responseType, context.SchemaRepository);
                    okResponse.Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = schema
                        }
                    };
                }
            }

            if (okResponse.Content == null || okResponse.Content.Count == 0)
            {
                return;
            }

            foreach (var media in okResponse.Content)
            {
                var mediaType = media.Value;
                if (mediaType.Example != null || mediaType.Schema == null)
                {
                    continue;
                }

                mediaType.Example = BuildExampleFromSchema(mediaType.Schema, context.SchemaRepository, 0);
            }
        }

        private static Microsoft.OpenApi.Any.IOpenApiAny BuildExampleFromSchema(
            OpenApiSchema schema,
            SchemaRepository schemaRepository,
            int depth)
        {
            if (depth > 3)
            {
                return new Microsoft.OpenApi.Any.OpenApiString("...");
            }

            var resolvedSchema = ResolveSchema(schema, schemaRepository);

            // Normalize ApiResponse<T> style payload to keep docs consistent across endpoints.
            if (IsApiResponseSchema(resolvedSchema))
            {
                var apiResponse = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["success"] = new Microsoft.OpenApi.Any.OpenApiBoolean(true),
                    ["message"] = new Microsoft.OpenApi.Any.OpenApiString("Success"),
                    ["timestamp"] = new Microsoft.OpenApi.Any.OpenApiString(DateTime.UtcNow.ToString("O"))
                };

                if (resolvedSchema.Properties.TryGetValue("data", out var dataSchema))
                {
                    apiResponse["data"] = BuildExampleFromSchema(dataSchema, schemaRepository, depth + 1);
                }

                if (resolvedSchema.Properties.ContainsKey("errors"))
                {
                    apiResponse["errors"] = new Microsoft.OpenApi.Any.OpenApiObject();
                }

                return apiResponse;
            }

            if (resolvedSchema.Enum != null && resolvedSchema.Enum.Count > 0)
            {
                return resolvedSchema.Enum[0];
            }

            var type = resolvedSchema.Type?.ToLowerInvariant();
            switch (type)
            {
                case "boolean":
                    return new Microsoft.OpenApi.Any.OpenApiBoolean(true);
                case "integer":
                    return new Microsoft.OpenApi.Any.OpenApiInteger(1);
                case "number":
                    return new Microsoft.OpenApi.Any.OpenApiDouble(1.0);
                case "string":
                    return BuildStringExample(resolvedSchema.Format);
                case "array":
                {
                    var array = new Microsoft.OpenApi.Any.OpenApiArray();
                    if (resolvedSchema.Items != null)
                    {
                        array.Add(BuildExampleFromSchema(resolvedSchema.Items, schemaRepository, depth + 1));
                    }
                    return array;
                }
                case "object":
                {
                    var obj = new Microsoft.OpenApi.Any.OpenApiObject();
                    foreach (var prop in resolvedSchema.Properties.Take(6))
                    {
                        obj[prop.Key] = BuildExampleFromSchema(prop.Value, schemaRepository, depth + 1);
                    }
                    return obj;
                }
            }

            // Fallback for schemas without explicit type but with properties.
            if (resolvedSchema.Properties != null && resolvedSchema.Properties.Count > 0)
            {
                var obj = new Microsoft.OpenApi.Any.OpenApiObject();
                foreach (var prop in resolvedSchema.Properties.Take(6))
                {
                    obj[prop.Key] = BuildExampleFromSchema(prop.Value, schemaRepository, depth + 1);
                }
                return obj;
            }

            return new Microsoft.OpenApi.Any.OpenApiString("sample");
        }

        private static OpenApiSchema ResolveSchema(OpenApiSchema schema, SchemaRepository schemaRepository)
        {
            if (schema.Reference?.Id != null && schemaRepository.Schemas.TryGetValue(schema.Reference.Id, out var resolved))
            {
                return resolved;
            }

            return schema;
        }

        private static bool IsApiResponseSchema(OpenApiSchema schema)
        {
            return schema.Properties.ContainsKey("success")
                   && schema.Properties.ContainsKey("message")
                   && schema.Properties.ContainsKey("data");
        }

        private static Microsoft.OpenApi.Any.IOpenApiAny BuildStringExample(string? format)
        {
            return format switch
            {
                "date-time" => new Microsoft.OpenApi.Any.OpenApiString(DateTime.UtcNow.ToString("O")),
                "date" => new Microsoft.OpenApi.Any.OpenApiString(DateTime.UtcNow.ToString("yyyy-MM-dd")),
                "uuid" => new Microsoft.OpenApi.Any.OpenApiString(Guid.NewGuid().ToString()),
                _ => new Microsoft.OpenApi.Any.OpenApiString("string")
            };
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
    /// Adds descriptions to Swagger tags (controller groups) and normalizes operation tags
    /// </summary>
    public class SwaggerTagDescriptions : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Provide a curated list of tags. Keep controller-specific Admin* tags, but also include a general 'Admin' tag
            var tags = new System.Collections.Generic.List<OpenApiTag>
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
                    Name = "Categories",
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

            // Merge with existing tags keeping existing Admin* tags as well
            var existing = swaggerDoc.Tags ?? new System.Collections.Generic.List<OpenApiTag>();
            var merged = new System.Collections.Generic.List<OpenApiTag>(tags);

            foreach (var ex in existing)
            {
                if (!merged.Any(t => string.Equals(t.Name, ex.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    merged.Add(ex);
                }
            }

            // Sort tags alphabetically by Name
            merged = merged.OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase).ToList();

            // Assign merged tags to document
            swaggerDoc.Tags = merged;

            // Normalize operation tags across all paths: ensure Admin* operations also contain a general 'Admin' tag
            foreach (var path in swaggerDoc.Paths)
            {
                var pathItem = path.Value;

                foreach (var op in pathItem.Operations.Values)
                {
                    var opTags = op.Tags;
                    if (opTags != null && opTags.Any(t => t?.Name != null && t.Name.StartsWith("Admin", StringComparison.OrdinalIgnoreCase)))
                    {
                        if (!opTags.Any(t => string.Equals(t.Name, "Admin", StringComparison.OrdinalIgnoreCase)))
                        {
                            // Keep existing AdminProduct etc tags, but also add Admin
                            op.Tags.Insert(0, new OpenApiTag { Name = "Admin" });
                        }
                    }
                }
            }
        }
    }
}
