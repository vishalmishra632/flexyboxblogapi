using BlogAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAPI.Core.Interfaces
{
    public interface IBlogPostRepository : IRepository<BlogPost>
    {
        // Add any blog-specific repository methods here
        Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count);
    }
}
