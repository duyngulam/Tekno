using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Blog.DTOs;
using Tekno.Application.Blog.Interface;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common;
using Tekno.Application.Common.Exceptions;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Common.Media.Services;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Blog;

namespace Tekno.Application.Blog.Services
{
    public class BlogPostService
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IProductRepository _productRepository;
        private readonly MediaService _mediaService;
        private readonly IMapper _mapper;
        private readonly IAppLogger<BlogPostService> _logger;

        public BlogPostService(
            IBlogPostRepository blogPostRepository,
            IProductRepository productRepository,
            MediaService mediaService,
            IMapper mapper,
            IAppLogger<BlogPostService> logger)
        {
            _blogPostRepository = blogPostRepository;
            _productRepository = productRepository;
            _mediaService = mediaService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<BlogPostSummaryDto>> GetPagedAsync(BlogPostQueryDto query)
        {
            var paging = new PagingParams(query.Page, query.PageSize);

            // Parse status
            BlogPostStatus? status = null;
            if (!string.IsNullOrEmpty(query.Status) &&
                Enum.TryParse<BlogPostStatus>(query.Status, true, out var parsedStatus))
            {
                status = parsedStatus;
            }

            var result = await _blogPostRepository.GetPagedAsync(
                query.Tag,
                status,
                query.Keyword,
                paging);

            // Use AutoMapper for mapping
            var dtos = _mapper.Map<List<BlogPostSummaryDto>>(result.Data);

            return new PagedResult<BlogPostSummaryDto>(dtos, result.TotalRecords, result.Page, result.PageSize);
        }

        public async Task<BlogPostDetailDto?> GetBySlugAsync(string slug)
        {
            var blogPost = await _blogPostRepository.GetBySlugAsync(slug);
            if (blogPost == null) return null;

            await _blogPostRepository.IncrementViewCountAsync(blogPost.Id);

            return await MapToDetailDtoAsync(blogPost);
        }

        public async Task<BlogPostDetailDto?> GetByIdAsync(int id)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost == null) return null;

