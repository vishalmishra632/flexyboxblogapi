using BlogAPI.Core.Common;
using BlogAPI.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAPI.Core.Interfaces
{
    public interface IBlogService
    {
        Task<Result<List<BlogPostDto>>> GetAllPostsAsync();
        Task<Result<BlogPostDto>> GetPostByIdAsync(int id);
        Task<Result<BlogPostDto>> CreatePostAsync(CreateBlogPostDto dto);
        Task<Result<BlogPostDto>> UpdatePostAsync(int id, UpdateBlogPostDto dto);
        Task<Result<bool>> DeletePostAsync(int id);
    }
}
