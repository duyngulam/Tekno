using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Tekno.Application.Common.Interfaces;

namespace Tekno.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudinaryUrl = configuration["Cloudinary:Url"];
            if (string.IsNullOrEmpty(cloudinaryUrl))
                throw new ArgumentNullException("Cloudinary URL not configured.");

            _cloudinary = new Cloudinary(cloudinaryUrl);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            await using var stream = file.OpenReadStream();

            // folderPath sẽ được truyền từ logic domain (vd: "Category/Icon")
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folderPath.TrimEnd('/') // tránh lỗi double slash
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl?.AbsoluteUri ?? string.Empty;  
        }
        public async Task<bool> DeleteImageByUrlAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("URL không được rỗng", nameof(imageUrl));

            // Regex sửa lại, đúng cú pháp
            var match = Regex.Match(imageUrl, @"tekno/.+?(?:\.(png|jpg|jpeg))$", RegexOptions.IgnoreCase);
            if (!match.Success)
                throw new ArgumentException("URL không hợp lệ hoặc không tìm thấy public ID", nameof(imageUrl));

            var publicId = match.Groups[0].Value; 
            publicId = Path.ChangeExtension(publicId, null); 

            // In log publicId ra console
            Console.WriteLine($"Public ID được trích ra: {publicId}");

            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            return result.Result == "ok";
        }

    }
}
