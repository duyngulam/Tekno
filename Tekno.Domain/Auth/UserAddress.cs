using System;

namespace Tekno.Domain.Auth
{
    public class UserAddress
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string RecipientName { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        
        // Detailed address
        public string AddressLine { get; private set; } = string.Empty; // Street, building, apartment
        
        // Vietnam location system
        public int ProvinceCode { get; private set; }
        public int DistrictCode { get; private set; }
        public int WardCode { get; private set; }
        
        // Cached names for display (updated when codes change)
        public string ProvinceName { get; private set; } = string.Empty;
        public string DistrictName { get; private set; } = string.Empty;
        public string WardName { get; private set; } = string.Empty;
        
        public bool IsDefault { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        public User User { get; private set; } = null!;

        private UserAddress() { }

        public UserAddress(
            int userId,
            string recipientName,
            string phoneNumber,
            string addressLine,
            int provinceCode,
            string provinceName,
            int districtCode,
            string districtName,
            int wardCode,
            string wardName,
            bool isDefault = false)
        {
            UserId = userId;
            RecipientName = recipientName.Trim();
            PhoneNumber = phoneNumber.Trim();
            AddressLine = addressLine.Trim();
            ProvinceCode = provinceCode;
            ProvinceName = provinceName.Trim();
            DistrictCode = districtCode;
            DistrictName = districtName.Trim();
            WardCode = wardCode;
            WardName = wardName.Trim();
            IsDefault = isDefault;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string recipientName,
            string phoneNumber,
            string addressLine,
            int provinceCode,
            string provinceName,
            int districtCode,
            string districtName,
            int wardCode,
            string wardName)
        {
            RecipientName = recipientName.Trim();
            PhoneNumber = phoneNumber.Trim();
            AddressLine = addressLine.Trim();
            ProvinceCode = provinceCode;
            ProvinceName = provinceName.Trim();
            DistrictCode = districtCode;
            DistrictName = districtName.Trim();
            WardCode = wardCode;
            WardName = wardName.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetDefault(bool isDefault)
        {
            IsDefault = isDefault;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Get full address as formatted string
        /// </summary>
        public string GetFullAddress()
        {
            return $"{AddressLine}, {WardName}, {DistrictName}, {ProvinceName}";
        }
    }
}
