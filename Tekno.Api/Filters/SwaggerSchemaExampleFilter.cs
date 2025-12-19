using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace Tekno.Api.Filters
{
    /// <summary>
    /// Swagger schema filter to add examples for enums and fixed values
    /// </summary>
    public class SwaggerSchemaExampleFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            // Handle enum types - show as integers
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();
                schema.Type = "integer";
                schema.Format = "int32";
                
                var enumValues = Enum.GetValues(context.Type);
                var enumDescriptions = new List<string>();
                
                foreach (var value in enumValues)
                {
                    var intValue = (int)value;
                    schema.Enum.Add(new OpenApiInteger(intValue));
                    enumDescriptions.Add($"{intValue} = {value}");
                }
                
                // Add description showing all enum values
                schema.Description = $"{context.Type.Name} values: {string.Join(", ", enumDescriptions)}";
                
                // Set first value as example
                if (enumValues.Length > 0)
                {
                    schema.Example = new OpenApiInteger((int)enumValues.GetValue(0)!);
                }
            }

            // Add specific examples for Payment DTOs
            if (context.Type.Name == "PaymentRequestDto")
            {
                if (schema.Properties.ContainsKey("gateway"))
                {
                    schema.Properties["gateway"].Example = new OpenApiInteger(0);
                    schema.Properties["gateway"].Description = "Payment Gateway: 0=Mock, 1=Stripe, 2=PayPal, 3=VNPay, 4=MoMo, 5=ZaloPay";
                }

                if (schema.Properties.ContainsKey("method"))
                {
                    schema.Properties["method"].Example = new OpenApiInteger(1);
                    schema.Properties["method"].Description = "Payment Method: 1=CreditCard, 2=DebitCard, 3=BankTransfer, 4=EWallet, 5=Cash";
                }

                if (schema.Properties.ContainsKey("returnUrl"))
                {
                    schema.Properties["returnUrl"].Example = new OpenApiString("http://localhost:3000/payment/result");
                }

                if (schema.Properties.ContainsKey("shippingAddressId"))
                {
                    schema.Properties["shippingAddressId"].Example = new OpenApiInteger(1);
                }
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

            // Add examples for Advertisement DTOs
            if (context.Type.Name == "CreateAdvertisementDto" || context.Type.Name == "UpdateAdvertisementDto" || 
                context.Type.Name == "AdvertisementQueryDto")
            {
                if (schema.Properties.ContainsKey("position"))
                {
                    schema.Properties["position"].Example = new OpenApiString("HomeTop");
                    schema.Properties["position"].Description = "Advertisement position: HomeTop, HomeMiddle, HomeBottom, CategoryTop, or ProductSidebar";
                    schema.Properties["position"].Enum = new List<IOpenApiAny>
                    {
                        new OpenApiString("HomeTop"),
                        new OpenApiString("HomeMiddle"),
                        new OpenApiString("HomeBottom"),
                        new OpenApiString("CategoryTop"),
                        new OpenApiString("ProductSidebar")
                    };
                }

                if (schema.Properties.ContainsKey("priority"))
                {
                    schema.Properties["priority"].Example = new OpenApiInteger(10);
                    schema.Properties["priority"].Description = "Display priority (0-100). Higher number = shown first";
                }

                if (schema.Properties.ContainsKey("productId"))
                {
                    schema.Properties["productId"].Example = new OpenApiInteger(1);
                }

                if (schema.Properties.ContainsKey("isActive"))
                {
                    schema.Properties["isActive"].Example = new OpenApiBoolean(true);
                }

                if (schema.Properties.ContainsKey("startDate"))
                {
                    schema.Properties["startDate"].Example = new OpenApiString(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                }

                if (schema.Properties.ContainsKey("endDate"))
                {
                    schema.Properties["endDate"].Example = new OpenApiString(DateTime.UtcNow.AddMonths(3).ToString("yyyy-MM-ddTHH:mm:ssZ"));
                }
            }
        }
    }
}
