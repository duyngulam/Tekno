using System;
using System.Collections.Generic;

namespace Tekno.Domain.Location
{
    public class District
    {
        public int Id { get; private set; }
        public int Code { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Codename { get; private set; } = string.Empty;
        public string DivisionType { get; private set; } = string.Empty;
        public int ProvinceCode { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        // Navigation
        public Province Province { get; private set; } = null!;
        public ICollection<Ward> Wards { get; private set; } = new List<Ward>();

        private District() { }

        public District(int code, string name, string codename, string divisionType, int provinceCode)
        {
            Code = code;
            Name = name;
            Codename = codename ?? string.Empty;
            DivisionType = divisionType ?? string.Empty;
            ProvinceCode = provinceCode;
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
