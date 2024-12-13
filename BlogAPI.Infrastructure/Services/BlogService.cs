using BlogAPI.Core.Common;
using BlogAPI.Core.DTOs;
using BlogAPI.Core.Interfaces;
using BlogAPI.Core.Entities;

namespace BlogAPI.Infrastructure.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogPostRepository _repository;

        public BlogService(IBlogPostRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<BlogPostDto>>> GetAllPostsAsync()
        {
            try
            {
                // Retrieve all posts from the repository
                var posts = await _repository.GetAllAsync();

                // Map the entity objects to DTOs
                var dtos = posts.Select(p => new BlogPostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedDate = p.CreatedDate,
                    LastModifiedDate = p.LastModifiedDate
                }).ToList();

                return Result<List<BlogPostDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<List<BlogPostDto>>.Failure($"Error retrieving posts: {ex.Message}");
            }
        }

        public async Task<Result<BlogPostDto>> GetPostByIdAsync(int id)
        {
            try
            {
                // Retrieve the specific post from the repository
                var post = await _repository.GetByIdAsync(id);

                if (post == null)
                {
                    return Result<BlogPostDto>.Failure($"Blog post with ID {id} not found");
                }

                // Map the entity to DTO
                var dto = new BlogPostDto
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    CreatedDate = post.CreatedDate,
                    LastModifiedDate = post.LastModifiedDate
                };

                return Result<BlogPostDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<BlogPostDto>.Failure($"Error retrieving post: {ex.Message}");
            }
        }

        public async Task<Result<BlogPostDto>> CreatePostAsync(CreateBlogPostDto dto)
        {
            try
            {
                // Create a new BlogPost entity from the DTO
                var post = new BlogPost
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    CreatedDate = DateTime.UtcNow
                };

                // Save to repository
                var createdPost = await _repository.AddAsync(post);

                // Map the created entity back to DTO
                var createdDto = new BlogPostDto
                {
                    Id = createdPost.Id,
                    Title = createdPost.Title,
                    Content = createdPost.Content,
                    CreatedDate = createdPost.CreatedDate,
                    LastModifiedDate = createdPost.LastModifiedDate
                };

                return Result<BlogPostDto>.Success(createdDto);
            }
            catch (Exception ex)
            {
                return Result<BlogPostDto>.Failure($"Error creating post: {ex.Message}");
            }
        }

        public async Task<Result<BlogPostDto>> UpdatePostAsync(int id, UpdateBlogPostDto dto)
        {
            try
            {
                // First, check if the post exists
                var existingPost = await _repository.GetByIdAsync(id);
                if (existingPost == null)
                {
                    return Result<BlogPostDto>.Failure($"Blog post with ID {id} not found");
                }

                // Update the existing post
                existingPost.Title = dto.Title;
                existingPost.Content = dto.Content;
                existingPost.LastModifiedDate = DateTime.UtcNow;

                // Save changes
                await _repository.UpdateAsync(existingPost);

                // Map to DTO
                var updatedDto = new BlogPostDto
                {
                    Id = existingPost.Id,
                    Title = existingPost.Title,
                    Content = existingPost.Content,
                    CreatedDate = existingPost.CreatedDate,
                    LastModifiedDate = existingPost.LastModifiedDate
                };

                return Result<BlogPostDto>.Success(updatedDto);
            }
            catch (Exception ex)
            {
                return Result<BlogPostDto>.Failure($"Error updating post: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeletePostAsync(int id)
        {
            try
            {
                // Check if the post exists
                var existingPost = await _repository.GetByIdAsync(id);
                if (existingPost == null)
                {
                    return Result<bool>.Failure($"Blog post with ID {id} not found");
                }

                // Delete the post
                await _repository.DeleteAsync(id);

                return Result<bool>.Success(true, "Post deleted successfully");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error deleting post: {ex.Message}");
            }
        }
    }
}