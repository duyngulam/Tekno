using System;
using System.Collections.Generic;

namespace Tekno.Domain.Location
{
    public class Province
    {
        public int Id { get; private set; }
        public int Code { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Codename { get; private set; } = string.Empty;
        public string DivisionType { get; private set; } = string.Empty;
        public int? PhoneCode { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        // Navigation
        public ICollection<District> Districts { get; private set; } = new List<District>();

        private Province() { }

        public Province(int code, string name, string codename, string divisionType, int? phoneCode)
        {
            Code = code;
            Name = name;
            Codename = codename ?? string.Empty;
            DivisionType = divisionType ?? string.Empty;
            PhoneCode = phoneCode;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string codename, string divisionType, int? phoneCode)
        {
            Name = name;
            Codename = codename ?? string.Empty;
            DivisionType = divisionType ?? string.Empty;
            PhoneCode = phoneCode;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
