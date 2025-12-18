using System;

namespace Tekno.Domain.Location
{
    public class Ward
    {
        public int Id { get; private set; }
        public int Code { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Codename { get; private set; } = string.Empty;
        public string DivisionType { get; private set; } = string.Empty;
        public int DistrictCode { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        // Navigation
        public District District { get; private set; } = null!;

        private Ward() { }

        public Ward(int code, string name, string codename, string divisionType, int districtCode)
        {
            Code = code;
            Name = name;
            Codename = codename ?? string.Empty;
            DivisionType = divisionType ?? string.Empty;
            DistrictCode = districtCode;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string codename, string divisionType)
        {
            Name = name;
            Codename = codename ?? string.Empty;
            DivisionType = divisionType ?? string.Empty;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
