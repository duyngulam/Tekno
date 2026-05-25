using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tekno.Domain.Catalog;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Seeding
{
    public class TrainingProductImportService
    {
        private static readonly string[] ImageUrls =
        {
            "https://cdn.tgdd.vn/Products/Images/44/314838/dell-xps-13-plus-9320-i5-71013325-1-750x500.jpg",
            "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/t/e/text_ng_n_24__3_5.png",
            "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/g/r/group_744_2__7.png",
        };

        private readonly AppDbContext _context;
        private readonly ILogger<TrainingProductImportService> _logger;

        public TrainingProductImportService(AppDbContext context, ILogger<TrainingProductImportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TrainingProductImportResult> ImportFromCsvAsync(string csvPath, int seed = 42)
        {
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("Import CSV not found", csvPath);
            }

            var lines = File.ReadAllLines(csvPath)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            if (lines.Count <= 1)
            {
                throw new InvalidOperationException("Import CSV is empty");
            }

            var categories = await _context.Set<Category>()
                .Where(c => c.ParentId != null)
                .ToListAsync();

            if (categories.Count == 0)
            {
                categories = await _context.Set<Category>().ToListAsync();
            }

            var brands = await _context.Set<Brand>().ToListAsync();

            if (brands.Count == 0)
            {
                throw new InvalidOperationException("No brands found to assign products");
            }

            if (categories.Count == 0)
            {
                throw new InvalidOperationException("No categories found to assign products");
            }

            var rng = new Random(seed);
            var result = new TrainingProductImportResult();

            foreach (var line in lines.Skip(1))
            {
                var parts = SplitCsvLine(line);
                if (parts.Count < 2)
                {
                    continue;
                }

                if (!int.TryParse(parts[0], out var productId))
                {
                    continue;
                }

                var name = parts[1].Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                var exists = await _context.Set<Product>().AnyAsync(p => p.Id == productId);
                if (exists)
                {
                    result.Skipped++;
                    continue;
                }

                var category = categories[rng.Next(categories.Count)];
                var brand = brands[rng.Next(brands.Count)];
                var slug = BuildSlug($"{name}-{productId}");

                var basePrice = BuildPriceFromName(name, rng);

                var product = new Product
                {
                    Id = productId,
                    Name = name,
                    Slug = slug,
                    CategoryId = category.Id,
                    BrandId = brand.Id,
                    Status = "available",
                    BasePrice = basePrice,
                    Description = $"Training product: {name}",
                    Overview = $"Training product for {category.Name}",
                    Specs = "[]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                _context.Products.Add(product);

                var sku = $"TRN-{productId}-V1";
                var variant = new ProductVariant(productId, sku, basePrice, stock: 50, status: "available");
                _context.ProductVariants.Add(variant);

                var imageUrl = ImageUrls[rng.Next(ImageUrls.Length)];
                var image = new ProductImage(productId, imageUrl, isPrimary: true, sortOrder: 0);
                _context.ProductImages.Add(image);

                result.Created++;
            }

            await _context.SaveChangesAsync();
            return result;
        }

        private static string BuildSlug(string name)
        {
            var slug = name.ToLowerInvariant();
            slug = Regex.Replace(slug, @"[^a-z0-9]+", "-");
            slug = slug.Trim('-');
            return string.IsNullOrWhiteSpace(slug) ? "product" : slug;
        }

        private static decimal BuildPriceFromName(string name, Random rng)
        {
            if (name.Contains("High-end", StringComparison.OrdinalIgnoreCase))
            {
                return rng.Next(10_000_000, 30_000_000);
            }
            if (name.Contains("Mid-range", StringComparison.OrdinalIgnoreCase))
            {
                return rng.Next(3_000_000, 10_000_000);
            }
            return rng.Next(1_000_000, 5_000_000);
        }

        private static List<string> SplitCsvLine(string line)
        {
            var result = new List<string>();
            var current = "";
            var inQuotes = false;

            foreach (var ch in line)
            {
                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (ch == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                    continue;
                }

                current += ch;
            }

            result.Add(current);
            return result;
        }
    }

    public class TrainingProductImportResult
    {
        public int Created { get; set; }
        public int Skipped { get; set; }
    }
}
