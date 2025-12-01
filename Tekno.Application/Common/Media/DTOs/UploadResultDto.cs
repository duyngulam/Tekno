using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Application.Common.Media.DTOs
{
    public class UploadResultDto
    {
        public string Url { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Folder { get; set; } = string.Empty;
    }
}
