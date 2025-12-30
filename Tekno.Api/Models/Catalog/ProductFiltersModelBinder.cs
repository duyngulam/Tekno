using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tekno.Api.Models.Catalog
{
    /// <summary>
    /// Custom model binder for product search filters
    /// Binds query parameters like ?filters[Color]=Black, ?filters.Color=Black,
    /// or repeated parameters ?filters[Color]=Black&filters[Color]=White into a Dictionary<string, string>
    /// Supports multiple occurrences: they will be joined with comma to represent OR/Union semantics.
    /// </summary>
    public class ProductFiltersModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var query = bindingContext.HttpContext.Request.Query;

            // Accept keys in formats: filters[Name], filters.Name, filters%5BName%5D (decoded)
            foreach (var param in query)
            {
                var key = param.Key;
                string attributeName = null;

                if (key.StartsWith("filters[", StringComparison.OrdinalIgnoreCase) && key.EndsWith("]", StringComparison.OrdinalIgnoreCase))
                {
                    attributeName = key.Substring(8, key.Length - 9);
                }
                else if (key.StartsWith("filters.", StringComparison.OrdinalIgnoreCase))
                {
                    attributeName = key.Substring(8);
                }

                if (!string.IsNullOrEmpty(attributeName))
                {
                    // Join multiple values (either repeated param or comma-separated in a value)
                    var joined = string.Join(",", param.Value.ToArray()).Trim();
                    if (!string.IsNullOrEmpty(joined))
                    {
                        // Normalize: trim each value and remove duplicate empties
                        var parts = joined.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < parts.Length; i++)
                            parts[i] = parts[i].Trim();

                        var normalized = string.Join(',', parts);
                        result[attributeName] = normalized;
                    }
                }
            }

            bindingContext.Result = ModelBindingResult.Success(result);
            return Task.CompletedTask;
        }
    }
}
