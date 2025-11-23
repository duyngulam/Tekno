using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Common;
using Tekno.Application.Common.Exceptions;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Common.Paging;
using Tekno.Application.Promotion.DTOs;
using Tekno.Application.Promotion.Interface;
using Tekno.Domain.Promotion;

namespace Tekno.Application.Promotion.Services
{
    public class CouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<CouponService> _logger;

        public CouponService(
            ICouponRepository couponRepository,
            IMapper mapper,
            IAppLogger<CouponService> logger)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<CouponDto>> GetPagedCouponsAsync(
            string? search,
            string? status,
            DateTime? startDate,
            DateTime? endDate,
            int page = 1,
            int pageSize = 20)
        {
            var paging = new PagingParams(page, pageSize);
            var result = await _couponRepository.GetPagedAsync(search, status, startDate, endDate, paging);
            
            var dtos = _mapper.Map<List<CouponDto>>(result.Data);
            return new PagedResult<CouponDto>(dtos, result.TotalRecords, page, pageSize);
        }

        public async Task<CouponDto?> GetCouponByIdAsync(int id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            return coupon == null ? null : _mapper.Map<CouponDto>(coupon);
        }

        public async Task<CouponDto?> GetCouponByCodeAsync(string code)
        {
            var coupon = await _couponRepository.GetByCodeAsync(code);
            return coupon == null ? null : _mapper.Map<CouponDto>(coupon);
        }

        public async Task<List<CouponDto>> GetActiveCouponsAsync()
        {
            var coupons = await _couponRepository.GetActiveCouponsAsync();
            return _mapper.Map<List<CouponDto>>(coupons);
        }

        public async Task<CouponDto> CreateCouponAsync(CreateCouponDto dto)
        {
            // Validate code uniqueness
            if (await _couponRepository.IsCodeExistsAsync(dto.Code))
            {
                throw new ConflictException($"Coupon code '{dto.Code}' already exists", "COUPON_CODE_EXISTS");
            }

            // Validate dates
            if (dto.EndDate <= dto.StartDate)
            {
                throw new InvalidOperationException("End date must be after start date");
            }

            // Parse coupon type
            if (!Enum.TryParse<CouponType>(dto.Type, true, out var couponType))
            {
                throw new InvalidOperationException($"Invalid coupon type: {dto.Type}");
            }

            // Create coupon entity
            var coupon = new Coupon(
                code: dto.Code.Trim().ToUpperInvariant(),
                name: dto.Name,
                type: couponType,
                value: dto.Value,
                quantity: dto.Quantity,
                startDate: dto.StartDate,
                endDate: dto.EndDate,
                minPurchaseAmount: dto.MinPurchaseAmount,
                maxDiscountAmount: dto.MaxDiscountAmount,
                maxUsagePerUser: dto.MaxUsagePerUser,
                note: dto.Note
            );

            // Add applicable categories
            foreach (var categoryId in dto.ApplicableCategoryIds)
            {
                coupon.AddApplicableCategory(categoryId);
            }

            // Add applicable products
            foreach (var productId in dto.ApplicableProductIds)
            {
                coupon.AddApplicableProduct(productId);
            }

            var created = await _couponRepository.CreateAsync(coupon);
            _logger.LogInformation("Created coupon {Code} ({Name}) with {Quantity} available", 
                created.Code, created.Name, created.Quantity);

            return _mapper.Map<CouponDto>(created);
        }

        public async Task<CouponDto?> UpdateCouponAsync(int id, UpdateCouponDto dto)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
            {
                _logger.LogWarning("Update failed: Coupon with ID {Id} not found", id);
                return null;
            }

            // Validate dates
            if (dto.EndDate <= dto.StartDate)
            {
                throw new InvalidOperationException("End date must be after start date");
            }

            // Update coupon
            coupon.Update(
                name: dto.Name,
                value: dto.Value,
                quantity: dto.Quantity,
                startDate: dto.StartDate,
                endDate: dto.EndDate,
                minPurchaseAmount: dto.MinPurchaseAmount,
                maxDiscountAmount: dto.MaxDiscountAmount,
                maxUsagePerUser: dto.MaxUsagePerUser,
                note: dto.Note
            );

            // Update categories and products (clear and re-add)
            coupon.ApplicableCategories.Clear();
            foreach (var categoryId in dto.ApplicableCategoryIds)
            {
                coupon.AddApplicableCategory(categoryId);
            }

            coupon.ApplicableProducts.Clear();
            foreach (var productId in dto.ApplicableProductIds)
            {
                coupon.AddApplicableProduct(productId);
            }

