using BlogAPI.Core.Entities;
using BlogAPI.Core.Interfaces;
using BlogAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Infrastructure.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BlogDbContext _context;

        public BlogPostRepository(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await _context.BlogPosts.ToListAsync();
        }

        public async Task<BlogPost> GetByIdAsync(int id)
        {
            return await _context.BlogPosts.FindAsync(id);
        }

        public async Task<BlogPost> AddAsync(BlogPost entity)
        {
            await _context.BlogPosts.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(BlogPost entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.BlogPosts.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count)
        {
            return await _context.BlogPosts
                .OrderByDescending(x => x.CreatedDate)
                .Take(count)
                .ToListAsync();
        }
    }
}