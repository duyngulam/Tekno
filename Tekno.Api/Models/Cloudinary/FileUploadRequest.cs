using Microsoft.AspNetCore.Http;

namespace Tekno.Api.Models.Cloudinary
{
    public class FileUploadRequest
    {
        /// <summary>
        /// File to upload
        /// </summary>
        public IFormFile File { get; set; }
    }
}
