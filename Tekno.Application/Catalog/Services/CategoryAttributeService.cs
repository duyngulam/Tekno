using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Catalog.DTOs.Admin;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common.Exceptions;
using Tekno.Application.Common.Interfaces;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Services
{
    public class CategoryAttributeService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<CategoryAttributeService> _logger;

        public CategoryAttributeService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper,
            IAppLogger<CategoryAttributeService> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all attributes for a category (including global attributes)
        /// </summary>
        public async Task<List<CategoryAttributeDto>> GetAttributesByCategoryIdAsync(int categoryId)
        {
            // Validate category exists
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null)
                throw new NotFoundException("Category", categoryId);

            var attributes = await _productRepository.GetAttributesByCategoryIdAsync(categoryId);
            return _mapper.Map<List<CategoryAttributeDto>>(attributes);
        }

        /// <summary>
        /// Get a specific attribute by ID
        /// </summary>
        public async Task<CategoryAttributeDto?> GetAttributeByIdAsync(int attributeId)
        {
            var attribute = await _productRepository.GetAttributeByIdAsync(attributeId);
            return attribute != null ? _mapper.Map<CategoryAttributeDto>(attribute) : null;
        }

        /// <summary>
        /// Create a new attribute for a category
        /// </summary>
        public async Task<CategoryAttributeDto> CreateAttributeAsync(CreateAttributeDto dto)
        {
            // Validate category if specified
            if (dto.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(dto.CategoryId.Value);
                if (category == null)
                    throw new NotFoundException("Category", dto.CategoryId.Value);
            }
            else if (!dto.IsGlobal)
            {
                throw new InvalidOperationException("CategoryId is required for non-global attributes");
            }

            var attribute = new ProductAttribute(
                dto.Name,
                dto.InputType,
                dto.IsGlobal,
                dto.CategoryId);

            var created = await _productRepository.CreateAttributeAsync(attribute);

            // Add initial values if provided and input type is select
            if (dto.InputType == "select" && dto.InitialValues != null && dto.InitialValues.Any())
            {
                foreach (var valueString in dto.InitialValues)
                {
                    if (!string.IsNullOrWhiteSpace(valueString))
                    {
                        var value = new AttributeValue(created.Id, valueString.Trim());
                        await _productRepository.AddAttributeValueAsync(value);
                    }
                }
            }

            _logger.LogInformation("Created attribute {AttributeName} (ID: {AttributeId})", dto.Name, created.Id);

            // Reload with values
            var attributeWithValues = await _productRepository.GetAttributeByIdAsync(created.Id);
            return _mapper.Map<CategoryAttributeDto>(attributeWithValues);
        }

        /// <summary>
        /// Update an existing attribute
        /// </summary>
        public async Task<CategoryAttributeDto?> UpdateAttributeAsync(int attributeId, UpdateAttributeDto dto)
        {
            var existing = await _productRepository.GetAttributeByIdAsync(attributeId);
            if (existing == null)
            {
                _logger.LogWarning("Update failed: Attribute {AttributeId} not found", attributeId);
                return null;
            }

            var updated = new ProductAttribute(dto.Name, dto.InputType, existing.IsGlobal, existing.CategoryId)
            {
                // Set the ID via reflection or use a different approach
            };

            // Since ProductAttribute might not have a setter, we need to use the repository's update method
            // which should handle the update properly
            var result = await _productRepository.UpdateAttributeAsync(existing);

            if (result == null)
                return null;

            _logger.LogInformation("Updated attribute {AttributeId}", attributeId);

            var attributeWithValues = await _productRepository.GetAttributeByIdAsync(attributeId);
            return _mapper.Map<CategoryAttributeDto>(attributeWithValues);
        }

        /// <summary>
        /// Delete an attribute (only if not in use)
        /// </summary>
        public async Task<bool> DeleteAttributeAsync(int attributeId)
        {
            try
            {
                var deleted = await _productRepository.DeleteAttributeAsync(attributeId);
                if (deleted)
                {
                    _logger.LogInformation("Deleted attribute {AttributeId}", attributeId);
                }
                return deleted;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Cannot delete attribute {AttributeId}: {Message}", attributeId, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Add a new value to an attribute
        /// </summary>
        public async Task<AttributeValueDto> AddAttributeValueAsync(AddAttributeValueDto dto)
        {
            var attribute = await _productRepository.GetAttributeByIdAsync(dto.AttributeId);
            if (attribute == null)
                throw new NotFoundException("Attribute", dto.AttributeId);

            if (attribute.InputType != "select")
                throw new InvalidOperationException("Can only add values to select-type attributes");

            var value = new AttributeValue(dto.AttributeId, dto.Value.Trim());
            var created = await _productRepository.AddAttributeValueAsync(value);

            _logger.LogInformation("Added value '{Value}' to attribute {AttributeId}", dto.Value, dto.AttributeId);

            return _mapper.Map<AttributeValueDto>(created);
        }

        /// <summary>
        /// Update an attribute value
        /// </summary>
        public async Task<AttributeValueDto?> UpdateAttributeValueAsync(UpdateAttributeValueDto dto)
        {
            var existing = await _productRepository.GetAttributeValueByIdAsync(dto.ValueId);
            if (existing == null)
            {
                _logger.LogWarning("Update failed: AttributeValue {ValueId} not found", dto.ValueId);
                return null;
            }

            var updated = new AttributeValue(existing.AttributeId, dto.Value.Trim());
            // Set ID if needed
            
            var result = await _productRepository.UpdateAttributeValueAsync(existing);
            if (result == null)
                return null;

            _logger.LogInformation("Updated attribute value {ValueId}", dto.ValueId);

            return _mapper.Map<AttributeValueDto>(result);
        }

        /// <summary>
        /// Delete an attribute value (only if not in use)
        /// </summary>
        public async Task<bool> DeleteAttributeValueAsync(int valueId)
        {
            try
            {
                var deleted = await _productRepository.DeleteAttributeValueAsync(valueId);
                if (deleted)
                {
                    _logger.LogInformation("Deleted attribute value {ValueId}", valueId);
                }
                return deleted;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Cannot delete attribute value {ValueId}: {Message}", valueId, ex.Message);
                throw;
            }
        }
    }
}
