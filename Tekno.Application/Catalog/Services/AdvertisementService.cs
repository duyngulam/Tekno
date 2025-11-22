using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Catalog.DTOs.Advertisement;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common;
using Tekno.Application.Common.Exceptions;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Common.Media.Services;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Services
{
    public class AdvertisementService
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IProductRepository _productRepository;
        private readonly MediaService _mediaService;
        private readonly IAppLogger<AdvertisementService> _logger;

        public AdvertisementService(
            IAdvertisementRepository advertisementRepository,
            IProductRepository productRepository,
            MediaService mediaService,
            IAppLogger<AdvertisementService> logger)
        {
            _advertisementRepository = advertisementRepository;
            _productRepository = productRepository;
            _mediaService = mediaService;
            _logger = logger;
        }

        public async Task<PagedResult<ProductAdvertisementDto>> GetPagedAsync(AdvertisementQueryDto query)
        {
            var paging = new PagingParams(query.Page, query.PageSize);

            var result = await _advertisementRepository.GetPagedAsync(
                query.Position,
                query.IsActive,
                query.OnlyCurrentlyActive,
                paging);

            var dtos = result.Data.Select(a => MapToDto(a)).ToList();

            return new PagedResult<ProductAdvertisementDto>(dtos, result.TotalRecords, result.Page, result.PageSize);
        }

        public async Task<ProductAdvertisementDto?> GetByIdAsync(int id)
        {
            var advertisement = await _advertisementRepository.GetByIdAsync(id);
            if (advertisement == null) return null;

            return MapToDto(advertisement);
        }

        public async Task<List<ProductAdvertisementDto>> GetByPositionAsync(string position)
        {
            var advertisements = await _advertisementRepository.GetActiveByPositionAsync(position);
            return advertisements.Select(a => MapToDto(a)).ToList();
        }

        public async Task<List<ProductAdvertisementDto>> GetCurrentlyActiveAsync()
        {
            var advertisements = await _advertisementRepository.GetCurrentlyActiveAsync();
            return advertisements.Select(a => MapToDto(a)).ToList();
        }

        public async Task<ProductAdvertisementDto> CreateAsync(CreateAdvertisementDto dto)
        {
            // Validate product exists
            var product = await _productRepository.GetProductByIdAsync(dto.ProductId);
            if (product == null)
            {
                throw new NotFoundException("Product", dto.ProductId);
            }

            // Upload image
            var imageUrl = await _mediaService.UploadImageAsync(dto.Image, "tekno/advertisements");

            try
            {
                var advertisement = new ProductAdvertisement(
                    productId: dto.ProductId,
                    imageUrl: imageUrl,
                    position: dto.Position,
                    priority: dto.Priority,
                    startDate: dto.StartDate,
                    endDate: dto.EndDate);

                if (!dto.IsActive)
                {
                    advertisement.Deactivate();
                }

                advertisement = await _advertisementRepository.CreateAsync(advertisement);

                _logger.LogInformation("Created advertisement {Id} for product {ProductId}", advertisement.Id, dto.ProductId);

                return MapToDto(advertisement);
            }
            catch
            {
                // Clean up uploaded image on error
                await _mediaService.DeleteImageAsync(imageUrl);
                throw;
            }
        }

        public async Task<ProductAdvertisementDto?> UpdateAsync(int id, UpdateAdvertisementDto dto)
        {
            var advertisement = await _advertisementRepository.GetByIdAsync(id);
            if (advertisement == null) return null;

            // Validate product exists
            var product = await _productRepository.GetProductByIdAsync(dto.ProductId);
            if (product == null)
            {
                throw new NotFoundException("Product", dto.ProductId);
            }

            string? oldImageUrl = null;

            // Upload new image if provided
            if (dto.Image != null)
            {
                oldImageUrl = advertisement.ImageUrl;
                var newImageUrl = await _mediaService.UploadImageAsync(dto.Image, "tekno/advertisements");
                advertisement.UpdateImage(newImageUrl);
            }

            try
            {
                advertisement.UpdateProduct(dto.ProductId);
                advertisement.UpdatePosition(dto.Position);
                advertisement.UpdatePriority(dto.Priority);
                advertisement.UpdateSchedule(dto.StartDate, dto.EndDate);

                if (dto.IsActive)
                    advertisement.Activate();
                else
                    advertisement.Deactivate();

                advertisement = await _advertisementRepository.UpdateAsync(advertisement);

                // Delete old image if new one was uploaded
                if (oldImageUrl != null)
                {
                    await _mediaService.DeleteImageAsync(oldImageUrl);
                }

                _logger.LogInformation("Updated advertisement {Id}", id);

                return MapToDto(advertisement);
            }
            catch
            {
                // Clean up new image on error
                if (dto.Image != null && oldImageUrl != null)
                {
                    await _mediaService.DeleteImageAsync(advertisement.ImageUrl);
                }
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var advertisement = await _advertisementRepository.GetByIdAsync(id);
            if (advertisement == null) return false;

            var success = await _advertisementRepository.DeleteAsync(id);

            if (success)
            {
                // Delete image from storage
                await _mediaService.DeleteImageAsync(advertisement.ImageUrl);
                _logger.LogInformation("Deleted advertisement {Id}", id);
            }

            return success;
        }

        public async Task<bool> ActivateAsync(int id)
        {
            var advertisement = await _advertisementRepository.GetByIdAsync(id);
            if (advertisement == null) return false;

            advertisement.Activate();
            await _advertisementRepository.UpdateAsync(advertisement);

            _logger.LogInformation("Activated advertisement {Id}", id);
            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var advertisement = await _advertisementRepository.GetByIdAsync(id);
            if (advertisement == null) return false;

            advertisement.Deactivate();
            await _advertisementRepository.UpdateAsync(advertisement);

            _logger.LogInformation("Deactivated advertisement {Id}", id);
            return true;
        }

        private ProductAdvertisementDto MapToDto(ProductAdvertisement advertisement)
        {
            return new ProductAdvertisementDto
            {
                Id = advertisement.Id,
                ProductId = advertisement.ProductId,
                ProductName = advertisement.Product?.Name ?? string.Empty,
                ProductSlug = advertisement.Product?.Slug ?? string.Empty,
                ImageUrl = advertisement.ImageUrl,
                Position = advertisement.Position,
                Priority = advertisement.Priority,
                IsActive = advertisement.IsActive,
                StartDate = advertisement.StartDate,
                EndDate = advertisement.EndDate,
                CreatedAt = advertisement.CreatedAt,
                IsCurrentlyActive = advertisement.IsCurrentlyActive()
            };
        }
    }
}