            return await MapToDetailDtoAsync(blogPost);
        }

        public async Task<List<BlogPostSummaryDto>> GetRecentPostsAsync(int count = 5)
        {
            var blogPosts = await _blogPostRepository.GetRecentPublishedAsync(count);

            // Use AutoMapper for mapping
            return _mapper.Map<List<BlogPostSummaryDto>>(blogPosts);
        }

        public async Task<List<BlogPostSummaryDto>> GetRelatedPostsAsync(int blogPostId, int count = 3)
        {
            var blogPosts = await _blogPostRepository.GetRelatedPostsAsync(blogPostId, count);

            // Use AutoMapper for mapping
            return _mapper.Map<List<BlogPostSummaryDto>>(blogPosts);
        }

        public async Task<BlogPostDetailDto> CreateAsync(CreateBlogPostDto dto, int authorId)
        {
            // Check slug uniqueness
            if (await _blogPostRepository.IsSlugExistsAsync(dto.Slug))
            {
                throw new ConflictException($"Blog post with slug '{dto.Slug}' already exists", "SLUG_EXISTS");
            }

            // Upload featured image
            var imageUrl = await _mediaService.UploadImageAsync(dto.FeaturedImage, "tekno/blog");

            try
            {
                var blogPost = new BlogPost(
                    title: dto.Title,
                    slug: dto.Slug,
                    summary: dto.Summary,
                    content: dto.Content,
                    featuredImageUrl: imageUrl,
                    authorId: authorId);

                // Add tags
                foreach (var tag in dto.Tags)
                {
                    blogPost.AddTag(tag);
                }

                // Add related products
                foreach (var productId in dto.RelatedProductIds)
                {
                    // Validate product exists
                    if (await _productRepository.IsProductExistByIdAsync(productId))
                    {
                        blogPost.AddRelatedProduct(productId);
                    }
                }

                // Publish immediately if requested
                if (dto.PublishImmediately)
                {
                    blogPost.Publish();
                }

                var created = await _blogPostRepository.CreateAsync(blogPost);

                _logger.LogInformation("Created blog post {Title} (ID: {Id})", created.Title, created.Id);

                return await MapToDetailDtoAsync(created);
            }
            catch
            {
                // Clean up uploaded image on error
                await _mediaService.DeleteImageAsync(imageUrl);
                throw;
            }
        }

        public async Task<BlogPostDetailDto?> UpdateAsync(int id, UpdateBlogPostDto dto)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost == null) return null;

            string? oldImageUrl = null;

            // Upload new featured image if provided
            if (dto.FeaturedImage != null)
            {
                oldImageUrl = blogPost.FeaturedImageUrl;
                var newImageUrl = await _mediaService.UploadImageAsync(dto.FeaturedImage, "tekno/blog");
                blogPost.Update(dto.Title, dto.Summary, dto.Content, newImageUrl);
            }
            else
            {
                blogPost.Update(dto.Title, dto.Summary, dto.Content, blogPost.FeaturedImageUrl);
            }

            try
            {
                // Update tags
                blogPost.ClearTags();
                foreach (var tag in dto.Tags)
                {
                    blogPost.AddTag(tag);
                }

                // Update related products
                blogPost.ClearRelatedProducts();
                foreach (var productId in dto.ProductId)
                {
                    if (await _productRepository.IsProductExistByIdAsync(productId))
                    {
                        blogPost.AddRelatedProduct(productId);
                    }
                }

                var updated = await _blogPostRepository.UpdateAsync(blogPost);

                // Delete old image if new one was uploaded
                if (oldImageUrl != null)
                {
                    await _mediaService.DeleteImageAsync(oldImageUrl);
                }

                _logger.LogInformation("Updated blog post {Id}", id);

                return await MapToDetailDtoAsync(updated);
            }
            catch
            {
                // Clean up new image on error
                if (dto.FeaturedImage != null && oldImageUrl != null)
                {
                    await _mediaService.DeleteImageAsync(blogPost.FeaturedImageUrl);
                }
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost == null) return false;

            var success = await _blogPostRepository.DeleteAsync(id);

            if (success)
            {
                // Delete featured image
                await _mediaService.DeleteImageAsync(blogPost.FeaturedImageUrl);
                _logger.LogInformation("Deleted blog post {Id}", id);
            }

            return success;
        }

        public async Task<bool> PublishAsync(int id)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost == null) return false;

            blogPost.Publish();
            await _blogPostRepository.UpdateAsync(blogPost);

            _logger.LogInformation("Published blog post {Id}", id);
            return true;
        }

        public async Task<bool> UnpublishAsync(int id)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost == null) return false;

            blogPost.Unpublish();
            await _blogPostRepository.UpdateAsync(blogPost);

            _logger.LogInformation("Unpublished blog post {Id}", id);
            return true;
        }

        /// <summary>
        /// Maps BlogPost to BlogPostDetailDto with related products loaded
        /// Uses AutoMapper for base mapping, manually loads related products
        /// </summary>
        private async Task<BlogPostDetailDto> MapToDetailDtoAsync(BlogPost blogPost)
        {
            // Use AutoMapper for base mapping
            var dto = _mapper.Map<BlogPostDetailDto>(blogPost);

            // Load related products (requires async operation)
            foreach (var relatedProduct in blogPost.RelatedProducts)
            {
                var product = await _productRepository.GetProductByIdAsync(relatedProduct.ProductId);
                if (product != null)
                {
                    dto.RelatedProducts.Add(new RelatedProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Slug = product.Slug,
                        PrimaryImagePath = product.Images?.FirstOrDefault()?.ImageUrl ?? string.Empty,
                        BasePrice = product.BasePrice
                    });
                }
            }

            return dto;
        }
    }
}
