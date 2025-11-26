using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Tekno.Api.Filters
{
    /// <summary>
    /// Swagger schema filter to add examples for enums and fixed values
    /// </summary>
    public class SwaggerSchemaExampleFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            // Handle enum types
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();
                var enumValues = Enum.GetValues(context.Type);
                foreach (var value in enumValues)
                {
                    schema.Enum.Add(new OpenApiString(value.ToString()));
                }
                
                schema.Type = "string";
                schema.Example = new OpenApiString(enumValues.GetValue(0)?.ToString() ?? "");
            }

            // Add examples for specific DTO properties
            if (context.Type.Name == "CreateCouponDto" || context.Type.Name == "UpdateCouponDto")
            {
                if (schema.Properties.ContainsKey("type"))
                {
                    schema.Properties["type"].Example = new OpenApiString("FixedAmount");
                    schema.Properties["type"].Description = "Coupon type: FixedAmount, Percentage, or FreeShipping";
                    schema.Properties["type"].Enum = new List<IOpenApiAny>
                    {
                        new OpenApiString("FixedAmount"),
                        new OpenApiString("Percentage"),
                        new OpenApiString("FreeShipping")
                    };
                }

                if (schema.Properties.ContainsKey("code"))
                {
                    schema.Properties["code"].Example = new OpenApiString("SUMMER2025");
                }

                if (schema.Properties.ContainsKey("name"))
                {
                    schema.Properties["name"].Example = new OpenApiString("Summer Sale 2025");
                }

                if (schema.Properties.ContainsKey("value"))
                {
                    schema.Properties["value"].Example = new OpenApiDouble(50000);
                }

                if (schema.Properties.ContainsKey("quantity"))
                {
                    schema.Properties["quantity"].Example = new OpenApiInteger(100);
                }

                if (schema.Properties.ContainsKey("startDate"))
                {
                    schema.Properties["startDate"].Example = new OpenApiString(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                }

                if (schema.Properties.ContainsKey("endDate"))
                {
                    schema.Properties["endDate"].Example = new OpenApiString(DateTime.UtcNow.AddMonths(1).ToString("yyyy-MM-ddTHH:mm:ssZ"));
                }
            }

            // Add examples for RegisterRequest
            if (context.Type.Name == "RegisterRequest")
            {
                if (schema.Properties.ContainsKey("email"))
                {
                    schema.Properties["email"].Example = new OpenApiString("user@example.com");
                }

                if (schema.Properties.ContainsKey("password"))
                {
                    schema.Properties["password"].Example = new OpenApiString("SecurePass123");
                }

                if (schema.Properties.ContainsKey("role"))
                {
                    schema.Properties["role"].Example = new OpenApiString("Customer");
                    schema.Properties["role"].Description = "User role: Customer or Admin";
                    schema.Properties["role"].Enum = new List<IOpenApiAny>
                    {
                        new OpenApiString("Customer"),
                        new OpenApiString("Admin")
                    };
                }
            }

            // Add examples for profile DTOs
            if (context.Type.Name == "UpdateProfileDto")
            {
                if (schema.Properties.ContainsKey("fullname"))
                {
                    schema.Properties["fullname"].Example = new OpenApiString("John Doe");
                }

                if (schema.Properties.ContainsKey("phoneNumber"))
                {
                    schema.Properties["phoneNumber"].Example = new OpenApiString("+84987654321");
                }
            }

            if (context.Type.Name == "UpdateEmailDto")
            {
                if (schema.Properties.ContainsKey("newEmail"))
                {
                    schema.Properties["newEmail"].Example = new OpenApiString("newemail@example.com");
                }

                if (schema.Properties.ContainsKey("currentPassword"))
                {
                    schema.Properties["currentPassword"].Example = new OpenApiString("CurrentPassword123");
                }
            }

            if (context.Type.Name == "ChangePasswordDto")
            {
                if (schema.Properties.ContainsKey("currentPassword"))
                {
                    schema.Properties["currentPassword"].Example = new OpenApiString("OldPassword123");
                }

                if (schema.Properties.ContainsKey("newPassword"))
                {
                    schema.Properties["newPassword"].Example = new OpenApiString("NewPassword456");
                }

                if (schema.Properties.ContainsKey("confirmPassword"))
                {
                    schema.Properties["confirmPassword"].Example = new OpenApiString("NewPassword456");
                }
            }

            if (context.Type.Name == "CreateAddressDto")
            {
                if (schema.Properties.ContainsKey("recipientName"))
                {
                    schema.Properties["recipientName"].Example = new OpenApiString("John Doe");
                }

                if (schema.Properties.ContainsKey("phoneNumber"))
                {
                    schema.Properties["phoneNumber"].Example = new OpenApiString("+84987654321");
                }

                if (schema.Properties.ContainsKey("addressLine1"))
                {
                    schema.Properties["addressLine1"].Example = new OpenApiString("123 Nguyen Hue Street");
                }

                if (schema.Properties.ContainsKey("addressLine2"))
                {
                    schema.Properties["addressLine2"].Example = new OpenApiString("Apartment 5B");
                }

                if (schema.Properties.ContainsKey("city"))
                {
                    schema.Properties["city"].Example = new OpenApiString("Ho Chi Minh City");
                }

                if (schema.Properties.ContainsKey("state"))
                {
                    schema.Properties["state"].Example = new OpenApiString("Ho Chi Minh");
                }

                if (schema.Properties.ContainsKey("postalCode"))
                {
                    schema.Properties["postalCode"].Example = new OpenApiString("700000");
                }

                if (schema.Properties.ContainsKey("country"))
                {
                    schema.Properties["country"].Example = new OpenApiString("Vietnam");
                }

                if (schema.Properties.ContainsKey("isDefault"))
                {
                    schema.Properties["isDefault"].Example = new OpenApiBoolean(true);
                }
            }
        }
    }
}
