using System;

namespace Tekno.Domain.Auth
{
    public class UserAddress
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string RecipientName { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public string AddressLine1 { get; private set; } = string.Empty;
        public string? AddressLine2 { get; private set; }
        public string City { get; private set; } = string.Empty;
        public string State { get; private set; } = string.Empty;
        public string PostalCode { get; private set; } = string.Empty;
        public string Country { get; private set; } = "Vietnam";
        public bool IsDefault { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        public User User { get; private set; } = null!;

        private UserAddress() { }

        public UserAddress(
            int userId,
            string recipientName,
            string phoneNumber,
            string addressLine1,
            string city,
            string state,
            string postalCode,
            string country = "Vietnam",
            string? addressLine2 = null,
            bool isDefault = false)
        {
            UserId = userId;
            RecipientName = recipientName.Trim();
            PhoneNumber = phoneNumber.Trim();
            AddressLine1 = addressLine1.Trim();
            AddressLine2 = addressLine2?.Trim();
            City = city.Trim();
            State = state.Trim();
            PostalCode = postalCode.Trim();
            Country = country.Trim();
            IsDefault = isDefault;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string recipientName,
            string phoneNumber,
            string addressLine1,
            string city,
            string state,
            string postalCode,
            string country,
            string? addressLine2 = null)
        {
            RecipientName = recipientName.Trim();
            PhoneNumber = phoneNumber.Trim();
            AddressLine1 = addressLine1.Trim();
            AddressLine2 = addressLine2?.Trim();
            City = city.Trim();
            State = state.Trim();
            PostalCode = postalCode.Trim();
            Country = country.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetDefault(bool isDefault)
        {
            IsDefault = isDefault;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
