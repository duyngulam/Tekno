using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tekno.Api.Services.Hosted
{
    /// <summary>
    /// One-time background service that fetches Vietnam province/district/ward data from external API
    /// and stores it to data/vietnam-divisions.json for manual import later.
    /// </summary>
    public class ProvinceFetchBackgroundService : BackgroundService
    {
        private readonly ILogger<ProvinceFetchBackgroundService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _outputPath;
        private readonly string _completionMarkerPath;
        private const string DEFAULT_API_URL = "https://provinces.open-api.vn/api/?depth=3";

        public ProvinceFetchBackgroundService(ILogger<ProvinceFetchBackgroundService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            
            var dataDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
            _outputPath = Path.Combine(dataDir, "vietnam-divisions.json");
            _completionMarkerPath = Path.Combine(dataDir, ".fetch-completed");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Province fetch background service starting");

            // Check if already fetched
            if (File.Exists(_completionMarkerPath))
            {
                _logger.LogInformation("Vietnam divisions already fetched (marker file exists). Skipping fetch.");
                return;
            }

            // Ensure output directory
            var dir = Path.GetDirectoryName(_outputPath)!;
            if (!Directory.Exists(dir)) 
                Directory.CreateDirectory(dir);

            try
            {
                await FetchAndStoreAsync(stoppingToken);
                
                // Mark as completed so it won't run again
                await File.WriteAllTextAsync(_completionMarkerPath, DateTime.UtcNow.ToString("O"), stoppingToken);
                _logger.LogInformation("Province fetch completed successfully. Marker file created.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch Vietnam divisions");
            }
        }

        private async Task FetchAndStoreAsync(CancellationToken token)
        {
            var apiUrl = Environment.GetEnvironmentVariable("VIETNAM_DIVISIONS_API_URL") ?? DEFAULT_API_URL;
            
            _logger.LogInformation("Fetching Vietnam divisions from {Url}", apiUrl);

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(5); // API might be slow
            
            var resp = await client.GetAsync(apiUrl, token);
            resp.EnsureSuccessStatusCode();
            
            var content = await resp.Content.ReadAsStringAsync(token);

            // Write content to file
            await File.WriteAllTextAsync(_outputPath, content, token);
            
            _logger.LogInformation("Downloaded Vietnam divisions and saved to {Path} ({Size} bytes)", 
                _outputPath, content.Length);
        }
    }
}
