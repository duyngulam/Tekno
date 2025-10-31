using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Common.Interfaces;
using Tekno.Domain.Constants;

namespace Tekno.Application.Common.Media.Services
{
    public class MediaService
    {
        private readonly ICloudinaryService _cloudinary;
        public MediaService(ICloudinaryService cloudinary)
        {
            _cloudinary = cloudinary;
        }
        public async Task<string> UploadCategoryIconAsync(IFormFile file)
        {
            ValidateFile(file);
            return await _cloudinary.UploadImageAsync(file, CloudinaryFolders.CategoryIcon);
        }
        public async Task<string> UploadBrandLogoAsync(IFormFile file)
        {
            ValidateFile(file);
            return await _cloudinary.UploadImageAsync(file, CloudinaryFolders.BrandLogo);
        }
        public async Task<string> UploadProductImageAsync(IFormFile file)
        {
            ValidateFile(file);
            return await _cloudinary.UploadImageAsync(file, CloudinaryFolders.ProductImage);
        }
        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            ValidateFile(file);
            return await _cloudinary.UploadImageAsync(file, folder);
        }
        public async Task<bool> DeleteImageAsync(string url)
        {
            return await _cloudinary.DeleteImageByUrlAsync(url);
        }
        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ValidationException("File cannot be empty.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                throw new ValidationException("Invalid image format. Allowed: jpg, jpeg, png, webp");

            if (file.Length > 2 * 1024 * 1024) // 2MB
                throw new ValidationException("File size must be under 2MB.");
        }

    }
}