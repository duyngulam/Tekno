using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tekno.Application.Common.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folderPath);
        Task<bool> DeleteImageAsync(string publicId);
    }
}
