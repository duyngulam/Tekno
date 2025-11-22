using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Auth.DTOs;
using Tekno.Application.Auth.Interfaces;
using Tekno.Application.Common.Exceptions;
using Tekno.Application.Common.Interfaces;
using Tekno.Domain.Auth;

namespace Tekno.Application.Auth.Services
{
    public class ProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly IAppLogger<ProfileService> _logger;

        public ProfileService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IMapper mapper,
            IAppLogger<ProfileService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get user profile with addresses
        /// </summary>
        public async Task<UserProfileDto?> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdWithAddressesAsync(userId);
            if (user == null) return null;

            return _mapper.Map<UserProfileDto>(user);
        }

        /// <summary>
        /// Update user profile (fullname, phone)
        /// </summary>
        public async Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }

            user.UpdateProfile(dto.Fullname, dto.PhoneNumber);
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {UserId} updated profile", userId);

            return _mapper.Map<UserProfileDto>(user);
        }

        /// <summary>
        /// Update user email (requires password verification)
        /// </summary>
        public async Task<UserProfileDto> UpdateEmailAsync(int userId, UpdateEmailDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }

            // Verify current password
            if (!_passwordHasher.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                throw new InvalidOperationException("Current password is incorrect");
            }

            // Check if new email already exists
            if (await _userRepository.EmailExistsAsync(dto.NewEmail, userId))
            {
                throw new ConflictException($"Email '{dto.NewEmail}' is already in use", "EMAIL_EXISTS");
            }

            user.UpdateEmail(dto.NewEmail);
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {UserId} updated email to {NewEmail}", userId, dto.NewEmail);

            return _mapper.Map<UserProfileDto>(user);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }

            // Verify current password
            if (!_passwordHasher.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                throw new InvalidOperationException("Current password is incorrect");
            }

            // Hash new password
            var newPasswordHash = _passwordHasher.Hash(dto.NewPassword);
            user.UpdatePassword(newPasswordHash);
            
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {UserId} changed password", userId);

            return true;
        }

        /// <summary>
        /// Get all addresses for user
        /// </summary>
        public async Task<List<UserAddressDto>> GetAddressesAsync(int userId)
        {
            var user = await _userRepository.GetByIdWithAddressesAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }

            return _mapper.Map<List<UserAddressDto>>(user.Addresses.OrderByDescending(a => a.IsDefault).ThenByDescending(a => a.CreatedAt));
        }

        /// <summary>
        /// Add new address
        /// </summary>
        public async Task<UserAddressDto> AddAddressAsync(int userId, CreateAddressDto dto)
        {
            var user = await _userRepository.GetByIdWithAddressesAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }

            // If this is the first address, make it default
            var isFirstAddress = !user.Addresses.Any();
            var isDefault = dto.IsDefault || isFirstAddress;

            // If setting as default, unset others
            if (isDefault)
            {
                foreach (var addr in user.Addresses)
                {
                    addr.SetDefault(false);
                }
            }

            var address = new UserAddress(
                userId: userId,
                recipientName: dto.RecipientName,
                phoneNumber: dto.PhoneNumber,
                addressLine1: dto.AddressLine1,
                city: dto.City,
                state: dto.State,
                postalCode: dto.PostalCode,
                country: dto.Country,
                addressLine2: dto.AddressLine2,
                isDefault: isDefault);

            user.AddAddress(address);
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {UserId} added new address", userId);

            return _mapper.Map<UserAddressDto>(address);
        }

        /// <summary>
        /// Update existing address
        /// </summary>
        public async Task<UserAddressDto> UpdateAddressAsync(int userId, int addressId, UpdateAddressDto dto)
        {
            var address = await _userRepository.GetAddressByIdAsync(addressId);
            if (address == null)
            {
                throw new NotFoundException("Address", addressId);
            }

            // Verify ownership
            if (address.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only update your own addresses");
            }

            address.Update(
                recipientName: dto.RecipientName,
                phoneNumber: dto.PhoneNumber,
                addressLine1: dto.AddressLine1,
                city: dto.City,
                state: dto.State,
                postalCode: dto.PostalCode,
                country: dto.Country,
                addressLine2: dto.AddressLine2);

            await _userRepository.UpdateAddressAsync(address);

            _logger.LogInformation("User {UserId} updated address {AddressId}", userId, addressId);

            return _mapper.Map<UserAddressDto>(address);
        }

        /// <summary>
        /// Set address as default
        /// </summary>
        public async Task<bool> SetDefaultAddressAsync(int userId, int addressId)
        {
            var user = await _userRepository.GetByIdWithAddressesAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }

            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
            {
                throw new NotFoundException("Address", addressId);
            }

            user.SetDefaultAddress(addressId);
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {UserId} set address {AddressId} as default", userId, addressId);

            return true;
        }

        /// <summary>
        /// Delete address
        /// </summary>
        public async Task<bool> DeleteAddressAsync(int userId, int addressId)
        {
            var address = await _userRepository.GetAddressByIdAsync(addressId);
            if (address == null)
            {
                return false;
            }

            // Verify ownership
            if (address.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only delete your own addresses");
            }

            // Prevent deleting the only address if it's default
            var user = await _userRepository.GetByIdWithAddressesAsync(userId);
            if (user != null && address.IsDefault && user.Addresses.Count == 1)
            {
                throw new InvalidOperationException("Cannot delete the only address");
            }

            var success = await _userRepository.DeleteAddressAsync(addressId);

            if (success)
            {
                _logger.LogInformation("User {UserId} deleted address {AddressId}", userId, addressId);

                // If deleted address was default, set another as default
                if (address.IsDefault && user != null && user.Addresses.Count > 1)
                {
                    var newDefaultAddress = user.Addresses.First(a => a.Id != addressId);
                    user.SetDefaultAddress(newDefaultAddress.Id);
                    await _userRepository.UpdateAsync(user);
                }
            }

            return success;
        }
    }
}