            var updated = await _couponRepository.UpdateAsync(coupon);
            _logger.LogInformation("Updated coupon ID {Id} ({Code})", id, updated.Code);

            return _mapper.Map<CouponDto>(updated);
        }

        public async Task<bool> DeleteCouponAsync(int id)
        {
            var success = await _couponRepository.DeleteAsync(id);
            if (success)
            {
                _logger.LogInformation("Deleted coupon ID {Id}", id);
            }
            else
            {
                _logger.LogWarning("Delete failed: Coupon with ID {Id} not found", id);
            }
            return success;
        }

        public async Task<CouponValidationResult> ValidateCouponAsync(ValidateCouponDto dto)
        {
            var coupon = await _couponRepository.GetByCodeAsync(dto.Code);

            if (coupon == null)
            {
                return new CouponValidationResult
                {
                    IsValid = false,
                    Message = "Coupon code not found"
                };
            }

            if (!coupon.IsAvailable)
            {
                return new CouponValidationResult
                {
                    IsValid = false,
                    Message = coupon.IsExpired 
                        ? "Coupon has expired" 
                        : coupon.RemainingQuantity <= 0 
                            ? "Coupon has been fully redeemed" 
                            : "Coupon is not active"
                };
            }

            // Check minimum purchase amount
            if (coupon.MinPurchaseAmount.HasValue && dto.OrderAmount < coupon.MinPurchaseAmount.Value)
            {
                return new CouponValidationResult
                {
                    IsValid = false,
                    Message = $"Minimum purchase amount is {coupon.MinPurchaseAmount.Value:N0} VND"
                };
            }

            // Check user usage limit
            if (dto.UserId.HasValue && coupon.MaxUsagePerUser.HasValue)
            {
                var userUsageCount = await _couponRepository.GetUserCouponUsageCountAsync(coupon.Id, dto.UserId.Value);
                if (userUsageCount >= coupon.MaxUsagePerUser.Value)
                {
                    return new CouponValidationResult
                    {
                        IsValid = false,
                        Message = "You have reached the maximum usage limit for this coupon"
                    };
                }
            }

            // Check applicable categories (if specified)
            if (coupon.ApplicableCategories.Any())
            {
                var applicableCategoryIds = coupon.ApplicableCategories.Select(c => c.CategoryId).ToList();
                if (!dto.CategoryIds.Any(id => applicableCategoryIds.Contains(id)))
                {
                    return new CouponValidationResult
                    {
                        IsValid = false,
                        Message = "This coupon is not applicable to your cart items"
                    };
                }
            }

            // Check applicable products (if specified)
            if (coupon.ApplicableProducts.Any())
            {
                var applicableProductIds = coupon.ApplicableProducts.Select(p => p.ProductId).ToList();
                if (!dto.ProductIds.Any(id => applicableProductIds.Contains(id)))
                {
                    return new CouponValidationResult
                    {
                        IsValid = false,
                        Message = "This coupon is not applicable to your cart items"
                    };
                }
            }

            // Calculate discount
            var discountAmount = coupon.CalculateDiscount(dto.OrderAmount);

            return new CouponValidationResult
            {
                IsValid = true,
                Message = $"Coupon applied! You save {discountAmount:N0} VND",
                DiscountAmount = discountAmount,
                Coupon = _mapper.Map<CouponDto>(coupon)
            };
        }

        public async Task<bool> ApplyCouponAsync(string code, int userId, int orderId, decimal orderAmount)
        {
            var coupon = await _couponRepository.GetByCodeAsync(code);
            if (coupon == null || !coupon.IsAvailable)
            {
                return false;
            }

            var discountAmount = coupon.CalculateDiscount(orderAmount);

            // Record usage
            var usage = new CouponUsage
            {
                CouponId = coupon.Id,
                UserId = userId,
                OrderId = orderId,
                DiscountAmount = discountAmount,
                UsedAt = DateTime.UtcNow
            };

            await _couponRepository.RecordUsageAsync(usage);

            // Increment usage count
            coupon.IncrementUsage();
            await _couponRepository.UpdateAsync(coupon);

            _logger.LogInformation(
                "Coupon {Code} applied to order {OrderId} for user {UserId}. Discount: {Discount}", 
                code, orderId, userId, discountAmount);

            return true;
        }

        public async Task<List<CouponUsageDto>> GetUsageHistoryAsync(int couponId, int page = 1, int pageSize = 20)
        {
            var paging = new PagingParams(page, pageSize);
            var usages = await _couponRepository.GetUsageHistoryAsync(couponId, paging);
            return _mapper.Map<List<CouponUsageDto>>(usages);
        }
    }
}
