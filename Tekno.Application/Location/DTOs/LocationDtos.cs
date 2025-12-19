using System.Collections.Generic;

namespace Tekno.Application.Location.DTOs
{
    public class ProvinceDto
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Codename { get; set; } = string.Empty;
        public string DivisionType { get; set; } = string.Empty;
        public int? PhoneCode { get; set; }
    }

    public class DistrictDto
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Codename { get; set; } = string.Empty;
        public string DivisionType { get; set; } = string.Empty;
        public int ProvinceCode { get; set; }
    }

    public class WardDto
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Codename { get; set; } = string.Empty;
        public string DivisionType { get; set; } = string.Empty;
        public int DistrictCode { get; set; }
    }

    public class ImportResultDto
    {
        public int ProvincesImported { get; set; }
        public int DistrictsImported { get; set; }
        public int WardsImported { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
