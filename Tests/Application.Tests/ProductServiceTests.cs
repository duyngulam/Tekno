using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Tekno.Application.Catalog.DTOs.Admin;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Catalog.Services;
using Tekno.Application.Common.Cache;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Common.Media.Services;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;
using Xunit;

namespace Application.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepo = new();
        private readonly Mock<IElasticProductService> _elasticService = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<ICloudinaryService> _cloudinary = new();
        private readonly MediaService _mediaService;
        private readonly Mock<IAppLogger<ProductService>> _logger = new();
        private readonly Mock<ICacheService> _cache = new();

        private readonly Mock<IDbContextTransaction> _dbTransaction = new();

        private readonly ProductService _service;

        public ProductServiceTests()
        {
            // Create real MediaService with mocked cloudinary dependency
            _mediaService = new MediaService(_cloudinary.Object);

            // Default transaction mock: CommitAsync/RollbackAsync no-op
            _dbTransaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _dbTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _dbTransaction.Setup(t => t.DisposeAsync()).Returns(new ValueTask());

            _productRepo.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(_dbTransaction.Object);

            _service = new ProductService(
                _productRepo.Object,
                _elastic_service_obj(),
                _mapper.Object,
                _mediaService,
                _logger.Object,
                _cache.Object
            );
        }

        private IElasticProductService _elastic_service_obj()
        {
            return _elasticService.Object;
        }

        [Fact]
        public async Task GetProductDetailAsync_Should_Return_Null_For_Empty_Slug()
        {
            var result = await _service.GetProductDetailAsync("");
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetProductDetailAsync_Should_Return_MappedDto_When_Product_Found()
        {
            // Arrange
            var slug = "test-product";
            var product = new Product { Id = 1, Name = "P1", Slug = slug };
            var dto = new ProductDetailDto { Id = 1, Name = "P1" };

            _productRepo.Setup(r => r.GetProductBySlugAsync(slug)).ReturnsAsync(product);
            _mapper.Setup(m => m.Map<ProductDetailDto>(product)).Returns(dto);

            // Act
            var result = await _service.GetProductDetailAsync(slug);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("P1");

            _productRepo.Verify(r => r.GetProductBySlugAsync(slug), Times.Once);
            _mapper.Verify(m => m.Map<ProductDetailDto>(product), Times.Once);
        }

        [Fact]
        public async Task CreateProductAsync_Should_Throw_When_Slug_Exists_In_Db()
        {
            // Arrange
            var dto = new CreateProductDto { Name = "X", Slug = "x" };
            _productRepo.Setup(r => r.IsProductExistBySlugAsync(dto.Slug)).ReturnsAsync(true);
            _elasticService.Setup(e => e.IsProductExistBySlug(dto.Slug)).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _service.CreateProductAsync(dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Product with slug 'x' already exists");

            _productRepo.Verify(r => r.IsProductExistBySlugAsync(dto.Slug), Times.Once);
        }

        [Fact]
        public async Task CreateProductAsync_Should_Create_Product_When_SlugNotExist()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                Name = "NewProd",
                Slug = "new-prod",
                CategoryId = 2,
                BrandId = 3,
                BasePrice = 100m,
                Images = new List<IFormFile>()
            };

            var mappedProduct = new Product { Name = dto.Name, Slug = dto.Slug, CategoryId = dto.CategoryId, BrandId = dto.BrandId, BasePrice = dto.BasePrice };
            var createdProduct = new Product { Id = 111, Name = dto.Name, Slug = dto.Slug, CategoryId = dto.CategoryId, BrandId = dto.BrandId, BasePrice = dto.BasePrice };

            _productRepo.Setup(r => r.IsProductExistBySlugAsync(dto.Slug)).ReturnsAsync(false);
            _elasticService.Setup(e => e.IsProductExistBySlug(dto.Slug)).ReturnsAsync(false);
            _mapper.Setup(m => m.Map<Product>(dto)).Returns(mappedProduct);

            _cloudinary.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ReturnsAsync("http://images/1.jpg");

            _productRepo.Setup(r => r.AddProductAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product p) => { p.Id = createdProduct.Id; return p; });

            _elasticService.Setup(e => e.IndexProductAsync(It.IsAny<ProductSummaryDto>())).Returns(Task.CompletedTask);
            _mapper.Setup(m => m.Map<CreateProductDto>(It.IsAny<Product>())).Returns(dto);

            // Act
            var result = await _service.CreateProductAsync(dto);

            // Assert
            result.Should().NotBeNull();
            // No images in dto.Images so cloudinary shouldn't be called
            _cloudinary.Verify(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Never);
            _productRepo.Verify(r => r.AddProductAsync(It.IsAny<Product>()), Times.Once);
            _elasticService.Verify(e => e.IndexProductAsync(It.IsAny<ProductSummaryDto>()), Times.Once);
        }

        [Fact]
        public async Task CreateProductAsync_Should_Cleanup_UploadedImages_On_Failure()
        {
            // Arrange
            // Mock IFormFile so we don't depend on concrete FormFile type
            var fileMock = new Mock<IFormFile>();
            var content = "dummy image";
            var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns("img2.jpg");
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var dto = new CreateProductDto
            {
                Name = "FailProd",
                Slug = "fail-prod",
                CategoryId = 2,
                BrandId = 3,
                BasePrice = 50m,
                Images = new List<IFormFile> { fileMock.Object }
            };

            _productRepo.Setup(r => r.IsProductExistBySlugAsync(dto.Slug)).ReturnsAsync(false);
            _elastic_service_setup_for_slug_false(dto.Slug);
            _mapper.Setup(m => m.Map<Product>(dto)).Returns(new Product { Name = dto.Name, Slug = dto.Slug });

            // Upload returns url but DB insert fails
            _cloudinary.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ReturnsAsync("http://images/fail.jpg");

            _productRepo.Setup(r => r.AddProductAsync(It.IsAny<Product>())).ThrowsAsync(new Exception("DB error"));

            _cloudinary.Setup(c => c.DeleteImageByUrlAsync(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _service.CreateProductAsync(dto);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("DB error");
            _cloudinary.Verify(c => c.DeleteImageByUrlAsync("http://images/fail.jpg"), Times.Once);
        }

        private void _elastic_service_setup_for_slug_false(string slug)
        {
            _elasticService.Setup(e => e.IsProductExistBySlug(slug)).ReturnsAsync(false);
        }

        [Fact]
        public async Task UpdateProductAsync_Should_Throw_When_NewSlug_Already_Exists()
        {
            // Arrange
            var id = 200;
            var existing = new Product { Id = id, Slug = "old-slug" };
            var dto = new CreateProductDto { Name = "Updated", Slug = "new-slug" };

            _productRepo.Setup(r => r.GetProductByIdAsync(id)).ReturnsAsync(existing);
            _productRepo.Setup(r => r.IsProductExistBySlugAsync(dto.Slug)).ReturnsAsync(true);
            _elastic_service_setup_for_slug_false(dto.Slug);

            Func<Task> act = async () => await _service.UpdateProductAsync(id, dto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Product with slug 'new-slug' already exists");
        }

        [Fact]
        public async Task DeleteProductAsync_Should_Return_False_When_Product_Not_Found()
        {
            // Arrange
            _productRepo.Setup(r => r.GetProductByIdAsync(123)).ReturnsAsync((Product?)null);

            // Act
            var result = await _service.DeleteProductAsync(123);

            // Assert
            result.Should().BeFalse();
            _productRepo.Verify(r => r.GetProductByIdAsync(123), Times.Once);
        }

        [Fact]
        public async Task AddProductImageAsync_Should_Throw_NotFound_When_Product_Missing()
        {
            // Arrange
            var dto = new AddProductImageDto { ProductId = 99, ImageFile = null! };
            _productRepo.Setup(r => r.GetProductByIdAsync(dto.ProductId)).ReturnsAsync((Product?)null);

            // Act
            Func<Task> act = async () => await _service.AddProductImageAsync(dto);

            await act.Should().ThrowAsync<Tekno.Application.Common.Exceptions.NotFoundException>();
        }

        [Fact]
        public async Task UpdateProductImageAsync_Should_Return_False_When_Image_Not_Found()
        {
            // Arrange
            var dto = new UpdateProductImageDto { ImageId = 999 };
            _productRepo.Setup(r => r.GetProductImageByIdAsync(dto.ImageId)).ReturnsAsync((ProductImage?)null);

            // Act
            var result = await _service.UpdateProductImageAsync(dto);

            // Assert
            result.Should().BeFalse();
        }

        // New tests
        [Fact]
        public async Task AddProductImageAsync_Should_Add_Image_When_ProductExists()
        {
            // Arrange
            var productId = 10;
            var product = new Product { Id = productId, Slug = "p-slug" };
            var existingImages = new List<ProductImage>
            {
                new ProductImage(productId, "http://old.jpg", true, 0) { }
            };

            // Create a non-empty IFormFile mock so MediaService.ValidateFile passes
            var fileMock = new Mock<IFormFile>();
            var content = "dummy image content";
            var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns("img.jpg");
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var dto = new AddProductImageDto { ProductId = productId, ImageFile = fileMock.Object, IsPrimary = true };

            _productRepo.Setup(r => r.GetProductByIdAsync(productId)).ReturnsAsync(product);
            _productRepo.Setup(r => r.GetProductImagesAsync(productId)).ReturnsAsync(existingImages);
            _cloudinary.Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("http://new.jpg");

            var created = new ProductImage(productId, "http://new.jpg", true, 1);
            _productRepo.Setup(r => r.AddProductImageAsync(It.IsAny<ProductImage>())).ReturnsAsync(created);
            _productRepo.Setup(r => r.GetProductByIdAsync(productId)).ReturnsAsync(product);
            _mapper.Setup(m => m.Map<ProductSummaryDto>(It.IsAny<Product>())).Returns(new ProductSummaryDto { Id = productId, Name = "P" });
            _elasticService.Setup(e => e.IndexProductAsync(It.IsAny<ProductSummaryDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.AddProductImageAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.ImageUrl.Should().Be("http://new.jpg");
            _cloudinary.Verify(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Once);
            _productRepo.Verify(r => r.AddProductImageAsync(It.IsAny<ProductImage>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProductImageAsync_Should_Delete_And_Cleanup_Image()
        {
            // Arrange
            var imageId = 55;
            var image = new ProductImage(20, "http://to.delete.jpg", false, 0);
            var product = new Product { Id = 20, Slug = "p" };

            _productRepo.Setup(r => r.GetProductImageByIdAsync(imageId)).ReturnsAsync(image);
            _productRepo.Setup(r => r.DeleteProductImageAsync(imageId)).ReturnsAsync(true);
            _productRepo.Setup(r => r.GetProductByIdAsync(image.ProductId)).ReturnsAsync(product);
            _mapper.Setup(m => m.Map<ProductSummaryDto>(It.IsAny<Product>())).Returns(new ProductSummaryDto { Id = product.Id, Name = "P" });
            _elasticService.Setup(e => e.IndexProductAsync(It.IsAny<ProductSummaryDto>())).Returns(Task.CompletedTask);
            _cloudinary.Setup(c => c.DeleteImageByUrlAsync(image.ImageUrl)).ReturnsAsync(true);

            // Act
            var ok = await _service.DeleteProductImageAsync(imageId);

            // Assert
            ok.Should().BeTrue();
            _cloudinary.Verify(c => c.DeleteImageByUrlAsync(image.ImageUrl), Times.Once);
            _productRepo.Verify(r => r.DeleteProductImageAsync(imageId), Times.Once);
        }

        [Fact]
        public async Task ReorderProductImagesAsync_Should_Update_Order_And_Return_List()
        {
            // Arrange
            var productId = 30;
            var imagesInitial = new List<ProductImage>
            {
                new ProductImage(productId, "a.jpg", false, 0) { },
                new ProductImage(productId, "b.jpg", false, 1) { }
            };

            // assign deterministic ids to images using reflection because Id setter is private
            var idProp = typeof(ProductImage).GetProperty("Id", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)!;
            idProp.SetValue(imagesInitial[0], 1);
            idProp.SetValue(imagesInitial[1], 2);

            // request to reorder: swap order to [2,1]
            var dto = new ReorderImagesDto { ProductId = productId, ImageIds = new List<int> { imagesInitial[1].Id, imagesInitial[0].Id } };

            // prepare returned reordered images with ids set via reflection
            var imgB = new ProductImage(productId, "b.jpg", false, 0);
            var imgA = new ProductImage(productId, "a.jpg", false, 1);
            idProp.SetValue(imgB, 2);
            idProp.SetValue(imgA, 1);

            // first call returns existing images, second returns reordered list
            _productRepo.SetupSequence(r => r.GetProductImagesAsync(productId))
                .ReturnsAsync(imagesInitial)
                .ReturnsAsync(new List<ProductImage> { imgB, imgA });

            _productRepo.Setup(r => r.UpdateProductImageAsync(It.IsAny<ProductImage>())).ReturnsAsync(true);
            _productRepo.Setup(r => r.GetProductByIdAsync(productId)).ReturnsAsync(new Product { Id = productId, Slug = "s" });
            _mapper.Setup(m => m.Map<ProductSummaryDto>(It.IsAny<Product>())).Returns(new ProductSummaryDto { Id = productId, Name = "P" });
            _elasticService.Setup(e => e.IndexProductAsync(It.IsAny<ProductSummaryDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.ReorderProductImagesAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThan(0);
            _productRepo.Verify(r => r.UpdateProductImageAsync(It.IsAny<ProductImage>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetProductVariantByIdAsync_Should_Return_MappedDto_When_Found()
        {
            // Arrange
            var variant = new ProductVariant(1, "sku", 10, 5, "available");
            variant = await Task.FromResult(variant);

            _productRepo.Setup(r => r.GetProductVariantByIdAsync(100)).ReturnsAsync(variant);
            _mapper.Setup(m => m.Map<ProductVariantDetailDto>(variant)).Returns(new ProductVariantDetailDto { Id = variant.Id, Sku = variant.Sku });

            // Act
            var dto = await _service.GetProductVariantByIdAsync(100);

            // Assert
            dto.Should().NotBeNull();
            dto!.Sku.Should().Be(variant.Sku);
        }

        [Fact]
        public async Task UpdateProductVariantAsync_Should_Return_Null_When_Variant_NotFound()
        {
            // Arrange
            _productRepo.Setup(r => r.GetProductVariantByIdAsync(999)).ReturnsAsync((ProductVariant?)null);

            // Act
            var result = await _service.UpdateProductVariantAsync(999, new UpdateProductVariantDto { Sku = "x", Price = 1, Stock = 1, Attributes = new List<VariantAttributeInputDto>() });

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteProductVariantAsync_Should_Delete_And_Reindex()
        {
            // Arrange
            var variant = new ProductVariant(40, "sku40", 10, 2, "available");
            variant = await Task.FromResult(variant);
            _productRepo.Setup(r => r.GetProductVariantByIdAsync(40)).ReturnsAsync(variant);
            _productRepo.Setup(r => r.DeleteProductVariantAsync(40)).ReturnsAsync(true);

            _productRepo.Setup(r => r.GetProductByIdAsync(variant.ProductId)).ReturnsAsync(new Product { Id = variant.ProductId, Name = "P" });
            _mapper.Setup(m => m.Map<ProductSummaryDto>(It.IsAny<Product>())).Returns(new ProductSummaryDto { Id = variant.ProductId, Name = "P" });
            _elasticService.Setup(e => e.IndexProductAsync(It.IsAny<ProductSummaryDto>())).Returns(Task.CompletedTask);

            // Act
            var ok = await _service.DeleteProductVariantAsync(40);

            // Assert
            ok.Should().BeTrue();
            _productRepo.Verify(r => r.DeleteProductVariantAsync(40), Times.Once);
            _elastic_service_verify_index_once();
        }

        private void _elastic_service_verify_index_once()
        {
            _elasticService.Verify(e => e.IndexProductAsync(It.IsAny<ProductSummaryDto>()), Times.Once);
        }

        [Fact]
        public async Task GetPagedProductAsync_Should_Use_ElasticService_When_Keyword_Present_And_Cache_Miss()
        {
            // Arrange
            var request = new ProductSearchRequestDto { Keyword = "phone", Page = 1, PageSize = 10 };
            var summary = new ProductSummaryDto { Id = 500, Name = "S" };
            var paged = new PagedResult<ProductSummaryDto>(new List<ProductSummaryDto> { summary }, 1, 1, 10);

            _cache.Setup(c => c.GetAsync<PagedResult<ProductSummaryDto>>(It.IsAny<string>())).ReturnsAsync((PagedResult<ProductSummaryDto>?)null);
            _elasticService.Setup(e => e.SearchProductsAsync(request.Keyword, request.Category, request.Brand, request.Filters, request.MinPrice, request.MaxPrice, request.Sort, request.Page, request.PageSize))
                .ReturnsAsync(paged);

            _productRepo.Setup(r => r.GetProductsRatingStatsAsync(It.IsAny<List<int>>())).ReturnsAsync(new Dictionary<int, (double, int)> { { 500, (4.5, 10) } });
            _cache.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<PagedResult<ProductSummaryDto>>(), It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.GetPagedProductAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
            _elasticService.Verify(e => e.SearchProductsAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Dictionary<string,string>?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _cache.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<PagedResult<ProductSummaryDto>>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}
