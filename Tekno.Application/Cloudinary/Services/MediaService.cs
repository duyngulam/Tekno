using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Common.Interfaces;
using Tekno.Domain.Constants;

namespace Tekno.Application.Cloudinary.Services
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
            return await _cloudinary.UploadImageAsync(file, CloudinaryFolders.CategoryIcon);
        }

        public async Task<string> UploadProductImageAsync(IFormFile file)
        {
            return await _cloudinary.UploadImageAsync(file,CloudinaryFolders.ProductImage);
        }
    }
}
